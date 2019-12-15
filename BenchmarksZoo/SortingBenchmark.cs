using System.Linq;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class SortingBenchmark
    {
        User[] Users = null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Users = User.Generate(2);
        }

        [Benchmark]
        public void SortByName()
        {
            var sorted = Users.OrderBy(x => x.Name).ToArray();
        }
        
    }
}