/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents the callback for when data is received
    /// </summary>
    /// <param name="tag">Name of the tag that holds the data</param>
    /// <param name="value">Value of any type</param>
    /// <param name="timestamp"><see cref="Timestamp"/> of the data received</param>
    public delegate void DataReceived(Tag tag, object value, Timestamp timestamp);
}