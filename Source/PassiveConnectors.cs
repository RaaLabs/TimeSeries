/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.Types;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents an implementation of <see cref="IPassiveConnectors"/>
    /// </summary>
    public class PassiveConnectors : IPassiveConnectors
    {
        readonly IInstancesOf<IAmAPassiveConnector> _connectors;

        /// <summary>
        /// Initializes a new instance of <see cref="PassiveConnectors"/>
        /// </summary>
        /// <param name="connectors">Instances of <see cref="IAmAPassiveConnector"/></param>
        public PassiveConnectors(IInstancesOf<IAmAPassiveConnector> connectors)
        {
            _connectors = connectors;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _connectors.ForEach(_ =>
            {
                Task.Run(() =>
                {
                    try
                    {

                    }
                    catch
                    {

                    }
                });
            });

        }
    }
}