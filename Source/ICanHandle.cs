/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;

namespace Dolittle.Edge.Modules
{
    /// <summary>
    /// Defines a handle of a given type
    /// </summary>
    public interface ICanHandle<T> : IInputHandler
    {
        /// <summary>
        /// Handle an incoming instance of the type given
        /// </summary>
        /// <param name="value">A given value of the type for handler</param>
        Task Handle(T value);
    }
}