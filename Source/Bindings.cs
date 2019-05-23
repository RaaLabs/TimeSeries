/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.DependencyInversion;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Defines a system for holding properties
    /// </summary>
    public interface IHoldProperties
    {

    }


    /// <summary>
    /// Provides bindings
    /// </summary>
    public class Bindings : ICanProvideBindings
    {
        /// <inheritdoc/>
        public void Provide(IBindingProviderBuilder builder)
        {
            if (!IsRunningInIotEdge())
                builder.Bind<ICommunicationClient>().To<NullCommunicationClient>();
            else
                builder.Bind<ICommunicationClient>().To<CommunicationClient>();
        }


        bool IsRunningInIotEdge()
        {
            return  !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("EdgeHubConnectionString")) ||
                    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IOTEDGE_MODULEID"));
        }
    }
}