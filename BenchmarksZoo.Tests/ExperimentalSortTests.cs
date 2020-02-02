using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using BenchmarksZoo;
using BenchmarksZoo.ClassicAlgorithms;
using NUnit.Framework;
using Tests;
using Universe;
using Universe.CpuUsage;

namespace BenchmarksZoo.Tests
{
    public class ExperimentalSortTests : NUnitTestsBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            // ThreadPool.SetMaxThreads(64, 1024);
            // ThreadPool.SetMinThreads(32, 1024);
            
            var original = User.Generate(666);
            ExperimentalQuickSorter<User>.QuickSort(original, User.ComparerByName, concurrencyLimit: 2);
            ThreadPoolHeating.HeatThreadPool(16);

            // Smoke_Experimental_Sorting_Test(1).Wait();
            // Smoke_Experimental_Sorting_Test(3).Wait();
            
            
        }


        [Test]
        
        /*
        [TestCase(14)]
        [TestCase(13)]
        [TestCase(12)]
        [TestCase(11)]
        [TestCase(10)]
        [TestCase(9)]
        */
        [TestCase(10)]
        [TestCase(9)]
        [TestCase(8)]
        [TestCase(7)]
        [TestCase(6)]
        [TestCase(5)]
        [TestCase(4)]
        [TestCase(3)]
        [TestCase(2)]
        [TestCase(1)]
        public async Task Smoke_Experimental_Sorting_Test(int maxThreads)
        {

            var count = 1000 * 1000;
            User[] original = User.Generate(count);
            User[] expected = original.OrderBy(x => x.Name).ToArray();
            ThreadPool.GetAvailableThreads(out var workersCount, out var waitersCount);
            Console.WriteLine($"Arg threads: {maxThreads}. Actual workers: {workersCount}. Actual Waiters: {waitersCount}");

            CpuUsageAsyncWatcher watcher = new CpuUsageAsyncWatcher();
            Stopwatch sw = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                ExperimentalQuickSorter<User>.QuickSort(original, User.ComparerByName, concurrencyLimit: maxThreads);
            });
            var totals = watcher.Totals;
            var duration = sw.ElapsedMilliseconds;

            CollectionAssert.AreEqual(expected, original, $"Improper experimental sorting of array sized {count}");
            var msg = totals.ToHumanString(taskDescription: $"Parallel Sorting using {maxThreads} threads ({duration} msec)");
            Console.WriteLine(Environment.NewLine + msg);
        }
    }
}