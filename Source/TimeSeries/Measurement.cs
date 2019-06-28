/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Concepts;

namespace Dolittle.TimeSeries
{
    /// <summary>
    /// Represents a measurement
    /// </summary>
    public class Measurement<T> : Value<Measurement<T>>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the precision of the value in the range 0 to 1
        /// </summary>
        public float Precision { get; set; }
    }
}