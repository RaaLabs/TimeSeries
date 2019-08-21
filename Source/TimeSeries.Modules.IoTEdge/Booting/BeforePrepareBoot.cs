/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;
using Dolittle.Logging;
using Microsoft.Azure.Devices.Client;

namespace Dolittle.TimeSeries.Modules.IoTEdge.Booting
{
    /// <summary>
    /// Represents a step that gets run after <see cref="BootStage.Basics"/> to setup necessary configuration
    /// for IoT Edge to work
    /// </summary>
    public class BeforePrepareBoot : ICanRunBeforeBootStage<NoSettings>
    {
        /// <inheritdoc/>
        public BootStage BootStage => BootStage.PrepareBoot;

        /// <inheritdoc/>
        public void Perform(NoSettings settings, IBootStageBuilder builder)
        {
            if (IoTEdgeHelpers.IsRunningInIotEdge())
            {
                var logger = builder.GetAssociation(WellKnownAssociations.Logger) as ILogger;
                ModuleClient client = null;                
                
                ModuleClient.CreateFromEnvironmentAsync(TransportType.Mqtt)
                  .ContinueWith(_ => client = _.Result)
                  .Wait();

                logger.Information("Open IoT Edge ModuleClient and wait");
                client.OpenAsync().Wait();
                logger.Information("Client is ready");

                builder.Bindings.Bind<ModuleClient>().To(client);
            }
        }

    }


}