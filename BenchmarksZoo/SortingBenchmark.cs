using System.Linq;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [MemoryDiagnoser]
    public partial class SortingBenchmark
    {
        User[] Users = null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Users = User.Generate(256);
        }

        [Benchmark]
        public void SortByName()
        {
            var sorted = Users.OrderBy(x => x.Name).ToArray();
        }
        
    }
}