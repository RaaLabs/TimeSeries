/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Reflection;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Lifecycle;
using Dolittle.Types;

namespace Dolittle.TimeSeries.Modules
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

        /// <summary>
        /// Initializes a new instance of <see cref="InputHandlers"/>
        /// </summary>
        /// <param name="client"><see cref="ICommunicationClient"/> to use</param>
        /// <param name="typeFinder"><see cref="ITypeFinder"/> for discovering types</param>
        /// <param name="container"><see cref="IContainer"/> for activation of services</param>
        public InputHandlers(
            ICommunicationClient client,
            ITypeFinder typeFinder,
            IContainer container)
        {
            _client = client;
            _container = container;
            _typeFinder = typeFinder;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            var handlers = _typeFinder.FindMultiple(typeof(ICanHandle<>));
            handlers.ForEach(_ => SetupSubscriptionFor(_, typeof(ICanHandle<>)));

            var dataPointHandlers = _typeFinder.FindMultiple(typeof(ICanHandleDataPoint<>));
            dataPointHandlers.ForEach(_ => SetupSubscriptionFor(_, typeof(ICanHandleDataPoint<>), typeof(DataPoint<>)));
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
    }
}