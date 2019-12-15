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

            Job jobCore22 = run.With(CoreRuntime.Core22).WithId("NET-CORE 2.2").WithWarmupCount(3);
            Job jobCore30 = run.With(CoreRuntime.Core30).WithId("NET-CORE 3.0").WithWarmupCount(3);
            Job jobCore31 = run.With(CoreRuntime.Core31).WithId("NET-CORE 3.1").WithWarmupCount(3);
            config = config.With(new[] { jobCore22, jobCore30, jobCore31});
            
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