using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace Benchmark.Parallel
{
    public class Benchmark
    {
        private static TaskFactory TaskFactory = new TaskFactory();

        [Benchmark]
        [ArgumentsSource(nameof(SourceTaskScheduler))]
        public int TaskScheduler(int countActions, int maxParallelism, TaskFactory taskFactory)
        {
            List<Task> subTasks = new List<Task>(countActions);

            for (int i = 0; i < countActions; i++)
            {
                var task = taskFactory.StartNew(() => { });
                subTasks.Add(task);
            }

            subTasks.ForEach(task => task.Wait());
            return countActions;
        }

        public IEnumerable<object[]> SourceTaskScheduler()
        {
            var counts = new[] { 0, 1, 2, 100, 1000, 10000 };
            for (int cores = 1; cores <= Environment.ProcessorCount; cores++)
                foreach (var count in counts)
                {
                    TaskFactory taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount));
                    yield return new object[] { count, cores, taskFactory };
                }
        }



        [Benchmark]
        [ArgumentsSource(nameof(SourceParallelFor))]
        public int ParallelFor(int countActions, int maxParallelism)
        {
            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = maxParallelism };
            System.Threading.Tasks.Parallel.ForEach(Enumerable.Range(0, countActions), options, (i) => { });
            return countActions;
        }

        public IEnumerable<object[]> SourceParallelFor()
        {
            var counts = new[] { 0, 1, 2, 100, 1000, 10000 };
            for (int cores = 1; cores <= Environment.ProcessorCount; cores++)
                foreach (var count in counts)
                {
                    yield return new object[] { count, cores };
                }
        }

    }
}
