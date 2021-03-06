using System;
using System.Security.Cryptography;
using BenchmarksZoo.ClassicAlgorithms;
using NUnit.Framework;

namespace BenchmarksZoo.Tests
{
    public class ClassicSha256Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SmokeTest()
        {
            foreach (var fast in new[] {true, false})
            {
                foreach (var len in new[] { 0, 1, 2, 3, 4, 5, 1001, 1002, 1003, 42, 77,256*1024})
                {
                    byte[] src = new byte[len];
                    new Random(42).NextBytes(src);
                    CSharpSHA256 sha = new CSharpSHA256(fast);
                    var actual = sha.ComputeHash(src);
                    var expected = SHA256.Create().ComputeHash(src);
                    CollectionAssert.AreEqual(expected, actual, $"Src Size: {len} Unsafe:{fast}");
                }
            }

        }
        
    }
}