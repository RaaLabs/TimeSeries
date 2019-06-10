/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents a passive connector that connects and data streamed through the channel it
    /// connects to at the cadence decided by the server
    /// </summary>
    public interface IAmAPassiveConnector
    {
        /// <summary>
        /// Connect to the system 
        /// </summary>
        void Connect();
    }
}