using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarksZoo.ClassicAlgorithms;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class ShaBenchmark
    {
        private byte[] Content;

        private SHA256 SHA256 = SHA256.Create();
        CSharpSHA256 CSharpSHA256_Unsafe = new CSharpSHA256(true); 
        CSharpSHA256 CSharpSHA256_Slow = new CSharpSHA256(false); 

        [GlobalSetup]
        public void GlobalSetup()
        {
            Content = new byte[64000];
            new Random(42).NextBytes(Content);
        }

        [Benchmark(Description = "System.SHA256")]
        public int System_SHA256()
        {
            return SHA256.ComputeHash(Content).Length;
        }

        [Benchmark(Description = "CSharp.SHA256:Unsafe")]
        public int CSharp_SHA256_Unsafe()
        {
            return CSharpSHA256_Unsafe.ComputeHash(Content).Length;
        }

        [Benchmark(Description = "CSharp.SHA256:Slow")]
        public int CSharp_SHA256_Slow()
        {
            return CSharpSHA256_Slow.ComputeHash(Content).Length;
        }
        
    }

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