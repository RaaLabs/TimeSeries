using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RaaLabs.TimeSeries.Modules
{
    /// <inheritdoc/>
    public interface IRunInAsyncContext
    {
        /// <inheritdoc/>
        Task Run();

        /// <inheritdoc/>
        bool ReadyToRun();
    }
}
