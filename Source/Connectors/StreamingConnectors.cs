/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Logging;
using Dolittle.Types;
using Polly;

namespace Dolittle.TimeSeries.Modules.Connectors
{
    /// <summary>
    /// Represents an implementation of <see cref="IStreamingConnectors"/>
    /// </summary>
    public class StreamingConnectors : IStreamingConnectors
    {
        readonly IInstancesOf<IAmAStreamingConnector> _connectors;
        readonly ILogger _logger;
        readonly ICommunicationClient _communicationClient;

        /// <summary>
        /// Initializes a new instance of <see cref="StreamingConnectors"/>
        /// </summary>
        /// <param name="connectors">Instances of <see cref="IAmAStreamingConnector"/></param>
        /// <param name="communicationClient"><see cref="ICommunicationClient"/> for communication</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public StreamingConnectors(
            IInstancesOf<IAmAStreamingConnector> connectors,
            ICommunicationClient communicationClient,
            ILogger logger)
        {
            _connectors = connectors;
            _logger = logger;
            _communicationClient = communicationClient;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _connectors.ForEach(_ =>
            {
                Task.Run(() =>
                {
                    _.DataReceived += (tag, data, timestamp) => DataReceived(_, tag, data, timestamp);

                    var policy = Policy
                        .Handle<Exception>()
                        .WaitAndRetryForever(retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                            (exception, timeSpan, context) =>
                            {
                                _logger.Error(exception, $"Connector '{_.GetType()}' - with name '{_.Name}' threw an exception during connect - retrying");
                            });

                    policy.Execute(() =>
                    {
                        _.Connect();
                    });
                });
            });
        }

        void DataReceived(IAmAStreamingConnector connector, Tag tag, object data, Timestamp timestamp)
        {
            var dataPoint = new TagDataPoint<object>
            {
                Source = connector.Name,
                Tag = tag,
                Value = data,
                Timestamp = timestamp
            };
            _communicationClient.SendAsJson("output", dataPoint);
        }
    }
}