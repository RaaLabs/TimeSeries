/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Concepts;

namespace Dolittle.TimeSeries
{
    /// <summary>
    /// Represents the concept of an System
    /// </summary>
    public class Source : ConceptAs<string>
    {
        /// <summary>
        /// Implicitly convert from <see cref="string"/> to <see cref="Source"/>
        /// </summary>
        /// <param name="value">System as string</param>
        public static implicit operator Source(string value)
        {
            return new Source { Value = value };
        }
    }
}