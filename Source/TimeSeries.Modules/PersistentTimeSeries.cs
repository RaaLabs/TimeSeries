using RaaLabs.TimeSeries;
using System.Threading.Tasks;

namespace TimeSeries.Modules
{
    class PersistentTimeSeries<T>
    {
        public async Task Add(DataPoint<T> dataPoint)
        {
            await Task.CompletedTask;
        }
    }
}
