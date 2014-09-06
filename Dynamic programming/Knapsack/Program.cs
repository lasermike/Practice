using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack
{
    class Program
    {
        static void Main(string[] args)
        {
            Knapsack sack = new Knapsack();
            sack.AddItem(24, 12);
            sack.AddItem(13, 7);
            sack.AddItem(23, 11);
            sack.AddItem(15, 8);
            sack.AddItem(16, 9);
            int ans = sack.Solve(26);
            //sack.AddItem(5, 2);
            //sack.AddItem(3, 2);
            //int ans = sack.Solve(2);

            Console.WriteLine("Ans = " + ans);
            Console.ReadKey();
        }
    }

    class Knapsack
    {
        struct Item
        {
            public int value;
            public int weight;
        }

        List<Item> itemList = new List<Item>();
        public Knapsack() { }

        public void AddItem(int value1, int weight1)
        {
            itemList.Add(new Item() { value = value1, weight = weight1 });
        }

        public int Solve(int capacity)
        {
            Item[] items = itemList.ToArray();
            int[,] bag = new int[capacity + 1, items.Length + 1];

            for (int i = 0; i < items.Length + 1; i++)
            {
                bag[0, i] = 0;
            }
            for (int w = 0; w < capacity + 1; w++)
            {
                bag[w, 0] = 0;
            }

            for (int w = 1; w <= capacity; w++)
            {
                for (int i = 1; i <= items.Length; i++)
                {
                    if (items[i - 1].weight <= w)
                    {
                        bag[w, i] = Math.Max(bag[w, i - 1], bag[w - items[i - 1].weight, i - 1] + items[i - 1].value );
                    }
                }
            }

            for (int w = 0; w < capacity + 1; w++)
            {
                for (int i = 0; i < items.Length + 1; i++)
                {
                    Console.Write(bag[w, i] + ", ");
                }
                Console.WriteLine();
            }
                return bag[capacity, items.Length];
        }
    }
}
