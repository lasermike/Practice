using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace median
{
    class Program
    {
        static void Write(int[] data, int start = 0, int end = -1)
        {
            if (end == -1)
                end = data.Length - 1;

            for (int i = start; i <= end; i++)
                Console.Write(data[i] + ", ");
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            //int[] data = { 9, 3, 5,  7, 8, 1 };
            //int[] data = { 3, 8, 16, 14, 12, 2, 3, 0, 5, 2 };

            Random rand = new Random(DateTime.Now.Second);

            for (int x = 0; x < 5000; x++)
            {
                int[] data = new int[10];
                for (int i = 0; i < data.Length; i++)
                    data[i] = rand.Next(data.Length * 2);

                Write(data);
                int median = FindMedian(data, rand);

                Write(data);
                Console.WriteLine("Median: " + median);

                // Verify answer
                int[] sorted = data.OrderBy(s => s).ToArray();
                int test = sorted[(int)Math.Round(sorted.Length / 2.0) - 1];
                Console.WriteLine("Test = " + test);
                if (test != median)
                {
                    Debugger.Break();
                }
                Write(sorted);
            }                    

            Console.ReadKey();
        }

        static void Swap(int[] data, int i, int j)
        {
            int temp = data[i];
            data[i] = data[j];
            data[j] = temp;
        }

        static int FindMedian(int[] data, Random rand)
        {
            int start = 0;
            int end = data.Length - 1;
            int mid = (int) Math.Round(data.Length / 2.0) - 1; 
            while (true)
            {
                Console.WriteLine("--Iteration--");
                int randomPivot = start + rand.Next(end - start);
                // Partition at a random place in the array
                int pivotIndex = Partition(data, start, end, randomPivot);
                
                if (pivotIndex == mid)
                {
                    break;
                }
                else if (pivotIndex < mid)
                {
                    start = pivotIndex + 1;
                }
                else
                {
                    end = pivotIndex - 1;
                }
            }

            return data[mid];

        }

        static int Partition(int[] data, int start, int end, int pivotIndex)
        {
            int pivot = data[pivotIndex];
            Console.WriteLine("Pivot: " + pivot);

            // Put the pivot value out of the way at the end of the array
            Swap(data, pivotIndex, end);

            int store = start;
            for (int i = start; i < end; i++)
            {
                // If the current value is less than the pivot, store it at the front of the array
                if (data[i] < pivot)
                {
                    Swap(data, i, store);
                    store++;
                }
                Write(data, start, end);
            }
            Swap(data, end, store);

            Console.WriteLine("store = " + store);
            Write(data, start, end);

            return store;
        }
    }
}
