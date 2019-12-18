using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class SyncParticipantsBenchmark
    {

        [Benchmark]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(8)]
        [Arguments(16)]
        public void Using_ThreadPool_and_Countdown(int Racers)
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
        public void Using_ThreadPool_and_Barrier(int Racers)
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
        public async Task Using_Tasks_and_Countdown(int Racers)
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
        public async Task Using_Tasks_and_Barrier(int Racers)
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