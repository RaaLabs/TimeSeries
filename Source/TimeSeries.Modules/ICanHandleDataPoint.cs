/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;

namespace RaaLabs.TimeSeries.Modules
{
    /// <summary>
    /// Defines a handler of a <see cref="DataPoint{T}"/>
    /// </summary>
    /// <typeparam name="TValue">Type of value for the <see cref="DataPoint{T}"/></typeparam>
    /// <remarks>
    /// If you want to support any type in the datapoint, you can simply use dynamic as the value type
    /// </remarks>
    public interface ICanHandleDataPoint<TValue> : IInputHandler
    {
        /// <summary>
        /// Handle a <see cref="DataPoint{T}"/>
        /// </summary>
        /// <param name="dataPoint"><see cref="DataPoint{T}"/> to handle</param>
        Task Handle(DataPoint<TValue> dataPoint);
    }
}