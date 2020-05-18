/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Reflection;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Types;
using Dolittle.Logging;

namespace RaaLabs.TimeSeries.Modules
{
    /// <summary>
    /// Represents an implementation of <see cref="IInputHandlers"/>
    /// </summary>
    [Singleton]
    public class InputHandlers : IInputHandlers
    {
        readonly ICommunicationClient _client;
        readonly IContainer _container;
        readonly ITypeFinder _typeFinder;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="InputHandlers"/>
        /// </summary>
        /// <param name="client"><see cref="ICommunicationClient"/> to use</param>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> for discovering types</param>
        /// <param name="container"><see cref="IContainer"/> for activation of services</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public InputHandlers(
            ICommunicationClient client,
            ITypeFinder typeFinder,
            IContainer container,
            ILogger logger)
        {
            _client = client;
            _container = container;
            _typeFinder = typeFinder;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            var handlers = _typeFinder.FindMultiple(typeof(ICanHandle<>));
            handlers.ForEach(_ => SetupSubscriptionFor(_, typeof(ICanHandle<>)));

            var dataPointHandlers = _typeFinder.FindMultiple(typeof(ICanHandleDataPoint<>));
            dataPointHandlers.ForEach(_ => SetupSubscriptionFor(_, typeof(ICanHandleDataPoint<>), typeof(DataPoint<>)));

            var methodHandlers = _typeFinder.FindMultiple(typeof(ICanHandleMethods));
            methodHandlers.ForEach(_ => SetupMethodHandlingFor(_));
        }

        void SetupSubscriptionFor(Type handlerType, Type baseHandlerType, Type wrappedType = null) 
        {
            var handler = _container.Get(handlerType) as IInputHandler;
            var type = handlerType.GetInterfaces().Single(i => i.Name.StartsWith(baseHandlerType.Name));
            var dataType = type.GetGenericArguments()[0];
            if( wrappedType != null ) dataType = wrappedType.MakeGenericType(dataType);

            var delegateType = typeof(Subscriber<>).MakeGenericType(dataType);
            var subscribeToMethod = _client.GetType()
                                            .GetMethod("SubscribeTo",
                                                BindingFlags.Public | BindingFlags.Instance)
                                            .MakeGenericMethod(dataType);
            var handleDelegate = Delegate.CreateDelegate(delegateType, handler, "Handle");

            subscribeToMethod.Invoke(_client, new object[] { handler.Input, handleDelegate });
        }

        void SetupMethodHandlingFor(Type handlerType)
        {
            var handler = _container.Get(handlerType);
            var methods = handlerType.GetMethods().Where(method => method.GetCustomAttributes(true).Any(_ => _ is IotHubMethodAttribute));

            foreach (var method in methods)
            {
                var inputTypee = method.GetParameters().FirstOrDefault()?.ParameterType;
                var returnType = method.ReturnType.GetGenericArguments()?.FirstOrDefault();   // 'T' if ReturnType is 'Task<T>', 'null' if ReturnType is 'Task'
                var methodName = method.Name;

                _logger.Information($"Setting up method handling for '{methodName}'");

                var methodHandlerMethodd = _client.GetType().GetMethod("RegisterFunctionHandler", BindingFlags.Public | BindingFlags.Instance);
                var delegateTypee = MakeHandlerDelegate(inputTypee, returnType);
                var handleDelegatee = Delegate.CreateDelegate(delegateTypee, handler, methodName);

                methodHandlerMethodd.Invoke(_client, new object[] { handleDelegatee });
            }
        }

        private Type MakeHandlerDelegate(Type inputType, Type outputType)
        {
            if (outputType != null)
            {
                // Create function delegate
                return (inputType != null) ? typeof(FunctionHandler<,>).MakeGenericType(inputType, outputType) : typeof(FunctionHandler<>).MakeGenericType(outputType);
            }
            else
            {
                // Create action delegate
                return (inputType != null) ? typeof(ActionHandler<>).MakeGenericType(inputType) : typeof(ActionHandler);
            }
        }
    }
}