using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Dolittle.Lifecycle;
using Dolittle.Types;
using RaaLabs.TimeSeries.Modules.EventBus;
using Machine.Specifications;
using Moq;
using Dolittle.Logging;

namespace RaaLabs.TimeSeries.Modules.EventBus.given
{
    /// <inheritdoc/>
    public class an_EventBus<Emitters, Consumers>
    {
        protected static EventBus _eventBus;
        protected static ILogger _logger;

        protected static Emitters _emitters;
        protected static Consumers _consumers;

        protected static Thread _eventBusThread;

        class EmittedEvent { }

        private class Instances<T> : IInstancesOf<T> where T : class
        {
            private List<T> _inner;

            public Instances(params T[] instances)
            {
                _inner = instances.ToList();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _inner.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _inner.GetEnumerator();
            }
        }

        Establish context = () =>
        {
            var emitterInstances = typeof(Emitters).GenericTypeArguments.Select(_ => Activator.CreateInstance(_) as IEmitEvent).ToArray();
            _emitters = (Emitters) Activator.CreateInstance(typeof(Emitters));

            foreach (var emitter in emitterInstances)
            {
                var prop = typeof(Emitters).GetProperties().Where(_ => _.PropertyType == emitter.GetType());
                if (prop.Count() > 0)
                {
                    prop.First().SetValue(_emitters, emitter);
                }
            }

            var emitters = new Instances<IEmitEvent>(emitterInstances);

            var consumerInstances = typeof(Consumers).GenericTypeArguments.Select(_ => Activator.CreateInstance(_) as IConsumeEvent).ToArray();
            _consumers = (Consumers)Activator.CreateInstance(typeof(Consumers));

            foreach (var consumer in consumerInstances)
            {
                var prop = typeof(Consumers).GetProperties().Where(_ => _.PropertyType == consumer.GetType());
                if (prop.Count() > 0)
                {
                    prop.First().SetValue(_consumers, consumer);
                }
            }

            var consumers = new Instances<IConsumeEvent>(consumerInstances);

            var logger = new Mock<ILogger>();
            _logger = logger.Object;
            _eventBus = new EventBus(emitters, consumers, logger.Object);

            _eventBus.Perform();

            _eventBusThread = new Thread(async _ => {
                await _eventBus.Run();
            });
            _eventBusThread.Start();
        };
    }
}
