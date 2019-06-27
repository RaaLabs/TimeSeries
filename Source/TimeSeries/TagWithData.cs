/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.TimeSeries
{
    /// <summary>
    /// Represents data for a <see cref="Tag"/>
    /// </summary>
    public class TagWithData
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TagWithData"/>
        /// </summary>
        /// <param name="tag"><see cref="Tag"/> the data is for</param>
        /// <param name="data"><see cref="Data"/> for the tag</param>
        public TagWithData(Tag tag, object data)
        {
            Tag = tag;
            Data = data;
        }
        
        /// <summary>
        /// Gets the <see cref="Tag"/> the data is for
        /// </summary>
        public Tag Tag { get; }

        /// <summary>
        /// Get the value representing the data
        /// </summary>
        public object Data { get; }
    }
}