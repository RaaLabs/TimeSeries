/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Concepts;

namespace Dolittle.TimeSeries
{
    /// <summary>
    /// Represents a timestamp in EPOCH milliseconds
    /// </summary>
    public class Timestamp : ConceptAs<long>
    {
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
            return new Timestamp {  Value = timeStamp };
        }

        /// <summary>
        /// Gets the UTC time for **NOW**
        /// </summary>
        public static Timestamp UtcNow => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}