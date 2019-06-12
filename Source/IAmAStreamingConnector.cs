/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents a connector type that connects and streams data from the source at the cadence decided by the source
    /// </summary>
    public interface IAmAStreamingConnector
    {
        /// <summary>
        /// The event that represents data being received
        /// </summary>
        event DataReceived  DataReceived;

        /// <summary>
        /// Gets the name of the connector
        /// </summary>
        Source Name { get; }

        /// <summary>
        /// Connect to the system 
        /// </summary>
        void Connect();
    }
}