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
        /// Gets or sets the measurement error
        /// </summary>
        /// <remarks>
        /// Typicaly the value is of a number, an error of 0 would mean there is
        /// no deviations to be expected from the value - the value is 100% accurate.
        /// </remarks>
        public T Error { get; set; }
    }
}