using System;
using System.IO;
using System.IO.Compression;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    public class CompressionBenchmark
    {
        
        byte[] CompressedFile = null;

        [GlobalSetup]
        public void GlobalSetup()
        {
            using (FileStream fs = new FileStream("libcoreclr.so.gz", FileMode.Open, FileAccess.Read, FileShare.Read))
            using (MemoryStream copy = new MemoryStream())
            {
                fs.CopyTo(copy);
                CompressedFile = copy.ToArray();
            }
        }

        [Benchmark(Baseline = true, Description = "System.GZipStream")]
        public void System_Compression()
        {
            using(MemoryStream mem = new MemoryStream(CompressedFile))
            using (System.IO.Compression.GZipStream gzip = new GZipStream(mem, CompressionMode.Decompress))
            {
                gzip.CopyTo(Stream.Null);
            }
        }

        [Benchmark(Description = "CSharp.GZipStream")]
        public void Managed_GZip()
        {
            using(MemoryStream mem = new MemoryStream(CompressedFile))
            using (Universe.TinyGZip.GZipStream gzip = new Universe.TinyGZip.GZipStream(mem, Universe.TinyGZip.CompressionMode.Decompress))
            {
                gzip.CopyTo(Stream.Null);
            }
        }
   }

    [RankColumn]
    [MemoryDiagnoser]
    public class PiBenchmark
    {

        [Benchmark(Baseline = true, Description = "PI")]
        public void CalcPi()
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
        }
    }

}