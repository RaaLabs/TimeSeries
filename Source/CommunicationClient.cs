/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Text;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Serialization.Json;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;

namespace Dolittle.Edge.Modules
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommunicationClient"/>
    /// </summary>
    [Singleton]
    public class CommunicationClient : ICommunicationClient
    {
        ModuleClient _client;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CommunicationClient"/>
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="serializer"><see cref="ISerializer">JSON serializer</see></param>
        public CommunicationClient(ILogger logger, ISerializer serializer)
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
        public Task SendAsJson(Output output, object payload)
        {
            _logger.Information($"Send as JSON to '{output}'");
            var outputMessageString = _serializer.ToJson(payload);
            _logger.Information($"Payload: '{outputMessageString}'");
            var outputMessageBytes = Encoding.UTF8.GetBytes(outputMessageString);
            var outputMessage = new Message(outputMessageBytes);
            return _client.SendEventAsync(output, outputMessage);
        }


        /// <inheritdoc/>
        public Task SendRaw(Output output, byte[] payload)
        {
            _logger.Information($"Sending raw to '{output}");
            var outputMessage = new Message(payload);
            return _client.SendEventAsync(output, outputMessage);         
        }


        /// <inheritdoc/>
        public void SubscribeTo<T>(Input input, Subscriber<T> subscriber)
        {
            _logger.Information($"Subscribing to '{input}' for type '{typeof(T).AssemblyQualifiedName}'");
            _client.SetInputMessageHandlerAsync(input, async(message, context) =>
            {
                _logger.Information($"Handling incoming for '{subscriber.GetType().AssemblyQualifiedName}' on input '{input}'");
                return await HandleSubscriber(subscriber, message);
            }, null);

        }

        

        async Task<MessageResponse> HandleSubscriber<T>(Subscriber<T> subscriber, Message message)
        {
            try
            {
                var messageBytes = message.GetBytes();
                var messageString = Encoding.UTF8.GetString(messageBytes);
                var deserialized = _serializer.FromJson<T>(messageString);
                await subscriber(deserialized);
                return MessageResponse.Completed;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Error during handling");
                return MessageResponse.Abandoned;
            }
        }
    }
}