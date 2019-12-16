using System;
using System.Diagnostics;

namespace BenchmarksZoo
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

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
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
            
            Console.WriteLine($"NATIVE LIBRARIES:");
            Process p = Process.Start("sudo", $"lsof -p {Process.GetCurrentProcess().Id}");
            p.WaitForExit();
            Console.WriteLine();
        }



    }
}