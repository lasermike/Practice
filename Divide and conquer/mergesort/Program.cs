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
            int[] data = new int[] { 6, 5, 4, 3, 2, 1 };
            //int[] data = new int[] { 3, 2, 5, 56, 4, 3, 4, 5, 6, 7, 78, 9, 7, 1 };

            foreach (int i in data)
                Console.Write(i + ", ");
            Console.WriteLine("");

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
            //Bound bounds = sorter.MergeSortRecursive(new Bound(0, data.Length - 1));
            Bound bounds = sorter.MergeSortIterative(new Bound(0, data.Length - 1));
            Debug.Assert(bounds.Item1 == 0 && bounds.Item2 == data.Length - 1);
            return data; 
        }

        private Sorter(int[] data)
        {
            this.data = data;
        }

        private Bound MergeSortIterative(Bound bound)
        {
            Queue<Bound> queue = new Queue<Bound>();
            for (int i = 0; i < data.Length; i++)
            {
                queue.Enqueue(new Bound(i, i));
            }

            while (queue.Count > 1)
            {
                Bound a = queue.Dequeue();

                if (a.Item2 == bound.Item2)
                {
                    queue.Enqueue(a);
                    continue;
                }

                Bound b = queue.Dequeue();

                //if (a.Item1 > b.Item1)
                //    Debugger.Break();

                //if (a.Item2 + 1 != b.Item1)
                //    Debugger.Break();

                MergeIterative(a, b);
                queue.Enqueue( new Bound(a.Item1, b.Item2));
            }
            return queue.Dequeue();
        }

        private void MergeIterative(Bound left, Bound right)
        {
            Bound a = new Bound(left.Item1, left.Item2);
            Bound b = new Bound(right.Item1, right.Item2);

            while (a.Item1 < b.Item1 && b.Item1 <= b.Item2)
            {
                if (data[a.Item1] <= data[b.Item1])
                {
                    // Just advance the left array start and continue
                    a = new Bound(a.Item1 + 1, a.Item2);
                    b = new Bound(b.Item1, b.Item2);
                }
                else
                {
                    // We will take from the right side array, but we need to shift the left array's bounds over since we are not taking from that side.
                    // Cache the item we want
                    int temp = data[b.Item1];

                    // Shift everything to the right 
                    for (int i = b.Item1; i > a.Item1; i--)
                        data[i] = data[i - 1];

                    // Store item we want
                    data[a.Item1] = temp;

                    a = new Bound(a.Item1 + 1, a.Item2 + 1);
                    b = new Bound(b.Item1 + 1, b.Item2);
                }
            }
        }

        private Bound MergeSortRecursive(Bound bound)
        {
            if (((bound.Item2 - bound.Item1) + 1) > 1)
            {
                int half = bound.Item1 + (int)Math.Floor((double)((bound.Item2 - bound.Item1) / 2));

                Bound a = MergeSortRecursive( new Bound(bound.Item1, half) );
                Bound b = MergeSortRecursive( new Bound(half + 1, bound.Item2) );
                MergeRecursive(a, b);
                return bound;
            }
            else
            { 
                return new Bound(bound.Item1, bound.Item2);
            }
        }

        private void MergeRecursive(Bound left, Bound right)
        {
            if (left.Item2 - left.Item1 < 0 || right.Item2 - right.Item1 < 0)
                return ;

            if (data[left.Item1] <= data[right.Item1])
            {
                // Just advance the left array start and continue
                MergeRecursive(new Bound(left.Item1 + 1, left.Item2), right);
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

                MergeRecursive(new Bound(left.Item1 + 1, left.Item2 + 1), new Bound(right.Item1 + 1, right.Item2));
            }
        }
    }
}
