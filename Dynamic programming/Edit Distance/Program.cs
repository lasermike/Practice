using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edit_Distance
{
    class Program
    {
        static void Main(string[] args)
        {
            EditDistanceCalculator calc = new EditDistanceCalculator();
            calc.ComputeDistance("snowy", "sunny");


        }

        class EditDistanceCalculator
        {
            public EditDistanceCalculator()
            {
                
            }

            public void ComputeDistance(string str1, string str2)
            {
                int[,] distances = new int[str1.Length + 1, str2.Length + 1];

                for (int i = 0; i < str1.Length; i++)
                {
                    distances[i, 0] = 0;
                }
                for (int j = 0; j < str2.Length; j++)
                {
                    distances[0, j] = 0;
                }

                for (int i = 1; i <= str1.Length; i++ )
                { 
                    for (int j = 1; j <= str2.Length; j++)
                    {
                        int cellCost;
                        if (str1[i-1] == str2[j-1])
                            cellCost = 0;
                        else
                            cellCost = 1;

                        distances[i, j] = Math.Min(distances[i - 1, j] + cellCost, distances[i, j - 1] + cellCost);
                        distances[i, j] = Math.Min(distances[i - 1, j - 1] + cellCost, distances[i, j]);
                    }
                }

                for (int i = 0; i < distances.GetLength(0); i++)
                {
                    for (int j = 0; j < distances.GetLength(1); j++)
                    {
                        Console.Write(distances[i, j] + " ");
                    }
                    Console.WriteLine();
                }

                Console.ReadKey();
            }
        }
    }
}
