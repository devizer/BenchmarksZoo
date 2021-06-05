using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace Benchmarks7
{
    [SimpleJob(RuntimeMoniker.Mono)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [RankColumn]
    public class StringComparerBenchmark
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                AddJob(Job.Dry);
            }
        }

        [Params("Jobs", "jOBS", "4242")]
        public string Arg;

        [Benchmark]
        public bool Fastest()
        {
            return IsJobs(Arg);
        }

        [Benchmark]
        public bool Ordinal()
        {
            return Arg.StartsWith("Jobs", StringComparison.Ordinal);
        }

        [Benchmark]
        public bool OrdinalIgnoreCase()
        {
            return Arg.StartsWith("Jobs", StringComparison.OrdinalIgnoreCase);
        }

        [Benchmark]
        public bool CurrentCulture()
        {
            return Arg.StartsWith("Jobs", StringComparison.CurrentCulture);
        }

        [Benchmark]
        public bool CurrentIgnoreCase()
        {
            return Arg.StartsWith("Jobs", StringComparison.CurrentCultureIgnoreCase);
        }

        [Benchmark]
        public bool InvariantCulture()
        {
            return Arg.StartsWith("Jobs", StringComparison.InvariantCulture);
        }

        [Benchmark]
        public bool InvariantIgnoreCase()
        {
            return Arg.StartsWith("Jobs", StringComparison.InvariantCultureIgnoreCase);
        }

        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsJobs(string arg)
        {
            char c;
            return arg.Length >= 4
                   && ((c = arg[0]) == 'J' || c == 'j')
                   && ((c = arg[1]) == 'O' || c == 'o')
                   && ((c = arg[2]) == 'B' || c == 'b')
                   && ((c = arg[3]) == 'S' || c == 's');
        }
    }
}