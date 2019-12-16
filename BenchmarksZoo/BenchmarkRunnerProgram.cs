using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System.Linq;

namespace BenchmarksZoo
{
    public static class BenchmarkRunnerProgram
    {
        private static bool IsRelease;

        static void Main(string[] args)
        {
            IsRelease = args.Any(x => x.IndexOf("release", StringComparison.InvariantCultureIgnoreCase) >= 0);
            
        
            var run = IsRelease ? Job.MediumRun : Job.ShortRun;
            IConfig config = ManualConfig.Create(DefaultConfig.Instance);
            // Job jobLlvm = Job.InProcess;

            if (IsMono())
            {
                Job jobLlvm = run.With(Jit.Llvm).With(MonoRuntime.Default).WithId("LLVM-ON").ConfigWarmUp();
                Job jobNoLlvm = run.With(MonoRuntime.Default).WithId("LLVM-OFF").ConfigWarmUp();
                config = config.With(new[] { jobLlvm, jobNoLlvm});
            }

            Job jobCore22 = run.With(CoreRuntime.Core22).WithId("NET-CORE 2.2").ConfigWarmUp();
            Job jobCore30 = run.With(CoreRuntime.Core30).WithId("NET-CORE 3.0").ConfigWarmUp();
            Job jobCore31 = run.With(CoreRuntime.Core31).WithId("NET-CORE 3.1").ConfigWarmUp();
            config = config.With(new[] { jobCore22, jobCore30, jobCore31});
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                config = config.With(run.With(ClrRuntime.Net47).WithId("NETFW-47").ConfigWarmUp());
                
            var summary = BenchmarkRunner.Run(typeof(BenchmarkRunnerProgram).Assembly, config);
            // var summary = BenchmarkRunner.Run(typeof(CompressionBenchmark), config);
            
        }
        
        static bool IsMono()
        {
            return Type.GetType("Mono.Runtime", false) != null;
        }
        
        public static Job ConfigWarmUp(this Job job)
        {
            if (!IsRelease) job = job.WithWarmupCount(3).WithLaunchCount(3);
            return job;
        }


    }
}