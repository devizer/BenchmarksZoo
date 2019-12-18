using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System.Linq;
using BenchmarkDotNet.Exporters.Json;
using BenchmarksShared;
using Universe;

namespace BenchmarksZoo
{
    public static class BenchmarkRunnerProgram
    {
        private static bool IsMedium;
        private static bool IsShort;
        private static bool IsDry;
        private static bool NeedNetCore;

        static void Main(string[] args)
        {
            Func<string, bool> hasArgument = (name) => args.Any(x => x.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) >= 0);
            if (hasArgument("help"))
            {
                Console.WriteLine($"MonoFeaturesChecker.IsMonoSupported:        {MonoFeaturesChecker.IsMonoSupported(null)}");
                Console.WriteLine($"MonoFeaturesChecker.IsLlvmForMonoSupported: {MonoFeaturesChecker.IsLlvmForMonoSupported(null)}");
                LoadedAssemblies.ShowManagedLibraries();
                LoadedAssemblies.ShowNativeLibraries();
                return;
            }

            NeedNetCore = !hasArgument("skip-net-core");

            IsMedium = hasArgument("medium");
            IsShort = hasArgument("short");
            IsDry = hasArgument("dry");

            var run = Job.MediumRun;
            if (IsDry) run = Job.Dry;
            if (IsShort) run = Job.ShortRun;

            IConfig config = ManualConfig.Create(DefaultConfig.Instance);
            // Job jobLlvm = Job.InProcess;

            MonoRuntime monoRuntime = MonoRuntime.Default;

            bool isWindows = CrossInfo.ThePlatform == CrossInfo.Platform.Windows;

            if (IsMono())
            {
                config = config.With(new[] {run.With(monoRuntime).WithId("Llvm-OFF").ConfigWarmUp()});
                config = config.With(new[] {run.With(Jit.Llvm).With(monoRuntime).WithId("Llvm-ON").ConfigWarmUp()});
            }
                
            if (NeedNetCore)
            {
                Job jobCore22 = run.With(Jit.RyuJit).With(CoreRuntime.Core22).WithId($"Net Core 2.2").ConfigWarmUp();
                Job jobCore30 = run.With(Jit.RyuJit).With(CoreRuntime.Core30).WithId($"Net Core 3.0").ConfigWarmUp();
                Job jobCore31 = run.With(Jit.RyuJit).With(CoreRuntime.Core31).WithId($"Net Core 3.1").ConfigWarmUp();
                config = config.With(new[] {jobCore22, jobCore30, jobCore31});
            }

            if (CrossInfo.ThePlatform == CrossInfo.Platform.Windows)
                config = config.With(run.With(ClrRuntime.Net47).WithId("NETFW-47").ConfigWarmUp());

            config = config.With(JsonExporter.Custom(fileNameSuffix: "-full", indentJson: true, excludeMeasurements: false));
            config = config.With(JsonExporter.Custom(fileNameSuffix: "-brief", indentJson: true, excludeMeasurements: true));

            // var summary = BenchmarkRunner.Run(typeof(BenchmarkRunnerProgram).Assembly, config);
            var summary = BenchmarkRunner.Run(typeof(PiBenchmark), config);
        }

        static bool IsMono()
        {
            return Type.GetType("Mono.Runtime", false) != null;
        }

        public static Job ConfigWarmUp(this Job job)
        {
            if (IsShort)
                job = job.WithWarmupCount(1).WithLaunchCount(1).WithUnrollFactor(2);

            return job;
        }
    }
}