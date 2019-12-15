using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace BenchmarksZoo
{
    class Program
    {
        static void Main(string[] args)
        {
            var run = Job.ShortRun;
            IConfig config = ManualConfig.Create(DefaultConfig.Instance);
            // Job jobLlvm = Job.InProcess;

            if (IsMono())
            {
                Job jobLlvm = run.With(Jit.Llvm).With(MonoRuntime.Default).WithId("LLVM-ON").WithWarmupCount(3);
                Job jobNoLlvm = run.With(MonoRuntime.Default).WithId("LLVM-OFF").WithWarmupCount(3);
                config = config.With(new[] { jobLlvm, jobNoLlvm});
            }

            Job jobCore = run.With(CoreRuntime.Core22).WithId("NET-CORE").WithWarmupCount(3);
            config = config.With(jobCore);
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                config = config.With(run.With(ClrRuntime.Net47).WithId("NETFW-47").WithWarmupCount(3));
                
            // Job jobFW47 = run.With(ClrRuntime.Net47).WithId("NETFW-47").WithWarmupCount(3);
            Summary summary = BenchmarkRunner.Run<SortingBenchmark>(config);
        }
        
        static bool IsMono()
        {
            return Type.GetType("Mono.Runtime", false) != null;
        }

    }
}