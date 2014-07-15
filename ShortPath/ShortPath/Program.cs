using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortPath
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph(4);

            /*
            graph.AddVertex(0, new int[2] { 1, 2 }, new int[2] { 1, 1} );
            graph.AddVertex(1, new int[2] { 3, 4 }, new int[2] { 1, 1} );
            graph.AddVertex(2, new int[2] { 4, 5 }, new int[2] { 1, 1} );
            graph.AddVertex(3, new int[2] { 6, 7  }, new int[2] { 1, 1 } );
            graph.AddVertex(5, new int[2] { 7, 9 }, new int[2] { 1, 1} );
            graph.AddVertex(7, new int[1] { 9 }, new int[1] { 1} );
             */

            graph.AddVertex(0, new int[2] { 1, 2 }, new int[2] { 2, 4 });
            graph.AddVertex(1, new int[1] { 2 }, new int[1] { 1 });
            graph.AddVertex(2, new int[1] { 3 }, new int[1] { 3 }); 

            graph.Print();

            graph.ShortestPath();

            Console.ReadKey();
        }
    }

    class Graph
    {

        private int numNodes;
        private int[,] adjMatrix;
        const int infinity = 10000;

        public Graph(int numNodes)
        {
            this.numNodes = numNodes;
            this.adjMatrix= new int[numNodes, numNodes];
            for (int j = 0; j < numNodes; j++)
            {
                for (int i = 0; i < numNodes; i++)
                {
                    this.adjMatrix[j,i] = i == j ? 0 : -1;
                }
            }
        }

        public void ShortestPath()
        {
            int[] shortest = new int[numNodes];
            int[] previous = new int[numNodes];

            for (int i = 0; i < numNodes; i++)
            {
                shortest[i] = infinity;
                previous[i] = i;
            }

            shortest[0] = 0;

            for (int i = 0; i < numNodes; i++) // for each node
            {
                for (int j = 0; j < numNodes; j++)  // for each path
                {
                    if (adjMatrix[i, j] != -1 && shortest[i] >  adjMatrix[i, j] + shortest[j])
                    {
                        shortest[i] = adjMatrix[i, j] + shortest[j];
                        previous[i] = j;
                    }

                }

                PrintArray(shortest);
            }

            Console.WriteLine("Path: ");
            PrintArray(previous);
        }

        private void PrintArray(int[] shortest)
        {
            foreach (int i in shortest)
                Console.Write(i.ToString() + " ");
            Console.WriteLine();
        }

        public void Print()
        {
            for (int j = 0; j < numNodes; j++) 
            {
                for (int i = 0; i < numNodes; i++) 
                {
                    Console.Write( (adjMatrix[i, j] == -1 ? "X" : adjMatrix[i, j].ToString()) );
                }
                Console.WriteLine();
            }
        }

        public void AddVertex(int nodeNum, int[] edges, int[] weights)
        {
            Debug.Assert(edges.Length == weights.Length);
            for (int i = 0; i < edges.Length; i++ )
            { 
                Debug.Assert(adjMatrix[nodeNum, edges[i]] == -1);
                Debug.Assert(adjMatrix[edges[i], nodeNum] == -1);
                adjMatrix[nodeNum, edges[i]] = weights[i];
                adjMatrix[edges[i], nodeNum] = weights[i];
            }
        }

    }
}
