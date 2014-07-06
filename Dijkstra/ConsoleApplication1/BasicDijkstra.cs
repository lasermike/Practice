
using Heaps;
using System;


namespace BasicDijkstra
{

    class Graph
    {
        private const int numNodes = 10;
        private int[,] adjMatrix;

        public Graph()
        {
            adjMatrix = new int[numNodes, numNodes];

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
            adjMatrix[from, to] = cost;
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

        class DijkstraNode : IPriorityQueueNode
        {
            public int node;
            public int runningCost;

            public int Cost() { return runningCost; }
        }

        public int Dijkstra(int from, int to)
        {
            //PriorityQueueList queue = new PriorityQueueList();
            PriorityQueueHeap<DijkstraNode> queue = new PriorityQueueHeap<DijkstraNode>(numNodes);
            bool[] visited = new bool[10];

            queue.Add(new DijkstraNode() { node = from, runningCost = 0 });
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
    }
}
