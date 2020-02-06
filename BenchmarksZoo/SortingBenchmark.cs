using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarksZoo.ClassicAlgorithms;
using HPCsharp;
using HPCsharpExperimental;
using Universe;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    /*[NativeMemoryProfiler]*/
    public class SortingBenchmark
    {
        [Params(1000*1000/*, 543*/)]
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
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 1, true);
        }
        
        [Benchmark(Description = "QuickSorter<T>.Sort:2Threads")]
        public void QuickSort_NET20_2Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 2, true);
        }
        
        [Benchmark(Description = "QuickSorter<T>.Sort:3Threads")]
        public void QuickSort_NET20_3Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 3, true);
        }
        
        [Benchmark(Description = "QuickSorter<T>.Sort:4Threads")]
        public void QuickSort_NET20_4Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 4, true);
        }
        
        [Benchmark(Description = "Array.Sort<T>")]
        public void Array_Sort()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            Array.Sort(copy, User.ComparerByName);
        }
        
        [Benchmark(Description = "Array.Sort<T>:2Threads")]
        public void ArraySort_2Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 2, false);
        }
        
        [Benchmark(Description = "Array.Sort<T>:3Threads")]
        public void ArraySort_3Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 3, false);
        }
        
        [Benchmark(Description = "Array.Sort<T>:4Threads")]
        public void ArraySort_4Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, 4, false);
        }
        
        [Benchmark(Description = "Array.Sort<T>:MaxThreads")]
        public void ArraySort_8Threads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            ExperimentalQuickSorter<User>.QuickSort(copy, User.ComparerByName, Environment.ProcessorCount, false);
        }
        
        [Benchmark(Description = "HpcMergeSort<T>:MaxThreads")]
        public void HpcSort_AllTheThreads()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
            User[] sorted = copy.SortMergePar(User.ComparerByName);
        }

        [Benchmark(Description = "NoSorting")]
        public void JustCopy()
        {
            User[] copy = new User[Users.Length];
            var usersLength = Users.Length;
            for (int i = 0, l=usersLength; i < l; i++) copy[i] = Users[i];
        }


    }
}