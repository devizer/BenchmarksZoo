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

            Func<string,bool> hasArgument = (name) => args.Any(x => x.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0);
            if (hasArgument("help"))
            {
                LoadedAssemblies.ShowManagedLibraries();
                LoadedAssemblies.ShowNativeLibraries();
                return;
            }
            
            IsRelease = hasArgument("release");
            var run = IsRelease ? Job.MediumRun : Job.ShortRun;
            IConfig config = ManualConfig.Create(DefaultConfig.Instance);
            // Job jobLlvm = Job.InProcess;

            if (IsMono())
            {
                Job jobLlvm = run.With(Jit.Llvm).With(MonoRuntime.Default).WithId("Llvm-ON").ConfigWarmUp();
                Job jobNoLlvm = run.With(MonoRuntime.Default).WithId("Llvm-OFF").ConfigWarmUp();
                config = config.With(new[] { jobLlvm, jobNoLlvm});
            }

            foreach (var jit in new[] {/*Jit.LegacyJit, */Jit.RyuJit})
            {
                Job jobCore22 = run.With(CoreRuntime.Core22).With(Jit.Default).WithId($"Net Core 2.2 {jit}").ConfigWarmUp();
                Job jobCore30 = run.With(CoreRuntime.Core30).With(Jit.Default).WithId($"Net Core 3.0 {jit}").ConfigWarmUp();
                Job jobCore31 = run.With(CoreRuntime.Core31).With(Jit.Default).WithId($"Net Core 3.1 {jit}").ConfigWarmUp();
                /*
                if (jit == Jit.RyuJit)
                {
                    jobCore22 = jobCore22.With(jit);
                    jobCore30 = jobCore30.With(jit);
                    jobCore31 = jobCore31.With(jit);
                }
                */

                config = config.With(new[] { jobCore22, jobCore30, jobCore31});
            }
            
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                config = config.With(run.With(ClrRuntime.Net47).WithId("NETFW-47").ConfigWarmUp());
                
            // var summary = BenchmarkRunner.Run(typeof(BenchmarkRunnerProgram).Assembly, config);
            var summary = BenchmarkRunner.Run(typeof(PiBenchmark), config);
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