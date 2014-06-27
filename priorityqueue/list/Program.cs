using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace List
{
    class Program
    {
        static void Main(string[] args)
        {
            PriorityQueueHeap queue = new PriorityQueueHeap();
            var random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int ranNum = random.Next();
                queue.Add(ranNum, ranNum);
            }

            int prev = Int32.MaxValue;
            Node node = queue.Pop();
            while (node != null)
            {
                if (node.Priority > prev)
                {
                    Console.WriteLine("ERROR: heap not sorted: node = " + node.Priority + ", next = " + prev);
                }
                Console.WriteLine(node.Value);
                prev = node.Priority;

                node = queue.Pop();
            }
            Console.ReadKey();
        }
    }

    class Node
    {
        public int Priority { get; set; }
        public int Value { get; set; }

    }

}