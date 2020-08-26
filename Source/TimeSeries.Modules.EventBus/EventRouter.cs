using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Logging;
using System.Linq;

namespace RaaLabs.TimeSeries.Modules.EventBus
{
    interface EventRouter
    {
        Task Run();
    }

    class EventRouter<T> : EventRouter
    {
        private Channel<T> _events;
        private IList<IConsumeEvent<T>> _consumers;

        public EventRouter(IList<IConsumeEvent> consumers)
        {
            _events = Channel.CreateUnbounded<T>();
            _consumers = consumers.Select(_ => (IConsumeEvent<T>) _).ToList();
        }

        public async Task Run()
        {
            while(true)
            {
                var @event = await _events.Reader.ReadAsync();
                await Task.WhenAll(_consumers.Select(_ => _.Consume(@event)));
            }
        }

        public void HandleEmittedEvent(T @event)
        {
            Task.Run(() => _events.Writer.WriteAsync(@event)).Wait();
        }

    }
}
