using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    class Graph
    {
        private const int numNodes = 10;
        private int[,] adjMatrix;

        public Graph()
        {
            adjMatrix = new int[numNodes,numNodes];

            for (int j = 0; j < numNodes; j++)
            {
                for (int i = 0; i < numNodes; i++)
                {
                    adjMatrix[i, j] = int.MaxValue;
                }
            }
        }

        public void AddConnection(int from, int to, int cost)
        {
            adjMatrix[from,to] = cost;
        }

        public void ShowAdjMatrix()
        {
            for (int j = 0; j < numNodes; j++)
            {
                for (int i = 0; i < numNodes; i++)
                {
                    Console.Write(adjMatrix[j, i] == int.MaxValue ? "X" : adjMatrix[j, i].ToString());
                }
                Console.WriteLine();
            }
        }

        class DijkstraNode
        {
            public int node;
            public int runningCost;
        }

        public int Dijkstra(int from, int to)
        {
            //PriorityQueueList queue = new PriorityQueueList();
            PriorityQueueHeap queue = new PriorityQueueHeap(numNodes);
            bool[] visited = new bool[10];

            queue.Add( new DijkstraNode() { node = from, runningCost = 0 } );
            while (!queue.IsEmpty())
            {
                DijkstraNode top = queue.Top();
                queue.Pop();
                if (top.node == to)
                {
                    return top.runningCost;  // DONE!
                }

                if (visited[top.node]) continue; // Been there, done that

                for (int i = 0; i < numNodes; i++)
                {
                    if (i == top.node) continue; // can't travel to self
                    if (adjMatrix[top.node, i] != int.MaxValue)
                    {
                        queue.Add(new DijkstraNode { node = i, runningCost = top.runningCost + adjMatrix[top.node, i] });
                    }
                }

                visited[top.node] = true;
            }

            return int.MaxValue; // No path
        }

        class PriorityQueueHeap
        {
            int maxSize = 0;
            int currentSize = 0;
            DijkstraNode[] queue;

            public PriorityQueueHeap(int size)
            {
                maxSize = size;
                queue = new DijkstraNode[maxSize];
            }

            
            public void Add(DijkstraNode node)
            {
                queue[currentSize] = node;
                currentSize++;
                SiftUp();
            }

            public DijkstraNode Top()
            {
                if (IsEmpty()) throw new ArgumentOutOfRangeException();
                return queue[0];
            }

            public void Pop()
            {
                if (IsEmpty()) 
                    throw new ArgumentOutOfRangeException();

                queue[0] = queue[--currentSize];
                SiftDown();
            }

            public bool IsEmpty()
            {
                return currentSize == 0;
            }

            int Parent(int node) { return node / 2; }
            int Left(int node) { return node * 2 + 1; }
            int Right(int node) { return node * 2 + 2; }

            void SiftDown()
            {
                int current = 0;

                while (current < currentSize - 1)
                {
                    if (Left(current) == currentSize - 1)
                    {
                        if (queue[current].runningCost > queue[Left(current)].runningCost)
                        {
                            Swap(current, Left(current));
                        }
                        break; // At the end of the tree
                    }
                    else if (Right(current) < Left(current))
                    {
                        if (queue[current].runningCost > queue[Right(current)].runningCost)
                        {
                            Swap(current, Right(current));
                        }
                        current = Right(current);
                    }
                    else if (Left(current) < Right(current))
                    {
                        if (queue[current].runningCost > queue[Left(current)].runningCost)
                        {
                            Swap(current, Left(current));
                        }
                        current = Left(current);
                    }
                }
            }

            void SiftUp()
            {
                int current = currentSize - 1;

                while (current > 0)
                {
                    if (queue[current].runningCost < queue[Parent(current)].runningCost)
                    {
                        Swap(current, Parent(current));
                    }
                    current = Parent(current);
                }
            }

            void Swap(int i, int j)
            {
                DijkstraNode temp = queue[i];
                queue[i] = queue[j];
                queue[j] = temp;

            }
        }


        class PriorityQueueList
        {
            ArrayList list = new ArrayList();

            public void Add(DijkstraNode node) 
            {
                int i;
                for (i = 0; i < list.Count; i++)
                {
                    if (node.runningCost < ((DijkstraNode) list[i]).runningCost )
                    {
                        break;
                    }
                }
                list.Insert(i, node);
            }
            public DijkstraNode Top()
            {
                return (DijkstraNode) list[0];
            }

            public void Pop() 
            {
                list.RemoveAt(0);
            }

            public bool IsEmpty() 
            {
                return list.Count == 0;
            }
        }
    }


    class Program
    {


        static void Main(string[] args)
        {
            Graph graph = new Graph();
            graph.AddConnection(0, 6, 1);
            graph.AddConnection(6, 7, 1);
            graph.AddConnection(7, 8, 1);
            graph.AddConnection(8, 9, 1);
            graph.AddConnection(9, 4, 1);

            graph.AddConnection(0, 4, 9);
            graph.AddConnection(0, 1, 3);
            graph.AddConnection(0, 2, 5);

            graph.AddConnection(1, 4, 4);
            graph.AddConnection(1, 0, 1);

            graph.AddConnection(2, 4, 9);

            graph.AddConnection(1, 4, 9);
            
            graph.ShowAdjMatrix();

            int cost = graph.Dijkstra(0, 4);
            Console.WriteLine("cost: " + (cost == int.MaxValue ? "NO PATH" : cost.ToString()) );
            Console.ReadKey();
        }
    }
}
