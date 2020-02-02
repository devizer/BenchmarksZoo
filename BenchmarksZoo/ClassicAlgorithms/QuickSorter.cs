
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BenchmarksZoo.ClassicAlgorithms
{
    public class QuickSorter<T>
    {

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
}