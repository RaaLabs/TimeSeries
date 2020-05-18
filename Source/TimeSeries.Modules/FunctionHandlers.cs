/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;

namespace RaaLabs.TimeSeries.Modules
{
    /// <summary>
    /// Represents the delegate of a function handler/>
    /// </summary>
    /// <typeparam name="ResultType">Type of the input payload</typeparam>
    /// <returns>Task</returns>
    public delegate Task<ResultType> FunctionHandler<ResultType>();

    /// <summary>
    /// Represents the delegate of a function handler/>
    /// </summary>
    /// <param name="payload">Deserialized payload</param>
    /// <typeparam name="PayloadType">Type of the input payload</typeparam>
    /// <typeparam name="ResultType">Type of the input payload</typeparam>
    /// <returns>Task</returns>
    public delegate Task<ResultType> FunctionHandler<PayloadType, ResultType>(PayloadType payload);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public delegate Task ActionHandler();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload">Deserialized payload</param>
    /// <typeparam name="PayloadType">Type of the input payload</typeparam>
    /// <returns></returns>
    public delegate Task ActionHandler<PayloadType>(PayloadType payload);
}