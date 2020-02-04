
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace BenchmarksZoo.ClassicAlgorithms
{
    public class ExperimentalQuickSorter<T>
    {
        private static readonly int[] PowersOfTwo = new[] {2, 4, 8, 16, 32, 64, 128, 256, 512};

        public static SortingDebugMetricsInfo LastSortingMetrics;
        
        
        public static unsafe void QuickSort(T[] keys, IComparer<T> comparer, int? concurrencyLimit = null)
        {
            if (!concurrencyLimit.HasValue) concurrencyLimit = Environment.ProcessorCount;
            if (keys.Length <= 1) return;
            var numThreads = concurrencyLimit.Value;
            int step = keys.Length / numThreads + (keys.Length % numThreads == 0 ? 0 : 1);
            if (step < 10 || concurrencyLimit == 1)
            {
                QuickSorter<T>.QuickSort(keys, 0, keys.Length - 1, comparer);
                return;
            }

            int depth = Array.IndexOf(PowersOfTwo, concurrencyLimit.Value);
            if (depth >= 0 && false)
            {
                depth++;
                ParallelSort(keys, 0, keys.Length - 1, comparer, depth);
                return;
            }
            
            

#if DEBUG
            Console.WriteLine($"Parallel sorting for array length: {keys.Length}");
#endif

            SortingDebugMetricsInfo metrics = new SortingDebugMetricsInfo();
            var sw = Stopwatch.StartNew();
#if true
            SortingPortion* portions = stackalloc SortingPortion[numThreads];
#else 
            // NET Core
            Span<SortingPortion> portions = stackalloc SortingPortion[numThreads];
#endif

            CountdownEvent done = new CountdownEvent(numThreads);
            long[] durationsByThreads = new long[numThreads];
            int left = 0;
            for (int t = 0; t < numThreads; t++)
            {
                var tCopy = t;
                SortingPortion portion = new SortingPortion()
                {
                    Left = left,
                    Right = Math.Min(left + step - 1, keys.Length - 1)
                };
                portions[t] = portion;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Stopwatch swThread = Stopwatch.StartNew();
                    QuickSorter<T>.QuickSort(keys, portion.Left, portion.Right, comparer);
                    done.Signal();
                    durationsByThreads[tCopy] = swThread.ElapsedMilliseconds;
                });

                left += step;

            }
            done.Wait();
            long msecPartialSorting = sw.ElapsedMilliseconds;

            // Merging numThreads portions
            sw = Stopwatch.StartNew();
            var copy = new T[keys.Length];
            MergePortions(keys, copy, comparer, numThreads, portions);
            long msecMerge = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            for (int i = 0; i < keys.Length; i++) keys[i] = copy[i];
            long msecCopy = sw.ElapsedMilliseconds;

            LastSortingMetrics = new SortingDebugMetricsInfo()
            {
                CopyDuration = msecCopy,
                PartialSortDuration = msecPartialSorting,
                MergeDuration = msecMerge,
                DurationsByThreads = durationsByThreads.ToList()
            };
        }

        private static unsafe void MergePortions(T[] items, T[] copy, IComparer<T> comparer, int numThreads, SortingPortion* portions)
        {
            if (numThreads == 2)
            {
                Merge_Two_Portions(items, copy, comparer, numThreads, portions);
                return;
            }
            
            var itemsCount = items.Length;
            int pos = 0;
            while (pos < itemsCount)
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
                            bool isBefore = comparer.Compare(items[portions[p].Left], items[portions[minPortion.Value].Left]) <= 0;
                            if (isBefore) minPortion = p;
                        }
                    }
                }

                if (minPortion == null)
                {
                    // crash
                    SortingPortion[] portionsCopy = new SortingPortion[numThreads];
                    for (int ix = 0; ix < numThreads; ix++) portionsCopy[ix] = portions[ix];
                    var info = string.Join(Environment.NewLine, portionsCopy.Select((x, i) => $"    {i,-3}:  {x.Left,-5} ... {x.Right,-5}"));
                    throw new InvalidOperationException($"Welcome the a hell. Array length is {itemsCount}. Pos: {pos}{Environment.NewLine}{info}");
                }

                {
                    var index = portions[minPortion.Value].Left;
                    portions[minPortion.Value].Left = index + 1;
                    copy[pos] = items[index];
                }

                pos++;
            }
        }
        
        private static unsafe void Merge_Two_Portions(T[] items, T[] copy, IComparer<T> comparer, int numThreads, SortingPortion* portions)
        {
            int left0 = portions[0].Left, right0 = portions[0].Right;
            int left1 = portions[1].Left, right1 = portions[1].Right;
            T item0 = items[left0], item1 = items[left1];
            var itemsCount = items.Length;
            int pos = 0;
            while (pos < itemsCount)
            {
                bool has0 = left0 <= right0;
                bool has1 = left1 <= right1;
                if (has0 && has1)
                {
                    bool isLeft0First = comparer.Compare(item0, item1) <= 0;
                    if (isLeft0First)
                    {
                        copy[pos++] = item0;
                        left0++;
                        if (left0 <= right0) item0 = items[left0];
                    }
                    else
                    {
                        copy[pos++] = item1;
                        left1++;
                        if (left1 <= right1) item1 = items[left1];
                    }
                }
                else
                {
                    if (has0) copy[pos++] = items[left0++]; 
                    else if (has1) copy[pos++] = items[left1++];
                    else
                    {
                        // crash
                        SortingPortion[] portionsCopy = new SortingPortion[numThreads];
                        for (int ix = 0; ix < numThreads; ix++) portionsCopy[ix] = portions[ix];
                        portions[0].Left = left0;
                        portions[1].Left = left1;
                        var info = string.Join(Environment.NewLine, portionsCopy.Select((x, i) => $"    {i,-3}:  {x.Left,-5} ... {x.Right,-5}"));
                        throw new InvalidOperationException($"Welcome the a hell. Array length is {itemsCount}. Pos: {pos}{Environment.NewLine}{info}");
                    }
                }
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Swap (T [] array, int i, int j)
        {
            T tmp = array [i];
            array [i] = array [j];
            array [j] = tmp;
        }
        
        private static void ParallelSort (T [] array, int low0, int high0, IComparer<T> comparer, int depth)
        {
            if (low0 >= high0)
                return;

            int low = low0;
            int high = high0;

            T keyPivot = array [(low + high) / 2];

            while (low <= high) {
                // Move the walls in
                while (low < high0 && comparer.Compare(array [low], keyPivot) < 0)
                    ++low;
                while (high > low0 && comparer.Compare (keyPivot, array [high]) < 0)
                    --high;

                if (low <= high) {
                    Swap (array, low, high);
                    ++low;
                    --high;
                }
            }

            if (depth > 0 && (low0 < high) && (low < high0))
            {
                // Parallel.Invoke is slower
                CountdownEvent done = new CountdownEvent(2);
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    ParallelSort(array, low0, high, comparer, depth - 1);
                    done.Signal();
                });
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    ParallelSort(array, low, high0, comparer, depth - 1);
                    done.Signal();
                });
                done.Wait();
            }
            else
            {
                if (low0 < high)
                    ParallelSort(array, low0, high, comparer, depth - 1);
                if (low < high0)
                    ParallelSort(array, low, high0, comparer, depth - 1);
            }
        }



        
    }
    
    
    
    struct SortingPortion
    {
        public int Left;
        public int Right;
    }

    public class SortingDebugMetricsInfo
    {
        public List<long> DurationsByThreads = new List<long>();
        public long PartialSortDuration;
        public long MergeDuration;
        public long CopyDuration;

        public override string ToString()
        {
            return $"SortDuration: {PartialSortDuration}, Merge: {MergeDuration}, Copy: {CopyDuration}, Duration by threads: [{string.Join(",", DurationsByThreads)}]";
        }
    }

}