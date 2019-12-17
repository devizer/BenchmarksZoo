using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class SyncLatencyBenchmark
    {
        
        [Benchmark]
        public async Task<int> AwaitLatency()
        {
            var y = await Task.Run(() => 42);
            return y;
        }

        [Benchmark]
        public void ThreadPoolLatency()
        {
            ManualResetEvent done = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(_ => done.Set());
            done.WaitOne();
        }
    }
}