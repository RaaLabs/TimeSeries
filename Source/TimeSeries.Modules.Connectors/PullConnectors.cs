/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Types;

namespace RaaLabs.TimeSeries.Modules.Connectors
{
    /// <summary>
    /// Represents an implementation of <see cref="IPullConnectors"/>
    /// </summary>
    public class PullConnectors : IPullConnectors
    {
        readonly IInstancesOf<IAmAPullConnector> _connectors;
        readonly ICommunicationClient _communicationClient;
        readonly ILogger _logger;
        readonly FactoryFor<PullConnectorsConfiguration> _configurationFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="PullConnectors"/>
        /// </summary>
        /// <param name="configurationFactory"></param>
        /// <param name="connectors"></param>
        /// <param name="communicationClient"></param>
        /// <param name="logger"></param>
        public PullConnectors(
            FactoryFor<PullConnectorsConfiguration> configurationFactory,
            IInstancesOf<IAmAPullConnector> connectors,
            ICommunicationClient communicationClient,
            ILogger logger)
        {
            _connectors = connectors;
            _communicationClient = communicationClient;
            _logger = logger;
            _configurationFactory = configurationFactory;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _logger.Information("Start pull connectors");
            var connectors = _connectors.ToDictionary(_ => _.Name, _ => _);

            if (connectors.Count > 0)
            {
                var configurationObject = _configurationFactory();

                foreach ((Source source, PullConnectorConfiguration configuration) in configurationObject)
                {
                    _logger.Information($"Starting '{source}'");
                    var timer = new Timer(configuration.Interval);
                    timer.Elapsed += (s, e) =>
                    {
                        var data = connectors[source].GetAllData();
                        data.ForEach(dataPoint => _communicationClient.SendAsJson("output", dataPoint));
                    };
                    timer.AutoReset = true;
                    timer.Enabled = true;
                }
            }
        }
    }
}