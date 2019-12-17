using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class SynchronizationLatencyBenchmark
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

        [Benchmark]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(8)]
        [Arguments(16)]
        public void SyncActionLatency_using_ThreadPool_and_Countdown(int Racers)
        {
            CountdownEvent race = new CountdownEvent(Racers);
            for (int i = 0; i < Racers; i++)
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

        [Benchmark]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(8)]
        [Arguments(16)]
        public void SyncActionLatency_using_ThreadPool_and_Barrier(int Racers)
        {
            Barrier barrier = new Barrier(Racers);
            ManualResetEventSlim done = new ManualResetEventSlim(false);
            for (int i = 0; i < Racers; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    barrier.SignalAndWait();
                    // lets rock&roll synchronously 
                    done.Set();
                });
            }

            done.Wait();
        }

        [Benchmark]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(8)]
        [Arguments(16)]
        public async Task SyncActionLatency_using_Tasks_and_Countdown(int Racers)
        {
            CountdownEvent race = new CountdownEvent(Racers);
            Task[] tasks = new Task[Racers];
            for (int i = 0; i < Racers; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    race.Signal();
                    race.Wait();
                    // lets rock&roll synchronously 
                });
            }

            Task.WaitAll(tasks);
        }
        
        [Benchmark]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(8)]
        [Arguments(16)]
        public async Task SyncActionLatency_using_Tasks_and_Barrier(int Racers)
        {
            Barrier barrier = new Barrier(Racers);
            Task[] tasks = new Task[Racers];
            for (int i = 0; i < Racers; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    barrier.SignalAndWait();
                    // lets rock&roll synchronously 
                });
            }

            Task.WaitAll(tasks);
        }
    }
}