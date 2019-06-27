/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.TimeSeries.Modules.Connectors
{
    /// <summary>
    /// Defines a system that is in charge of managing <see cref="IAmAStreamingConnector">passive connectors</see>
    /// </summary>
    public interface IStreamingConnectors
    {
        /// <summary>
        /// Start all connectors
        /// </summary>
        void Start();
    }
}