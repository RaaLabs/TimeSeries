/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents an active connector that is typically connecting on a regular cadence
    /// </summary>
    public interface IAmAnActiveConnector
    {
        /// <summary>
        /// Connect to the system and return
        /// </summary>
        void Connect();
    }
}