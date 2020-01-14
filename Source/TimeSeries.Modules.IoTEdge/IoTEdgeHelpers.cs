/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace RaaLabs.TimeSeries.Modules.IoTEdge
{
    /// <summary>
    /// Holds helper methods for working with IoTEdge
    /// </summary>
    public class IoTEdgeHelpers
    {
        /// <summary>
        /// Check if we're running in IoT Edge context or not
        /// </summary>
        /// <returns>True if we are running in IoT Edge context, false if not</returns>
        public static bool IsRunningInIotEdge()
        {
            return  !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IOTEDGE_MODULEID"));
        }

    }


}