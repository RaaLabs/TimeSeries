using Dolittle.Booting;
using Dolittle.Logging;
using Dolittle.Types;
using Dolittle.DependencyInversion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace RaaLabs.TimeSeries.Modules
{
    class AsyncContext : ICanPerformBootProcedure
    {
        /// <inheritdoc/>
        public bool CanPerform() => _instances.All(_ => _.ReadyToRun());

        IContainer _container;
        IInstancesOf<IRunInAsyncContext> _instances;
        ILogger _logger;

        public AsyncContext(IContainer container, IInstancesOf<IRunInAsyncContext> instances, ILogger logger)
        {
            _container = container;
            _instances = instances;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Perform()
        {
            var instances = _instances.Select(_ => _container.Get(_.GetType()) as IRunInAsyncContext).ToList();

            var thread = new Thread(_ => {
                _logger.Information($"Running {instances.Count} tasks in parallel.");
                var allTasks = instances.Select(instance => instance.Run());
                Task.WhenAll(allTasks).Wait();
            });
            thread.Start();
        }
    }
}