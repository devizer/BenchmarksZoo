using System;
using System.IO;
using System.IO.Compression;
using BenchmarkDotNet.Attributes;

namespace BenchmarksZoo
{
    [RankColumn]
    [MemoryDiagnoser]
    /*[NativeMemoryProfiler]*/
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
}