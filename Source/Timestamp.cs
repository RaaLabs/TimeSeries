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
    public class TimeStamp : Value<TimeStamp>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Implicitly convert <see cref="TimeStamp"/> to its <see cref="long"/> representation
        /// </summary>
        /// <param name="timeStamp"><see cref="TimeStamp"/> to get the <see cref="long"/> representation of</param>
        public static implicit operator long(TimeStamp timeStamp)
        {
            return timeStamp.Value;
        }

        /// <summary>
        /// Implicitly convert <see cref="long"/> to its <see cref="TimeStamp"/> representation
        /// </summary>
        /// <param name="timeStamp"><see cref="long"/> representation of <see cref="TimeStamp"/></param>
        public static implicit operator TimeStamp(long timeStamp)
        {
            return new TimeStamp { Value = timeStamp };
        }
    }
}