/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace Dolittle.Edge.Modules
{
    /// <summary>
    /// Represents the delegate of a subscriber of an <see cref="Input"/>
    /// </summary>
    /// <param name="payload">Deserialized payload</param>
    /// <typeparam name="T">Type of the payload</typeparam>
    /// <returns>Task</returns>
    public delegate Task Subscriber<T>(T payload);
}