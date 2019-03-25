/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Dolittle.Concepts;

namespace Dolittle.Edge.Modules
{
    /// <summary>
    /// Represents a timestamp in EPOCH microseconds
    /// </summary>
    public class Timestamp : Value<Timestamp>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Implicitly convert <see cref="Timestamp"/> to its <see cref="long"/> representation
        /// </summary>
        /// <param name="timeStamp"><see cref="Timestamp"/> to get the <see cref="long"/> representation of</param>
        public static implicit operator long(Timestamp timeStamp)
        {
            return timeStamp.Value;
        }

        /// <summary>
        /// Implicitly convert <see cref="long"/> to its <see cref="Timestamp"/> representation
        /// </summary>
        /// <param name="timeStamp"><see cref="long"/> representation of <see cref="Timestamp"/></param>
        public static implicit operator Timestamp(long timeStamp)
        {
            return new Timestamp { Value = timeStamp };
        }
    }
}