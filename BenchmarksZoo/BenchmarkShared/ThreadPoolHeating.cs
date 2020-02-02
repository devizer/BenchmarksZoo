using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Universe
{
    public static class ThreadPoolHeating
    {
        public static void HeatThreadPool(int poolSize = 18)
        {
            ConcurrentDictionary<int,object> threads = new ConcurrentDictionary<int, object>();
            
            CountdownEvent started = new CountdownEvent(poolSize);
            int sum = 0;
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < poolSize; i++)
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    // threads[Thread.CurrentThread.ManagedThreadId] = null;
                    var n = Interlocked.Increment(ref sum);
                    Console.WriteLine($"{sw.Elapsed} Started {n} of {poolSize} thread");
                    started.Signal();
                    started.Wait();
                });
            }

            started.Wait();
        }

        
    }
}