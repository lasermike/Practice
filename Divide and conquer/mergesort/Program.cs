using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mergesort
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] unsorted = new int[] { 3, 2, 5, 56, 4, 3, 4, 5, 6, 7, 78, 9, 7, 1 };
            int[] sorted = MergeSort(unsorted);
            foreach (int i in sorted)
                Console.Write( i + ", " );
            Console.ReadKey();
        }

        static public int[] MergeSort(int[] unsorted)
        {
            int[] sorted = new int[unsorted.Length];
            return _MergeSort(unsorted, 0, unsorted.Length - 1, sorted);
        }

        static private int[] _MergeSort(int[] unsorted, int start, int end, int[] sorted)
        {
            if ( ((end - start) + 1) > 1)
            {
                int half = start + (int)Math.Floor((double) ((end - start) / 2));
                return _Merge( 
                    _MergeSort(unsorted, start, half, sorted),
                    _MergeSort(unsorted, half + 1, end, sorted));
            }
            else
            { 
                return new int[1] { unsorted[start] } ;
            }
        }

        static private int[] _Merge(int[] left, int[] right)
        {
            if (left.Length < 1)
                return right;
            if (right.Length < 1)
                return left;

            if (left[0] <= right[0])
            {
                return (new int[] { left[0] }).Concat(_Merge( left.Skip(1).ToArray(), right)).ToArray();
            }
            else
            {
                return (new int[] { right[0] }).Concat(_Merge(left, right.Skip(1).ToArray())).ToArray();
            }
        }
    }
}
