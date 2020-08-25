using Dolittle.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaaLabs.TimeSeries.Modules.EventBus
{
    /// <inheritdoc/>
    public interface IEmitEvent { }

    /// <inheritdoc/>
    public interface IEmitEvent<T> : IEmitEvent { }
}
