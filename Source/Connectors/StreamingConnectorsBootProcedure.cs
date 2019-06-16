/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Booting;

namespace Dolittle.TimeSeries.Modules.Connectors
{
    /// <summary>
    /// Represents the <see cref="ICanPerformBootProcedure">boot procedure</see> for <see cref="IStreamingConnectors"/>
    /// </summary>
    public class StreamingConnectorsBootProcedure : ICanPerformBootProcedure
    {
        readonly IStreamingConnectors _streamingConnectors;

        /// <summary>
        /// Initializes a new instance of <see cref="StreamingConnectorsBootProcedure"/>
        /// </summary>
        /// <param name="streamingConnectors"></param>
        
        public StreamingConnectorsBootProcedure(IStreamingConnectors streamingConnectors)
        {
            _streamingConnectors = streamingConnectors;
        }

        /// <inheritdoc/>
        public bool CanPerform() => true;

        /// <inheritdoc/>
        public void Perform()
        {
            _streamingConnectors.Start();
        }
    }
}