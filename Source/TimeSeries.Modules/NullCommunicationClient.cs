/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Threading.Tasks;
using Dolittle.Logging;

namespace RaaLabs.TimeSeries.Modules
{
    /// <summary>
    /// Represents a null implementation of <see cref="ICommunicationClient"/>
    /// </summary>
    public class NullCommunicationClient : ICommunicationClient
    {
        readonly ILogger _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public NullCommunicationClient(ILogger logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodHandler"></param>
        public void RegisterFunctionHandler(Delegate methodHandler)
        {
            var methodName = methodHandler.Method.Name;

            _logger.Information($"Registering method handler method '{methodName}'");
        }

        /// <inheritdoc/>
        public Task SendAsJson(Output output, object payload)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task SendRaw(Output output, byte[] payload)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void SubscribeTo<T>(Input input, Subscriber<T> subscriber)
        {
            
        }
    }
}