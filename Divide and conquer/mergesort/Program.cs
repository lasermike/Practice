using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bound = System.Tuple<int, int>;

namespace mergesort
{
    class Program
    {
        static void Main(string[] args)
        {
            //int[] data = new int[] { 4, 3, 2, 1 };
            int[] data = new int[] { 3, 2, 5, 56, 4, 3, 4, 5, 6, 7, 78, 9, 7, 1 };

            int[] sorted = Sorter.MergeSort(data);
            foreach (int i in sorted)
                Console.Write( i + ", " );
            Console.ReadKey();
        }
    }

    class Sorter
    {
        private int[] data;

        static public int[] MergeSort(int[] data)
        {
            Sorter sorter = new Sorter(data);
            Bound bounds = sorter._MergeSort(0, data.Length - 1);
            Debug.Assert(bounds.Item1 == 0 && bounds.Item2 == data.Length - 1);
            return data; 
        }

        private Sorter(int[] data)
        {
            this.data = data;
        }

        private Bound _MergeSort(int start, int end)
        {
            if ( ((end - start) + 1) > 1)
            {
                int half = start + (int)Math.Floor((double) ((end - start) / 2));

                Bound a = _MergeSort(start, half);
                Bound b = _MergeSort(half + 1, end);
                return _Merge(a, b);
            }
            else
            { 
                return new Bound(start, start);
            }
        }

        private Bound _Merge(Bound left, Bound right)
        {
            if (left.Item2 - left.Item1 < 0)
                return right;
            if (right.Item2 - right.Item1 < 0)
                return left;

            if (data[left.Item1] <= data[right.Item1])
            {
                // Just advance the left array start and continue
                _Merge(new Bound(left.Item1 + 1, left.Item2), right);
                return new Bound(left.Item1, right.Item2);
            }
            else
            {
                // We will take from the right side array, but we need to shift the left array's bounds over since we are not taking from that side.
                // Cache the item we want
                int temp = data[right.Item1];

                // Shift everything to the right 
                for (int i = right.Item1; i > left.Item1; i--)
                    data[i] = data[i - 1];

                // Store item we want
                data[left.Item1] = temp;

                _Merge(new Bound(left.Item1 + 1, left.Item2 + 1), new Bound(right.Item1 + 1, right.Item2));
                return new Bound(left.Item1, right.Item2);
            }
        }
    }
}
