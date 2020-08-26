using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Dolittle.Lifecycle;
using Dolittle.Types;
using RaaLabs.TimeSeries.Modules.EventBus;
using Machine.Specifications;
using Moq;
using Dolittle.Logging;

namespace RaaLabs.TimeSeries.Modules.EventBus.given
{
    /// <summary>
    /// Provides a running event bus with the given emitters and consumers of events. The derived class will also receive member variables
    /// _emitters and _consumers, containing the instances of all the provided emitters and consumers.
    /// </summary>
    /// <typeparam name="Emitters">
    /// The different emitter classes of the system. This type must be a generic type containing all the different emitter classes.
    /// e.g. MyEmitters<MyFirstEmitter, MySecondEmitter>.
    /// </typeparam>
    /// <typeparam name="Consumers">
    /// The different consumer classes of the system. This type must be a generic type containing all the different consumer classes.
    /// e.g. MyConsumers<MyFirstConsumer, MySecondConsumer>.
    /// </typeparam>
    public class an_EventBus<Emitters, Consumers>
    {
        protected static EventBus _eventBus;
        protected static ILogger _logger;

        protected static Emitters _emitters;
        protected static Consumers _consumers;

        protected static Thread _eventBusThread;

        class EmittedEvent { }

        private class Instances<T> : IInstancesOf<T> where T : class
        {
            private List<T> _inner;

            public Instances(params T[] instances)
            {
                _inner = instances.ToList();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _inner.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _inner.GetEnumerator();
            }
        }

        Establish context = () =>
        {
            var emittersToCreate = typeof(Emitters).GetInterfaces().Where(_ => _.GetInterface("IUseEventBus") != null).First().GenericTypeArguments;
            var emitterInstances = emittersToCreate.Select(_ => Activator.CreateInstance(_)).ToArray();

            _emitters = (Emitters) Activator.CreateInstance(typeof(Emitters));

            // Populate the _emitters class with the instantiated emitter classes
            foreach (var emitter in emitterInstances)
            {
                var prop = typeof(Emitters).GetProperties().Where(_ => _.PropertyType == emitter.GetType());
                if (prop.Count() > 0)
                {
                    prop.First().SetValue(_emitters, emitter);
                }
            }

            var emitters = new Instances<IEmitEvent>(emitterInstances.Select(_ => (IEmitEvent)((_.GetType().BaseType == typeof(Mock)) ? (_ as Mock).Object : _)).ToArray());

            var consumersToCreate = typeof(Consumers).GetInterfaces().Where(_ => _.GetInterface("IUseEventBus") != null).First().GenericTypeArguments;
            var consumerInstances = consumersToCreate.Select(_ => Activator.CreateInstance(_)).ToArray();
            _consumers = (Consumers)Activator.CreateInstance(typeof(Consumers));

            // Populate the _consumers class with the instantiated consumer classes
            foreach (var consumer in consumerInstances)
            {
                var prop = typeof(Consumers).GetProperties().Where(_ => _.PropertyType == consumer.GetType());
                if (prop.Count() > 0)
                {
                    prop.First().SetValue(_consumers, consumer);
                }
            }

            var consumers = new Instances<IConsumeEvent>(consumerInstances.Select(_ => (IConsumeEvent)((_.GetType().BaseType == typeof(Mock)) ? (_ as Mock).Object : _)).ToArray());

            var logger = new Mock<ILogger>();
            _logger = logger.Object;
            _eventBus = new EventBus(emitters, consumers, logger.Object);

            _eventBus.Perform();

            _eventBusThread = new Thread(async _ => {
                await _eventBus.Run();
            });
            _eventBusThread.Start();
        };

        private static IEmitEvent InstantiateEmitter(Type type)
        {
            return Activator.CreateInstance(type) as IEmitEvent;
        }

        private static IConsumeEvent InstantiateConsumer(Type type)
        {
            return Activator.CreateInstance(type) as IConsumeEvent;
        }
    }

    public interface IUseEventBus { }
    public interface IUseEventBus<A> : IUseEventBus where A: class { }
    public interface IUseEventBus<A, B> : IUseEventBus where A : class where B : class { }
    public interface IUseEventBus<A, B, C> : IUseEventBus where A : class where B : class where C : class { }
    public interface IUseEventBus<A, B, C, D> : IUseEventBus where A : class where B : class where C : class where D : class { }
}
