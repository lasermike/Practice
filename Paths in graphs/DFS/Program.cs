using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFS
{
    class Program
    {
        class Graph
        {
            private int[,] adjacency;
            bool dag;

            public Graph(int size, bool dag)
            {
                adjacency = new int[size, size];
                Array.Clear(adjacency, 0, size * size);
                this.dag = dag;
            }

            public void AddEdge(char from, char to, int dist = 1)
            {
                AddEdge((int)from - (int)(Char.IsLower(from) ? 'a' : 'A'),
                        (int)to - (int)(Char.IsLower(to) ? 'a' : 'A'),
                        dist);
            }

            public void AddEdge(int from, int to, int dist = 1)
            {
                adjacency[from, to] = dist;
                if (!dag)
                    adjacency[to, from] = dist;

            }

            public void DFS(char from)
            {
                DFS((int)from - (int)(Char.IsLower(from) ? 'a' : 'A'));
            }

            public void DFS(int from)
            {
                int[] dist = new int[adjacency.GetLength(0)];
                for (int i = 0; i < dist.GetLength(0); i++)
                    dist[i] = int.MaxValue;

                Queue<int> queue = new Queue<int>();
                queue.Enqueue(from);
                dist[from] = 0;

                while (queue.Count > 0)
                {
                    int current = queue.Dequeue();
                    for (int i = 0; i < adjacency.GetLength(0); i++)
                    {
                        if (adjacency[current, i] != 0)
                        {
                            // If we have not traversed this node, queue for traversal
                            if (dist[i] == int.MaxValue)
                            {
                                queue.Enqueue(i);
                                dist[i] = dist[current] + 1;
                            }
                        }
                    }
                }

                PrintDistanceArray(from, dist);
            }

            public void PrintDistanceArray(int from, int[] dist)
            {
                Console.WriteLine("Distance from " + (char)(from + 'A') + " to...");
                for (int i = 0; i < dist.GetLength(0); i++)
                {
                    Console.WriteLine("  {0} is {1}", (char)(i + 'A'), dist[i]);
                }
            }

        
            class DAryPriorityQueue
            {
                private int dimension;
                private int[] queue;
                private int queueSize;
                private int[] dist;

                // Assumes a single value 0 with all others infinity.  
                // This value is stored as the first element in the array
                public DAryPriorityQueue(int[] dist, int dimension) 
                {
                    this.dimension = dimension;
                    queueSize = dist.Length;
                    queue = new int[queueSize];
                    this.dist = dist;
                    bool once = false;

                    for (int i = 0; i < dist.Length; i++)
                    {
                        if (dist[i] == 0)
                        {
                            Debug.Assert(!once, "More than one node with zero distance!");
                            once = true;

                            queue[i] = queue[0];
                            queue[0] = i;
                        }
                        else
                        {
                            queue[i] = i; // Set queue item to index into dist array
                        }
                    }
                }

                public int DeleteMin()
                {
                    int min = queue[0];

                    queue[0] = queue[queueSize - 1];
                    queueSize--;
                    SiftDown(0);
                    return min;
                }

                private void SiftDown(int index)
                {
                    for (int i = 0; i < dimension; i++)
                    {
                        int childIndex = ChildIndex(index, i);
                        if (childIndex >= queueSize)
                            return;

                        if (dist[queue[childIndex]] < dist[queue[index]])
                        {
                            // Swap
                            int temp = queue[index];
                            queue[index] = queue[childIndex];
                            queue[childIndex] = temp;
                            SiftDown(childIndex);
                            return; //right?
                        }
                    }
                }

                private int ParentIndex(int index)
                {
                    return (int) ((index - 1.0f) / dimension);
                }

                private int ChildIndex(int index, int numChild)
                {
                    return index * dimension + numChild + 1;
                }

                private int FindIndex(int id)
                {
                    for (int i = 0; i < queueSize; i++)
                    {
                        if (queue[i] == id)
                            return i;
                    }

                    throw new Exception("id "+id+" cannot be found");
                }

                public void DecreaseKey(int id)
                {
                    int index = FindIndex(id);
                    DecreaseKeyByIndex(index);
                }

                private void DecreaseKeyByIndex(int index)
                {
                    if (index < 1)
                        return;

                    if (dist[queue[ParentIndex(index)]] > dist[queue[index]])
                    {
                        // Swap with parent
                        int tempParentId = queue[ParentIndex(index)];
                        queue[ParentIndex(index)] = queue[index];
                        queue[index] = tempParentId;
                        DecreaseKeyByIndex(ParentIndex(index));
                    }
                }

                public bool IsEmpty()
                {
                    return queueSize <= 0;
                }
            }

            public void DijkstraDAray(char from)
            {
                DijkstraDAray(from - (Char.IsLower(from) ? 'a' : 'A'));
            }

            public void DijkstraDAray(int from)
            {
                //Init distance from "from" array 
                int[] dist = new int[adjacency.GetLength(0)];
                for (int i = 0; i < dist.GetLength(0); i++)
                    dist[i] = int.MaxValue;

                // Init prev step array
                int[] prev = new int[adjacency.GetLength(0)];
                Array.Clear(prev, 0, prev.Length);
                dist[from] = 0;

                DAryPriorityQueue queue = new DAryPriorityQueue(dist, 3);  // use distances
                while (!queue.IsEmpty())
                {
                    int current = queue.DeleteMin();
                    for (int v = 0; v < adjacency.GetLength(0); v++)
                    {
                        if (adjacency[current, v] > 0) // Path from current to v?
                        {
                            if (dist[v] > dist[current] + adjacency[current, v])
                            {
                                // Set closer distance
                                dist[v] = dist[current] + adjacency[current, v];
                                prev[v] = current;
                                queue.DecreaseKey(v);
                            }
                        }
                    }
                }
                PrintDistanceArray(from, dist);
            }
        }

        static void Main(string[] args)
        {
            //SimpleDFS();
            DijkstraDAray();
        }

        static void DijkstraDAray()
        {
            Graph graph = new Graph(5, true);

            graph.AddEdge('A', 'B', 4);
            graph.AddEdge('A', 'C', 2);
            graph.AddEdge('B', 'C', 3);
            graph.AddEdge('B', 'D', 2);
            graph.AddEdge('B', 'E', 3);
            graph.AddEdge('C', 'B', 1);
            graph.AddEdge('C', 'D', 4);
            graph.AddEdge('C', 'E', 5);
            graph.AddEdge('E', 'D', 1);

            graph.DijkstraDAray('A');

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static void SimpleDFS()
        {
            Graph graph = new Graph(10, false);

            graph.AddEdge('A', 'G');
            graph.AddEdge('A', 'B');
            graph.AddEdge('B', 'C');
            graph.AddEdge('G', 'C');
            graph.AddEdge('G', 'D');
            graph.AddEdge('G', 'E');

            graph.DFS('G');

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
