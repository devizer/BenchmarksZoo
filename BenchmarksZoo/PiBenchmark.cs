using System.Runtime.InteropServices.WindowsRuntime;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class PiBenchmark
    {

        [Benchmark(Baseline = true, Description = "PI")]
        public double CalcPi()
        {
            bool positive = false;
            long dev = 3;
            double pi = 4;
            for (int i = 0; i < 4200; i++)
            {
                if (positive) 
                    pi += 4d / dev;
                else 
                    pi -= 4d / dev;

                dev += 2;
                positive = !positive;
            }

            return pi;
        }
    }
}