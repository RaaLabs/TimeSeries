/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Concepts;

namespace Dolittle.TimeSeries.DataTypes
{
    /// <summary>
    /// Represents an actual geographical location
    /// </summary>
    public class Geolocation : Value<Geolocation>
    {
        /// <summary>
        /// Gets or sets the position part of the geolocation
        /// </summary>
        public Coordinate Position { get; set; }

        /// <summary>
        /// Gets or sets the location altitude in meters relative to sea level
        /// </summary>
        public Measurement<float> Altitude {  get; set; }

        /// <summary>
        /// Gets or sets the velocity of the device in meters per second
        /// </summary>
        public Measurement<float> Speed {  get; set; }

        /// <summary>
        /// Gets or sets the magnetic variation in decimal degrees
        /// </summary>
        public float MagneticVariation { get; set; }
    }
}