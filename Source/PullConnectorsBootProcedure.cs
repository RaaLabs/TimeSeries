/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents a <see cref="ICanPerformBootProcedure">boot procedure</see> for starting <see cref="IPullConnectors"/>
    /// </summary>
    public class PullConnectorsBootProcedure : ICanPerformBootProcedure
    {
        readonly IPullConnectors _pullConnectors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pullConnectors"></param>
        public PullConnectorsBootProcedure(IPullConnectors pullConnectors)
        {
            _pullConnectors = pullConnectors;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _pullConnectors.Start();
        }
    }
}