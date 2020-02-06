
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BenchmarksZoo.ClassicAlgorithms
{
    public class QuickSorter<T>
    {

        public static int InsertionSortingThreshold { get; set; } = 16;        
        
        public static void QuickSort(T[] keys, IComparer<T> comparer)
        {
            if (keys.Length <= 1) return;
            QuickSort(keys, 0, keys.Length - 1, comparer);
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
                    {
                        if (nextRight - left > InsertionSortingThreshold)
                            QuickSort(keys, left, nextRight, comparer);
                        else
                            InsertionSort(keys, left, nextRight - left + 1, comparer);
                    }

                    left = nextLeft;
                }
                else
                {
                    if (nextLeft < right)
                    {
                        if (right - nextLeft > InsertionSortingThreshold)
                            QuickSort(keys, nextLeft, right, comparer);
                        else
                            InsertionSort(keys, nextLeft, right - nextLeft + 1, comparer);
                    }

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
        
        public static void InsertionSort(T[] a, int l, int size, IComparer<T> comparer = null)
        {
            int r = l + size;
            for (int i = l + 1; i < r; i++)
            {
                //if (a[i] < a[i - 1])        // no need to do (j > 0) compare for the first iteration
                if (comparer.Compare(a[i], a[i - 1]) < 0)
                {
                    T currentElement = a[i];
                    a[i] = a[i - 1];
                    int j = i - 1;
                    for (; j > l && comparer.Compare(currentElement, a[j - 1]) < 0; j--)
                    {
                        a[j] = a[j - 1];
                    }
                    a[j] = currentElement;  // always necessary work/write
                }
                // Perform no work at all if the first comparison fails - i.e. never assign an element to itself!
            }
        }



        
    }
}