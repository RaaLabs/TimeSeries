/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

namespace Dolittle.Edge.Modules
{
    /// <summary>
    /// Defines an <see cref="IInputHandler"/>
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Get the input the messages is expected on
        /// </summary>
        Input Input {  get; }
    }
}