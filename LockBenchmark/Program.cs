using System;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace LockBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.FirstOrDefault()?.ToLower()?.StartsWith("--print") == true)
            {
                Console.WriteLine(Benchmarks.GetOsName().FirstOrDefault());
                return;
            }

            Console.WriteLine($"OS: {Benchmarks.GetOsName().FirstOrDefault()}");
            var config = DefaultConfig.Instance;
            var summary = BenchmarkRunner.Run<Benchmarks>(config, args);


            // Use this to select benchmarks from the console:
            // var summaries = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}