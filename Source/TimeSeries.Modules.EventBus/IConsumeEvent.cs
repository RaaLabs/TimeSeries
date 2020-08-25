using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RaaLabs.TimeSeries.Modules.EventBus
{
    /// <inheritdoc/>
    public interface IConsumeEvent { }

    /// <inheritdoc/>
    public interface IConsumeEvent<T> : IConsumeEvent
    {
        /// <inheritdoc/>
        Task Consume(T data);
    }
}
