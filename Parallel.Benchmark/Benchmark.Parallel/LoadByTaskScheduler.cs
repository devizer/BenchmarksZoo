using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace Benchmark.Parallel
{
    public class LoadByParallelFor
    {
        public int Do(int countActions, int maxParallelism)
        {
            ParallelOptions options = new ParallelOptions() { MaxDegreeOfParallelism = maxParallelism };
            System.Threading.Tasks.Parallel.ForEach(Enumerable.Range(0, countActions), options, (i) => { });
            return countActions;
        }
    }

    internal class LoadByTaskScheduler
    {
        public int Do(int countActions, TaskFactory taskFactory)
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
    }
}
