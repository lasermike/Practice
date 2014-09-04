// Implementation of Algowithms S. Dasgupta, CH Papadimitriou, and UV Vazirani's
// Exercise 3.8
// From 3 jars with the given capacities and starting states, find the the set of operations 
// needed to leave just 2 units of water in jars #2 and #3.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PouringWater
{

    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph(10, 7, 4);
            graph.Init(0, 7, 4);
            graph.Solve();

            Console.ReadKey();
        }
    }

    class Jar
    {
        public int capacity;
        public int amount;
    }


    class Node
    {
        public Jar[] jars ;
        public List<Node> children;

        public Node(int[] capacities, int[] amounts)
        {
            jars = new Jar[capacities.Length];
            for (int i = 0; i < capacities.Length; i++)
            {
                jars[i] = new Jar { capacity = capacities[i], amount = amounts[i] };
            }
            children = new List<Node>();
        }
    }

    class Graph
    {
        int[] capacities = new int[3];
        Node startState;
        Dictionary<int, Node> map;

        public Graph(int jar1Capacity, int jar2Capacity, int jar3Capacity)
        {
            capacities[0] = jar1Capacity;
            capacities[1] = jar2Capacity;
            capacities[2] = jar3Capacity;
            startState = null;
        }
        public void Init(int jar1Start, int jar2Start, int jar3Start)
        {
            map = new Dictionary<int, Node>();
            int[] nodeState = new int[3] { jar1Start, jar2Start, jar3Start };
            startState = new Node(capacities, nodeState);
            map.Add(GetKey(nodeState), startState);

            AddBranchStatesSlick(startState, 0);

            Console.WriteLine("Found solution in : added " + map.Count + " nodes");
        }

        public bool Solve()
        {

            return false;
        }

        private int GetKey(int[] amounts)
        {
            return GetKey(amounts[0], amounts[1], amounts[2]);
        }

        private int GetKey(int amount1, int amount2, int amount3)
        {
            return (amount1 << 16) + (amount2 << 8) + amount3;
        }

        private Node FindStateInMap(int amount1, int amount2, int amount3)
        {
            int key = (amount1 << 16) + (amount2 << 8) + amount3;
            return FindStateInMap(key);
        }        
        private Node FindStateInMap(int key)
        {
            Node node;
            map.TryGetValue(key, out node);
            return node ?? null;
        }

        private void PrintPadding(int depth)
        {
            for (int i = 0; i < depth; i++)
                Console.Write(" ");
        }

        private bool AddBranchStatesSlick(Node node, int depth)
        {
            for (int i = 0; i < node.jars.Length; i++)  // try pouring from i jar
            {
                for (int j = 0; j < node.jars.Length; j++)  // to j jar
                {
                    if (i == j)
                        continue;  //can't pour into self

                    if (node.jars[i].amount == 0) // does i have any water?
                        continue;

                    if (node.jars[j].amount == node.jars[j].capacity) // is j full already?
                        continue;

                    // pour from i to j
                    int newPourerAmount = 0;
                    int newReceiverAmount = node.jars[j].amount + node.jars[i].amount;
                    if (newReceiverAmount > node.jars[j].capacity)
                    {
                        newPourerAmount = newReceiverAmount - node.jars[j].capacity; // remaining in pourer
                        newReceiverAmount = node.jars[j].capacity;
                    }

                    // Add state node to graph
                    int[] stateData = new int[node.jars.Length];
                    for (int k = 0; k < node.jars.Length; k++)
                        stateData[k] = node.jars[k].amount;
                    stateData[i] = newPourerAmount;
                    stateData[j] = newReceiverAmount;

                    PrintPadding(depth);
                    if ( (i != 0 && newPourerAmount == 2) || (j != 0 && newReceiverAmount == 2) )
                    {
                        Console.WriteLine("Found solution!  " + stateData.Print());
                        return true;
                    }

                    int key = GetKey(stateData);
                    Node newNode = FindStateInMap(key);
                    if (newNode == null)
                    {
                        Console.WriteLine("Adding new state " + stateData.Print());
                        newNode = new Node(capacities, stateData);
                        map.Add(key, newNode);
                        node.children.Add(newNode);
                        if (AddBranchStatesSlick(newNode, depth + 1)) // Undiscovered, so continue to find new branches in state
                            return true;
                    }
                    else
                    {
                        Console.WriteLine("Founding existing state " + stateData.Print());
                        node.children.Add(newNode);
                    }
                }
            }
            return false;
        }

        /*private void AddBranchStates(Node node)
        {
            if (node.jars[0].amount > 0)
            {
                if (node.jars[1].amount < node.jars[1].capacity)
                {
                    int newj0 = 0;
                    int newj1 = node.jars[1].amount + node.jars[0].amount;
                    if (newj1 > capacities[1])
                    {
                        newj0 = newj1 - capacities[1];
                        newj1 = capacities[1];
                    }
                    int key = GetKey(newj0, newj1, node.jars[2].amount);
                    Node newNode = FindStateInMap(key);
                    if (newNode == null)
                    {
                        newNode = new Node(capacities, new int[3] { newj0, newj1, node.jars[2].amount });
                        map.Add(key, newNode);
                    }
                    node.children.Add(newNode);                    
                }
            }
        }*/
    }

    public static class Extensions
    {
        public static string Print(this int[] nums)
        {
            StringBuilder str = new StringBuilder();
            foreach (int num in nums)
                str.Append(num + " ");
            return str.ToString();
        }
    }
}

