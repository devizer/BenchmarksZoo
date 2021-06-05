using System.Security.Cryptography;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Benchmarks7
{
    class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<StringComparerBenchmark>();
        }
    }
}