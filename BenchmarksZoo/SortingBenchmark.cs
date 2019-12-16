using System;
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
            Users = User.Generate(543);
        }

        [Benchmark(Description = "Enumerable.OrderBy")]
        public void Linq_Sort()
        {
            var sorted = Users.OrderBy(x => x.Name).ToArray();
        }

        [Benchmark(Description = "QuickSort NET2.0")]
        public void Classic_QuickSort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            QuickSorter<User>.QuickSort(copy, User.ComparerByName);
        }
        
        [Benchmark(Baseline = true, Description = "Array.Sort")]
        public void Array_Sort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            Array.Sort(copy, User.ComparerByName);
        }
        
    }
}