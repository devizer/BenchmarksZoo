using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarksZoo.ClassicAlgoruthms;

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
        public void Default_Sort()
        {
            var sorted = Users.OrderBy(x => x.Name).ToArray();
        }

        [Benchmark]
        public void Classic_QuickSort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0; i < Users.Length; i++) copy[i] = Users[i];
            QuickSorter<User>.QuickSort(copy, User.ComparerByName);
        }
        
    }
}