/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents the configuration for a single <see cref="IAmAPullConnector"/>
    /// </summary>
    public class PullConnectorConfiguration
    {
        /// <summary>
        /// Gets or sets the pull interval in milliseconds used for the connector
        /// </summary>
        public int Interval { get; set; }
    }
}