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

        [Benchmark]
        public void ThreadLatencyWithJoin()
        {
            int y;
            Thread t = new Thread(_ => y = 42);
            t.Start();
            t.Join();
        }

        [Benchmark]
        public void ThreadLatencyWithWait()
        {
            ManualResetEvent done = new ManualResetEvent(false);
            Thread t = new Thread(_ => done.Set());
            t.Start();
            done.WaitOne();
            t.Join();
        }

        [Benchmark]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(8)]
        [Arguments(16)]
        public void CountdownLatency(int racers)
        {
            CountdownEvent race = new CountdownEvent(racers);
            for (int i = 0; i < racers; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    race.Signal();
                    race.Wait();
                    // lets rock&roll synchronously 
                });
            }

            race.Wait();
        }
    }
}