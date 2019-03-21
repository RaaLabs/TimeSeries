/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Serialization.Json;
using System.Text;

namespace Dolittle.Edge.Modules
{
    /// <summary>
    /// Represents an implementation of <see cref="IClient"/>
    /// </summary>
    [Singleton]
    public class Client : IClient
    {
        ModuleClient _client;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="Client"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="serializer"><see cref="ISerializer">JSON serializer</see></param>
        public Client(ILogger logger, ISerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
            logger.Information("Setting up ModuleClient");

            var mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            _client = null;

            ModuleClient.CreateFromEnvironmentAsync(settings)
                .ContinueWith(_ => _client = _.Result)
                .Wait();

            logger.Information("Open and wait");
            _client.OpenAsync().Wait();
            logger.Information("Client is ready");
            
        }


        /// <inheritdoc/>
        public Task SendEvent(Output output, Message message)
        {
            _logger.Information($"Send event to '{output}'");
            return _client.SendEventAsync(output, message);
        }

        /// <inheritdoc/>
        public Task SendEventAsJson(Output output, object @event)
        {
            _logger.Information($"Send event as JSON to '{output}'");
            var outputMessageString = _serializer.ToJson(@event);
            _logger.Information($"Event JSON: '{outputMessageString}'");
            var outputMessageBytes = Encoding.UTF8.GetBytes(outputMessageString);
            var outputMessage = new Message(outputMessageBytes);
            return _client.SendEventAsync(output, outputMessage);
        }

        /// <inheritdoc/>
        public Task SetInputMessageHandler(Input input, MessageHandler messageHandler, object userContext)
        {
            return SetInputMessageHandler(input, messageHandler, userContext);
        }
    }
}