
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace BenchmarksZoo.ClassicAlgorithms
{
    public class ExperimentalQuickSorter<T>
    {

        public static void QuickSort(T[] keys, IComparer<T> comparer, int? concurrencyLimit = null)
        {
            if (!concurrencyLimit.HasValue) concurrencyLimit = Environment.ProcessorCount;
            if (keys.Length <= 1) return;
            var numThreads = concurrencyLimit.Value;
            int step = keys.Length / numThreads + (keys.Length % numThreads == 0 ? 0 : 1);
            if (step < 10 || concurrencyLimit == 1)
            {
                QuickSort(keys, 0, keys.Length - 1, comparer);
                return;
            }

#if DEBUG
            Console.WriteLine($"Parallel sorting for array length: {keys.Length}");
#endif

            var sw = Stopwatch.StartNew();
            Span<SortingPortion> portions = stackalloc SortingPortion[numThreads];
            CountdownEvent done = new CountdownEvent(numThreads);
            int left = 0;
            for (int t = 0; t < numThreads; t++)
            {
                SortingPortion portion = new SortingPortion()
                {
                    Left = left,
                    Right = Math.Min(left + step - 1, keys.Length - 1)
                };
                portions[t] = portion;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    QuickSort(keys, portion.Left, portion.Right, comparer);
                    done.Signal();
                });

                left += step;

            }

            done.Wait();
            long msecState1 = sw.ElapsedMilliseconds;
            sw = Stopwatch.StartNew();
            
            // Merging numThreads portions
            T[] copy = new T[keys.Length];
            int pos = 0;
            while(pos < keys.Length)
            {
                int? minPortion = null;
                for (int p = 0; p < numThreads; p++)
                {
                    var portion = portions[p];
                    if (portion.Left <= portion.Right)
                    {
                        if (minPortion == null)
                            minPortion = p;
                        else
                        {
                            bool isBefore = comparer.Compare(keys[portions[p].Left], keys[portions[minPortion.Value].Left]) <= 0;
                            if (isBefore) minPortion = p;
                        }
                    }
                }

                if (minPortion == null)
                {
                    var info = string.Join(Environment.NewLine, portions.ToArray().Select((x,i) => $"    {i,-3}:  {x.Left,-5} ... {x.Right,-5}"));
                    throw new InvalidOperationException($"Welcome the a hell. Array length is {keys.Length}. Pos: {pos}{Environment.NewLine}{info}");
                }

                {
                    var index = portions[minPortion.Value].Left;
                    portions[minPortion.Value].Left = portions[minPortion.Value].Left + 1;
                    copy[pos] = keys[index];
                }

                pos++;
            }

            long msecStage2 = sw.ElapsedMilliseconds;
            sw = Stopwatch.StartNew();

            for (int i = 0; i < keys.Length; i++) keys[i] = copy[i];
            long msecState3 = sw.ElapsedMilliseconds;
            Console.WriteLine($"Sorting {keys.Length} items. [1st]: {msecState1}. [2nd]: {msecStage2}. [3rd]: {msecState3}");
        }

        
        internal static void QuickSort(T[] keys, int left, int right, IComparer<T> comparer)
        {
            do
            {
                int nextLeft = left;
                int nextRight = right;
                int center = nextLeft + (nextRight - nextLeft >> 1);
                SwapIfGreaterWithItems(keys, comparer, nextLeft, center);
                SwapIfGreaterWithItems(keys, comparer, nextLeft, nextRight);
                SwapIfGreaterWithItems(keys, comparer, center, nextRight);
                T keyCenter = keys[center];
                do
                {
                    while (comparer.Compare(keys[nextLeft], keyCenter) < 0)
                        ++nextLeft;
                    
                    while (comparer.Compare(keyCenter, keys[nextRight]) < 0)
                        --nextRight;
                    
                    if (nextLeft <= nextRight)
                    {
                        if (nextLeft < nextRight)
                        {
                            T keyTemp = keys[nextLeft];
                            keys[nextLeft] = keys[nextRight];
                            keys[nextRight] = keyTemp;
                        }
                        ++nextLeft;
                        --nextRight;
                    }
                    else
                        break;
                }
                while (nextLeft <= nextRight);
                
                if (nextRight - left <= right - nextLeft)
                {
                    if (left < nextRight)
                        QuickSort(keys, left, nextRight, comparer);
                    
                    left = nextLeft;
                }
                else
                {
                    if (nextLeft < right)
                        QuickSort(keys, nextLeft, right, comparer);
                    
                    right = nextRight;
                }
            }
            while (left < right);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SwapIfGreaterWithItems(T[] keys, IComparer<T> comparer, int a, int b)
        {
            if (a == b || comparer.Compare(keys[a], keys[b]) <= 0)
                return;
            
            T keyTemp = keys[a];
            keys[a] = keys[b];
            keys[b] = keyTemp;
        }


        
    }
    
    struct SortingPortion
    {
        public int Left;
        public int Right;
            
    }

}