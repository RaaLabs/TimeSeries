/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Types;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents an implementation of <see cref="IPullConnectors"/>
    /// </summary>
    public class PullConnectors : IPullConnectors
    {
        readonly PullConnectorsConfiguration _configuration;
        readonly IInstancesOf<IAmAPullConnector> _connectors;
        readonly ICommunicationClient _communicationClient;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="PullConnectors"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="connectors"></param>
        /// <param name="communicationClient"></param>
        /// <param name="logger"></param>
        public PullConnectors(
            PullConnectorsConfiguration configuration,
            IInstancesOf<IAmAPullConnector> connectors,
            ICommunicationClient communicationClient,
            ILogger logger)
        {
            _configuration = configuration;
            _connectors = connectors;
            _communicationClient = communicationClient;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _logger.Information("Start pull connectors");
            var connectors = _connectors.ToDictionary(_ => _.Name, _ => _);

            foreach ((Source source, PullConnectorConfiguration configuration) in _configuration.Sources)
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