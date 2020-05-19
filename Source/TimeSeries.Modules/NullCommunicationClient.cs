/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Threading.Tasks;

namespace RaaLabs.TimeSeries.Modules
{
    /// <summary>
    /// Represents a null implementation of <see cref="ICommunicationClient"/>
    /// </summary>
    public class NullCommunicationClient : ICommunicationClient
    {
        /// <summary>
        /// 
        /// </summary>
        public NullCommunicationClient()
        {
        }
        /// <inheritdoc/>
        public void RegisterFunctionHandler(Delegate methodHandler)
        {

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