/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents the configuration for <see cref="IPullConnectors"/>
    /// </summary>
    [Name("PullConnectors")]
    public class PullConnectorsConfiguration : IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PullConnectorsConfiguration"/>
        /// </summary>
        /// <param name="sources"></param>
        public PullConnectorsConfiguration(IDictionary<Source, PullConnectorConfiguration> sources)
        {
            Sources = new ReadOnlyDictionary<Source,PullConnectorConfiguration>(sources);
        }

        /// <summary>
        /// Gets the <see cref="Source">sources</see> and their <see cref="PullConnectorConfiguration"/>
        /// </summary>
        public IReadOnlyDictionary<Source, PullConnectorConfiguration>  Sources { get; }
    }
}