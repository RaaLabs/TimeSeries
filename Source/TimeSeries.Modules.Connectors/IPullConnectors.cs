/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.TimeSeries.Modules.Connectors
{
    /// <summary>
    /// Defines a system for working with all <see cref="IAmAPullConnector">pull connectors</see>
    /// </summary>
    public interface IPullConnectors
    {
        /// <summary>
        /// Start all <see cref="IAmAPullConnector">pull connectors</see>
        /// </summary>
        void Start();
    }
}