using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Subsequence
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph(8);
            graph.AddNode(0, 5);
            graph.AddNode(1, 2);
            graph.AddNode(2, 8);
            graph.AddNode(3, 6);
            graph.AddNode(4, 3);
            graph.AddNode(5, 6);
            graph.AddNode(6, 9);
            graph.AddNode(7, 7);


            int longest = graph.LongestsSubsequence();
            Console.WriteLine("Longest = " + longest);
        }
    }

    class Node
    {
        private int value;

        public Node(int value)
        {
            this.value = value;
        }
    }

    class Graph
    {

        private int numNodes;
        private Node[] adjList;
        const int infinity = 10000;

        public Graph(int numNodes)
        {
            this.numNodes = numNodes;
            adjList = new Node[numNodes];
            //for (int j = 0; j < numNodes; j++)
            //{
            //    adjList[int] = new Node();
            //}
        }

        public void AddNode(int nodeNum, int value)
        {
            adjList[nodeNum] = new Node(value);
        }

        public int LongestsSubsequence()
        {
            int[] longest = new int[numNodes];
            longest[0] = 1;

/*            for (int j = 1; j < numNodes; j++)
            {
                longest[j] = 1 + 
            }
            */
            return 0;
        }

    }
}
