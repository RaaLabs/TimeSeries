/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.DependencyInversion;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;

namespace Dolittle.Edge.Modules
{

    /// <summary>
    /// Provides bindings
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EdgeHubConnectionString")))
                builder.Bind<ICommunicationClient>().To<NullCommunicationClient>();
            else
                builder.Bind<ICommunicationClient>().To<CommunicationClient>();
        }
    }
}