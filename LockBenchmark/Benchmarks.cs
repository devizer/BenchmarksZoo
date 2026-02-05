using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using Universe;

namespace LockBenchmark
{
    [MemoryDiagnoser]
    [Config(typeof(Config))]
    [HideColumns(Column.RatioSD)]
    public class Benchmarks
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                SummaryStyle = SummaryStyle.Default
                        .WithMaxParameterColumnWidth(40)
                        .WithRatioStyle(RatioStyle.Percentage)
                    ;
            }
        }

        [ParamsSource(nameof(GetOsName))] public string OS;
        public static IEnumerable<string> GetOsName()
        {
            var net = RuntimeInformation.FrameworkDescription;
            var net2 = net.Split(' ').Last();
            yield return CrossInfo.ThePlatform + " " + RuntimeInformation.OSArchitecture + ", " + net2;
        }

        static readonly object _sync = new();
        private static readonly int _result = 42;

        [Benchmark(Baseline = true)]
        public int ByObject()
        {
            lock (_sync)
            {
                return _result;
            }
        }
        [Benchmark]
        public int NoLock()
        {
            return _result;
        }

        [Benchmark]
        public int ByInterlocked()
        {
            TargetSingleton singleton = null;
            if (singleton == null)
                Interlocked.CompareExchange(ref singleton, new TargetSingleton(), null);

            return _result;
        }

        class TargetSingleton
        {
        }

#if NET9_0_OR_GREATER        
        static readonly Lock _lock = new();
        [Benchmark]
        public int ByLock()
        {
            lock (_lock)
            {
                return _result;
            }
        }

        [Benchmark]
        public int ByUsingLock()
        {
            using(_lock.EnterScope())
            {
                return _result;
            }
        }
#endif
    }
}
