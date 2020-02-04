using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarksZoo.ClassicAlgorithms;
using Universe;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    /*[NativeMemoryProfiler]*/
    public class SortingBenchmark
    {
        [Params(543, 1000*1000)]
        public int ArraySize { get; set; }
        
        User[] Users = null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Users = User.Generate(ArraySize);
            QuickSort_NET20_2Threads();
            QuickSort_NET20_4Threads();
            ThreadPoolHeating.HeatThreadPool(9);
        }

        [Benchmark(Description = "Enumerable.OrderBy")]
        public void Enumerable_OrderBy()
        {
            var sorted = Users.OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
        }

        [Benchmark(Baseline = true, Description = "QuickSorter<T>.Sort")]
        public void QuickSort_NET20()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            QuickSorter<User>.QuickSort(copy, User.ComparerByName);
        }
        
        [Benchmark(Description = "QuickSorter<T>.Sort:2Threads")]
        public void QuickSort_NET20_2Threads()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 2);
        }
        
        [Benchmark(Description = "QuickSorter<T>.Sort:4Threads")]
        public void QuickSort_NET20_4Threads()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 4);
        }
        
        [Benchmark(Description = "Array<T>.Sort")]
        public void Array_Sort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            Array.Sort(copy, User.ComparerByName);
        }
    }
}