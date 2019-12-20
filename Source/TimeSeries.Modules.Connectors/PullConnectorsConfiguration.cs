/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dolittle.Configuration;

namespace RaaLabs.TimeSeries.Modules.Connectors
{
    /// <summary>
    /// Represents the configuration for <see cref="IPullConnectors"/>
    /// </summary>
    [Name("PullConnectors")]
    public class PullConnectorsConfiguration : 
        ReadOnlyDictionary<Source, PullConnectorConfiguration>,
        IConfigurationObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="PullConnectorsConfiguration"/>
        /// </summary>
        /// <param name="sources"></param>
        public PullConnectorsConfiguration(IDictionary<Source, PullConnectorConfiguration> sources) : base(sources)
        {
        }
    }
}