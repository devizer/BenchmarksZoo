using System;
using System.Diagnostics;
using System.Linq;
using BenchmarksZoo;

namespace BenchmarksShared
{
    class LoadedAssemblies
    {
        public static void ShowManagedLibraries()
        {
            {
                CompressionBenchmark b = new CompressionBenchmark();
                b.GlobalSetup();
                b.System_Compression();
                b.Managed_GZip();
            }
            {
                SortingBenchmark b = new SortingBenchmark();
                b.GlobalSetup();
                b.Array_Sort();
                b.Enumerable_OrderBy();
                b.QuickSort_NET20();
            }
            {
                PiBenchmark b = new PiBenchmark();
                b.CalcPi();
            }
            {
                Sha256Benchmark b = new Sha256Benchmark();
                b.GlobalSetup();
                b.System_SHA256();
                b.CSharp_SHA256_Slow();
                b.CSharp_SHA256_Unsafe();
            }
            {
                SynchronizationLatencyBenchmark b = new SynchronizationLatencyBenchmark();
                b.SyncActionLatency_using_Tasks_and_Barrier(1);
                b.AwaitLatency().Wait();
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies = assemblies.OrderBy(x => x.Location).ToArray();
            Console.WriteLine($"TOTAL MANAGED LIBRARIES: {assemblies.Length}");
            foreach (var asm in assemblies)
            {
                Console.WriteLine(asm.Location);
            }
            Console.WriteLine();
        }

        public static void ShowNativeLibraries()
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT) return;
            
            var libs = NativeLibraries.Get();
            libs.Sort();
            Console.WriteLine($"TOTAL NATIVE LIBRARIES: {libs.Count}");
            foreach (var lib in libs)
            {
                Console.WriteLine(lib);
            }
            Console.WriteLine();
        }



    }
}