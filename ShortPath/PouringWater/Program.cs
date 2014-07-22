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
            graph.Init(0, 2, 2);
            graph.Solve();
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

        public bool Solve()
        {

            return false;
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

        public void Init(int jar1Start, int jar2Start, int jar3Start)
        {
            map = new Dictionary<int, Node>();
            startState = new Node(capacities, new int[3] { jar1Start, jar2Start, jar3Start });
            map.Add(GetKey(jar1Start, jar2Start, jar3Start), startState);

            AddBranchStates(startState);
        }

        private void AddBranchStatesSlick(Node node)
        {
            for (int i = 0; i < node.jars.Length; i++)  // try pouring from i jar
            {
                for (int j = 0; j < node.jars.Length; j++)  // to j jar
                {
                    if (i == j)
                        continue;  //can't pour into self

                    if (node.jars[i].amount < node.jars[j].capacity)
                    {
                        int newPourer = 0;
                        int newReceiver = node.jars[j].amount + node.jars[i].amount;
                        if (newj1 > capacities[1])
                        {
                            newj0 = newj1 - capacities[1];
                            newj1 = capacities[1];
                        }

                    }
                }
            }
        }

        private void AddBranchStates(Node node)
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
        }

    }
}
