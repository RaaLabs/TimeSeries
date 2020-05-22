/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Serialization.Json;
using Microsoft.Azure.Devices.Client;

namespace RaaLabs.TimeSeries.Modules.IoTEdge
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommunicationClient"/>
    /// </summary>
    [Singleton]
    public class CommunicationClient : ICommunicationClient
    {
        readonly ModuleClient _client;
        readonly ISerializer _serializer;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="CommunicationClient"/>
        /// </summary>
        /// <param name="client">Underlying <see cref="ModuleClient"/></param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        /// <param name="serializer"><see cref="ISerializer">JSON serializer</see></param>
        public CommunicationClient(ModuleClient client, ILogger logger, ISerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;
            logger.Information("Setting up ModuleClient");

            _client = client;
        }

        /// <inheritdoc/>
        public void RegisterFunctionHandler(Delegate functionHandler)
        {
            var methodName = functionHandler.Method.Name;
            var inputType = functionHandler.Method.GetParameters().FirstOrDefault()?.GetType();
            var resultType = functionHandler.Method.ReturnType ?? typeof(Task);

            _logger.Information($"Registering method handler method '{methodName}'");
            _client.SetMethodHandlerAsync(methodName, async (request, context) =>
            {
                try
                {
                    var res = await ProcessMethod(request, functionHandler, _serializer);
                    return res;
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                    throw e;
                }
            }, null);
        }

        /// <inheritdoc/>
        public Task SendAsJson(Output output, object payload)
        {
            _logger.Information($"Send as JSON to '{output}'");
            var outputMessageString = _serializer.ToJson(payload, SerializationOptions.CamelCase);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="functionHandler"></param>
        /// <param name="serializer"></param>
        public static async Task<MethodResponse> ProcessMethod(MethodRequest request, Delegate functionHandler, ISerializer serializer)
        {
            var inputType = functionHandler.Method.GetParameters().FirstOrDefault()?.ParameterType;
            var resultType = functionHandler.Method.ReturnType ?? typeof(Task);

            var inputs = DeserializeMethodPayload(inputType, request.Data, serializer).ToArray();

            // Return type of functionHandler will be of either 'Task' or 'Task<T>' type.
            var resTask = (Task)functionHandler.DynamicInvoke(inputs);

            await resTask;

            // If result is of type 'Task<T>' rather than 'Task', that means that we should return data back to caller.
            if (resultType.IsGenericType)
            {
                var downcastedTask = Convert.ChangeType(resTask, resultType);
                var innerResultType = resultType.GetGenericArguments().FirstOrDefault();
                var property = typeof(Task<>).MakeGenericType(innerResultType).GetProperty("Result");
                var result = property.GetValue(downcastedTask);

                var outputMessageString = serializer.ToJson(result, SerializationOptions.CamelCase);
                var outputMessageBytes = Encoding.UTF8.GetBytes(outputMessageString);

                return new MethodResponse(outputMessageBytes, 200);
            }

            return new MethodResponse(200);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="data"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static IEnumerable<object> DeserializeMethodPayload(Type inputType, byte[] data, ISerializer serializer)
        {
            Type[] inputTypes = (inputType == null) ? new Type[] { } : new Type[] { inputType };
            var inputStrings = inputTypes.Select(_ => (_, Encoding.UTF8.GetString(data))).ToArray();
            var deserialized = inputStrings.Select(((Type fst, string snd) p) => serializer.FromJson(p.fst, p.snd, SerializationOptions.CamelCase)).ToArray();

            return deserialized;
        }
    }
}