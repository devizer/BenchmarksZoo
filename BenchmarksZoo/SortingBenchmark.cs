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

        [Benchmark]
        public void Linq_Sort()
        {
            var sorted = Users.OrderBy(x => x.Name).ToArray();
        }

        [Benchmark]
        public void Classic_QuickSort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            QuickSorter<User>.QuickSort(copy, User.ComparerByName);
        }
        
        [Benchmark]
        public void Array_Sort()
        {
            User[] copy = new User[Users.Length];
            for (int i = 0, l=Users.Length; i < l; i++) copy[i] = Users[i];
            Array.Sort(copy, User.ComparerByName);
        }
        
    }
}