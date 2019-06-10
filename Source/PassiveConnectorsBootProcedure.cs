/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;
using Dolittle.Types;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IPassiveConnectors"/>
    /// </summary>
    public class PassiveConnectorsBootProcedure : ICanPerformBootProcedure
    {
        readonly IPassiveConnectors _passiveConnectors;

        /// <summary>
        /// Initializes a new instance of <see cref="PassiveConnectorsBootProcedure"/>
        /// </summary>
        /// <param name="passiveConnectors"></param>
        
        public PassiveConnectorsBootProcedure(IPassiveConnectors passiveConnectors)
        {
            _passiveConnectors = passiveConnectors;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _passiveConnectors.Start();
        }
    }
}