/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.TimeSeries.Formats
{
    /// <summary>
    /// Represents an actual geographical location
    /// </summary>
    public class Geolocation : Value<Geolocation>
    {
        /// <summary>
        /// Gets or sets the location latitude in decimal degrees
        /// </summary>
        public float Latitude {  get; set; }

        /// <summary>
        /// Gets or sets the location longitude in decimal degrees
        /// </summary>
        public float Longitude {  get; set; }

        /// <summary>
        /// Gets or sets the location altitude in meters relative to sea level
        /// </summary>
        public float Altitude {  get; set; }

        /// <summary>
        /// Gets or sets the accuracy of latitude and longitude expressed in meters
        /// </summary>
        public float Accuracy {  get; set; }

        /// <summary>
        /// Gets or sets the accuracy of altitude expressed in meters
        /// </summary>
        public float AltitudeAccuracy {  get; set; }

        /// <summary>
        /// Gets or sets the velocity of the device in meters per second
        /// </summary>
        public float Speed {  get; set; }

        /// <summary>
        /// Gets or sets the magnetic variation in decimal degrees
        /// </summary>
        public float MagneticVariation { get; set; }
    }
}