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
                "010",
                "000",
                "110"
                             };

            teamBuilder.specialLocations(paths);

            Console.ReadKey();
        }
    }

    class TeamBuilder
    {
        private int[,] adjMatrix;
        int numNodes;

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
                    adjMatrix[i, j] = (paths[j][i] == '0') ? 666 : (paths[j][i] - '0');

                }
            }

            printAdjMatrix();
            Console.WriteLine();

            floydWarshall();

            printAdjMatrix();

            return new int[2] { 0, 0 };
        }

        private void floydWarshall()
        {
            for (int k = 0; k < numNodes; k++ )
            {
                for (int i = 0; i < numNodes; i++)
                {
                    for (int j = 0; j < numNodes; j++)
                    {
                        adjMatrix[i, j] = Math.Min(adjMatrix[i, j], adjMatrix[i,k] + adjMatrix[k,j]);
                        //if (adjMatrix[i,j] > adjMatrix[i,k] + adjMatrix[k,j])
                        //    adjMatrix[i, j] = adjMatrix[i,k] + adjMatrix[k, j];
                    }
                }
            }
        }

        private void printAdjMatrix()
        {
            for (int j = 0; j < numNodes; j++)
            {
                for (int i = 0; i < numNodes; i++)
                {
                    if (adjMatrix[i, j] == 666)
                        Console.Write("X");
                    else
                        Console.Write(adjMatrix[i,j]);
                }
                Console.WriteLine();
            }
        }
    }
}
