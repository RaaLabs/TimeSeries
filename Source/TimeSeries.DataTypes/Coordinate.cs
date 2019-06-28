/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.TimeSeries.DataTypes
{
    /// <summary>
    /// Represents a physical coordinate
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Gets or sets the location latitude in decimal degrees
        /// </summary>
        public Measurement<float> Latitude {  get; set; }

        /// <summary>
        /// Gets or sets the location longitude in decimal degrees
        /// </summary>
        public Measurement<float> Longitude {  get; set; }
    }
}