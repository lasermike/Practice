using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Longest_increasing_subsequence
{
    class Program
    {
        static void Main(string[] args)
        {
            Sequence seq = new Sequence();
            seq.Add( new int[] { 5, 2, 8, 6, 3, 6, 9, 7 } );
            seq.PrintLongestIncreasingSequence();
        }
    }

    class Sequence
    {
        List<int> data;
        public Sequence() { data = new List<int>(); }

        public void Add(int[] newData)
        {
            data.AddRange(newData);
        }

        public void PrintLongestIncreasingSequence()
        {
            int[] array = data.ToArray();
            int[] prev = new int[array.Length];
            int[] longest = new int[array.Length];
            int highest = 0;
            //Dictionary<int, List<int> > adjacencyList = new Dictionary<int,List<int>>(array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                longest[i] = 1;
                prev[i] = -1;
            }

            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (array[j] < array[i])
                    {
                        if (longest[i] < longest[j] + 1)
                        {
                            longest[i] = longest[j] + 1;
                            prev[i] = j;
                            highest = longest[j] > longest[highest] ? j : highest;
                        }
                    }
                }
            }

            List<int> ordered = new List<int>();
            while (highest >= 0)
            {
                ordered.Add(highest);
                highest = prev[highest];
            }

            ordered.Reverse();

            foreach(int i in ordered)
            {
                Console.Write(i + ", ");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
