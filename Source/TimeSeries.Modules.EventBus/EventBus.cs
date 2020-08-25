using Dolittle.Booting;
using Dolittle.Types;
using Dolittle.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Collections.Generic;
using Dolittle.Lifecycle;
using Dolittle.DependencyInversion.Scopes;
using RaaLabs.TimeSeries.Modules;

namespace RaaLabs.TimeSeries.Modules.EventBus
{
    /// <summary>
    /// 
    /// </summary>
    [Singleton]
    public class EventBus : ICanPerformBootProcedure, IRunInAsyncContext
    {
        /// <inheritdoc/>
        public bool CanPerform() => true;

        IList<IEmitEvent> _eventEmitters;
        IList<IConsumeEvent> _eventConsumers;
        IList<EventRouter> _eventRouters;
        
        ILogger _logger;
        bool _readyToRun = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventEmitters"></param>
        /// <param name="eventConsumers"></param>
        /// <param name="logger"></param>
        public EventBus(IInstancesOf<IEmitEvent> eventEmitters, IInstancesOf<IConsumeEvent> eventConsumers, ILogger logger)
        {
            _logger = logger;
            _eventEmitters = eventEmitters.Distinct().ToList();
            _eventConsumers = eventConsumers.Distinct().ToList();

            VerifyIsSingleton(_eventEmitters.Select(_ => _.GetType()));
            VerifyIsSingleton(_eventConsumers.Select(_ => _.GetType()));

            _eventRouters = new List<EventRouter>();
        }

        /// <inheritdoc/>
        public void Perform()
        {
            foreach (var emitterClass in _eventEmitters)
            {
                SetupEventEmittersForClass(emitterClass);
            }
            _readyToRun = true;
        }

        void SetupEventEmittersForClass(IEmitEvent emitterClass)
        {
            // Get a list of all events emitted by class
            var emittedEvents = emitterClass.GetType().GetInterfaces()
                .Where(_ => _.GetInterfaces().Contains(typeof(IEmitEvent)))
                .Select(_ => _.GetGenericArguments().First())
                .Distinct();

            // Get all events containing generic arguments, as these are the only candidates for being event emitters
            var events = emitterClass.GetType().GetEvents()
                .Where(_ => _.EventHandlerType.GetGenericArguments().Length > 0).ToList();

            // Filter all events actually containing event generic parameters
            var emitters = events
                .Where(_ => emittedEvents.Contains(_.EventHandlerType.GetGenericArguments().First())).ToList();

            // Get the types of all emitted events
            var declaredEvents = emitters.Select(_ => _.EventHandlerType.GetGenericArguments().First()).ToList();
            var undeclaredEvents = emittedEvents.Except(declaredEvents).Select(_ => _.Name).ToList();
            if (undeclaredEvents.Count > 0)
            {
                throw new Exception($"Undeclared events: {String.Join(",", undeclaredEvents)}");
            }
            //var emitters = events.Where(_ => _.EventHandlerType.Name == typeof(Action<>).Name).ToList();

            foreach(var emitter in emitters)
            {
                try
                {
                    var eventType = emitter.EventHandlerType.GenericTypeArguments.First();
                    var consumerType = typeof(IConsumeEvent<>).MakeGenericType(eventType);

                    var consumers = _eventConsumers
                        .Where(_ => _.GetType().GetInterfaces().Contains(consumerType))
                        .ToList();
                    Type delegateType = typeof(Action<>).MakeGenericType(eventType);

                    var eventHandlerClass = typeof(EventRouter<>).MakeGenericType(eventType);
                    var instance = (EventRouter) Activator.CreateInstance(eventHandlerClass, consumers);
                    _eventRouters.Add(instance);

                    var method = eventHandlerClass.GetMethod("HandleEmittedEvent");
                    var handleDelegate = Delegate.CreateDelegate(delegateType, instance, method);
                    emitter.AddEventHandler(emitterClass, handleDelegate);
                    
                }
                catch (Exception e)
                {
                    _logger.Information($"Unable to set up emitter function for emitter class. Reason: {e}");
                }
            }
        }

        /// <inheritdoc/>
        public async Task Run()
        {
            await Task.WhenAll(_eventRouters.Select(async _ => await _.Run()));
        }

        private void VerifyIsSingleton(IEnumerable<Type> types)
        {
            foreach(var type in types)
            {
                VerifyIsSingleton(type);
            }
        }

        private void VerifyIsSingleton(Type type)
        {
            bool hasSingletonAttribute = type.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(SingletonAttribute));
            if (!hasSingletonAttribute)
            {
                throw new Exception($"Class should be singleton: {type}");
            }
        }

        /// <inheritdoc/>
        public bool ReadyToRun() => _readyToRun;
    }
}
