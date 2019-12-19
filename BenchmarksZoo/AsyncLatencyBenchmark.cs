using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    /*[NativeMemoryProfiler]*/
    public class AsyncLatencyBenchmark
    {

        [Benchmark]
        public async Task<int> AwaitLatency()
        {
            var y = await Task.Run(() => 42);
            return y;
        }

        [Benchmark]
        public void ThreadPoolLatency_Slim()
        {
            ManualResetEventSlim done = new ManualResetEventSlim(false);
            ThreadPool.QueueUserWorkItem(_ => done.Set());
            done.Wait();
        }

        [Benchmark]
        public void ThreadPoolLatency_Legacy()
        {
            ManualResetEvent done = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(_ => done.Set());
            done.WaitOne();
        }

        [Benchmark]
        public void ThreadLatencyWith_Join()
        {
            int y;
            Thread t = new Thread(_ => y = 42);
            t.Start();
            t.Join();
        }

        [Benchmark]
        public void ThreadLatencyWith_SlimWait()
        {
            ManualResetEventSlim done = new ManualResetEventSlim(false);
            Thread t = new Thread(_ => done.Set());
            t.Start();
            done.Wait();
            // t.Join();
        }

        [Benchmark]
        public void ThreadLatencyWith_LegacyWait()
        {
            ManualResetEvent done = new ManualResetEvent(false);
            Thread t = new Thread(_ => done.Set());
            t.Start();
            done.WaitOne();
            // t.Join();
        }
    }
}