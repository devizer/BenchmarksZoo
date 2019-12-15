using System.Collections.Generic;

namespace BenchmarksZoo.ClassicAlgoruthms
{
    public class QuickSorter<T>
    {

        public static void QuickSort(T[] keys, IComparer<T> comparer)
        {
            QuickSort(keys, comparer);
        }
        
        internal static void QuickSort(T[] keys, int left, int right, IComparer<T> comparer)
        {
            do
            {
                int index1 = left;
                int index2 = right;
                int index3 = index1 + (index2 - index1 >> 1);
                SwapIfGreaterWithItems(keys, comparer, index1, index3);
                SwapIfGreaterWithItems(keys, comparer, index1, index2);
                SwapIfGreaterWithItems(keys, comparer, index3, index2);
                T key1 = keys[index3];
                do
                {
                    while (comparer.Compare(keys[index1], key1) < 0)
                        ++index1;
                    while (comparer.Compare(key1, keys[index2]) < 0)
                        --index2;
                    if (index1 <= index2)
                    {
                        if (index1 < index2)
                        {
                            T key2 = keys[index1];
                            keys[index1] = keys[index2];
                            keys[index2] = key2;
                        }
                        ++index1;
                        --index2;
                    }
                    else
                        break;
                }
                while (index1 <= index2);
                if (index2 - left <= right - index1)
                {
                    if (left < index2)
                        QuickSort(keys, left, index2, comparer);
                    
                    left = index1;
                }
                else
                {
                    if (index1 < right)
                        QuickSort(keys, index1, right, comparer);
                    
                    right = index2;
                }
            }
            while (left < right);
        }
        
        private static void SwapIfGreaterWithItems(T[] keys, IComparer<T> comparer, int a, int b)
        {
            if (a == b || comparer.Compare(keys[a], keys[b]) <= 0)
                return;
            T key = keys[a];
            keys[a] = keys[b];
            keys[b] = key;
        }


        
    }
}