using System.Linq;
using BenchmarksZoo;
using BenchmarksZoo.ClassicAlgorithms;
using NUnit.Framework;

namespace BenchmarksZoo.Tests
{
    public class ClassicSortTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SmokeTest()
        {
            for (int count = 0; count < 666; count++)
            {
                var original = User.Generate(count);
                var expected = original.OrderBy(x => x.Name).ToArray();
                QuickSorter<User>.QuickSort(original, User.ComparerByName);
                CollectionAssert.AreEqual(expected, original);
            }
        }
    }
}