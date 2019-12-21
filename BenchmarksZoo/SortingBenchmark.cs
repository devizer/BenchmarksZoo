using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarksZoo.ClassicAlgorithms;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    /*[NativeMemoryProfiler]*/
    public class SortingBenchmark
    {
        User[] Users = null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Users = User.Generate(543);
        }

        [Benchmark(Description = "Enumerable.OrderBy")]
        public void Enumerable_OrderBy()
        {
            var sorted = Users.OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
        }

        [Benchmark(Description = "QuickSorter<T>.Sort")]
        public void QuickSort_NET20()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            QuickSorter<User>.QuickSort(copy, User.ComparerByName);
        }
        
        [Benchmark(Baseline = true, Description = "Array<T>.Sort")]
        public void Array_Sort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            Array.Sort(copy, User.ComparerByName);
        }
    }
}