// Finds the nodes that are reachable by all other nodes, and nodes that can be reached from all nodes.
// Uses Floyd-Warshall algorithm to compute shortest path between all pairs of nodes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_Warshall
{
    class Program
    {
        static void Main(string[] args)
        {
            TeamBuilder teamBuilder = new TeamBuilder();

            string[] paths = {
/*                "010",
                "000",
                "110"
*/
/*                "0010",
                "1000",
                "1100",
                "1000"
 */
//                "01000","00100","00010","00001","10000"
                "0110000","1000100","0000001","0010000","0110000","1000010","0001000"

                             };

            int[] retval = teamBuilder.specialLocations(paths);

            Console.WriteLine("reach all: " + retval[0]);
            Console.WriteLine("reachable all: " + retval[1]);
            Console.ReadKey();
        }
    }

    class TeamBuilder
    {
        private int[,] adjMatrix;
        int numNodes;
        const int max = 1000;
        public TeamBuilder()
        {
        }

        public int[] specialLocations(string[] paths)
        {
            numNodes = paths.Length;
            adjMatrix = new int[numNodes, numNodes];
            for (int j = 0; j < numNodes; j++)
            {
                for (int i = 0; i < numNodes; i++)
                {
                    int input = paths[j][i] - '0';
                    if (i == j)
                        adjMatrix[i, j] = 0;
                    else if (input == 0)
                        adjMatrix[i, j] = max;
                    else
                        adjMatrix[i, j] = input;

                }
            }

            Console.WriteLine("Starting matrix");
            printAdjMatrix();
            Console.WriteLine();

            floydWarshall();

            int reachAll = findReachAll();
            int reachableAll = findReachableAll();

            return new int[2] { reachAll, reachableAll };
        }

        /// <summary>
        /// Determine the number of nodes that can reach all other nodes
        /// </summary>
        /// <returns></returns>
        private int findReachAll()
        {
            int reachAll = 0;
            for (int j = 0; j < numNodes; j++)
            {
                int i = 0;
                for (; i < numNodes; i++)
                {
                    if (i != j && adjMatrix[i, j] == max)
                        break;
                }
                if (i == numNodes)
                    reachAll++;
            }
            return reachAll;
        }

        /// <summary>
        /// Determine the number of nodes that are reachable by all other nodes
        /// </summary>
        /// <returns></returns>
        private int findReachableAll()
        {
            int reachableAll = 0;

            for (int j = 0; j < numNodes; j++)
            {
                int i = 0;
                for (; i < numNodes; i++)
                {
                    if (i != j && adjMatrix[j, i] == max)
                        break;
                }
                if (i == numNodes)
                    reachableAll++;
            }

            return reachableAll;
        }

        /// <summary>
        /// For each pair of nodes start and end, iteratively find a path through all other nodes between start and end
        /// </summary>
        private void floydWarshall()
        {
            for (int k = 0; k < numNodes; k++ )
            {
                for (int j = 0; j < numNodes; j++)
                {
                    for (int i = 0; i < numNodes; i++)
                    {
                        //adjMatrix[i, j] = Math.Min(adjMatrix[i, j], adjMatrix[i,k] + adjMatrix[k,j]);
                        if (adjMatrix[i,j] > adjMatrix[i,k] + adjMatrix[k,j])
                            adjMatrix[i, j] = adjMatrix[i,k] + adjMatrix[k, j];
                    }
                }

                Console.WriteLine("Matrix at k="+k);
                printAdjMatrix();
                Console.WriteLine();
            }


        }

        private void printAdjMatrix()
        {
            for (int j = 0; j < numNodes; j++)
            {
                for (int i = 0; i < numNodes; i++)
                {
                    if (adjMatrix[i, j] == max)
                        Console.Write("X");
                    else
                        Console.Write(adjMatrix[i,j]);
                }
                Console.WriteLine();
            }
        }
    }
}
