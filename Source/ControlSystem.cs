/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using Dolittle.Concepts;

namespace Dolittle.TimeSeries.Modules
{
    /// <summary>
    /// Represents the concept of an System
    /// </summary>
    public class ControlSystem : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="ControlSystem"/>
        /// </summary>
        /// <param name="value">System as string</param>
        public static implicit operator ControlSystem(string value)
        {
            return new ControlSystem { Value = value };
        }
    }
}