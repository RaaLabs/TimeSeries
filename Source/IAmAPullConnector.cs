/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents a connector type that pulls data from its source
    /// </summary>
    public interface IAmAPullConnector
    {
        /// <summary>
        /// Gets the name of the connector
        /// </summary>
        Source Name {  get; }

        /// <summary>
        /// Connect to the system and return
        /// </summary>
        /// <param name="tag"><see cref="Tag"/> to get for</param>
        /// <returns>The data for the <see cref="Tag"/></returns>
        object GetData(Tag tag);

        /// <summary>
        /// Get all available data for all <see cref="Tag">tags</see>
        /// </summary>
        IEnumerable<TagWithData> GetAllData();
    }
}