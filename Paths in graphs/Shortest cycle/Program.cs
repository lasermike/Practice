using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Find length shortest cycle containing specific edge in an undirected graph

namespace Shortest_cycle
{
    class Program
    {
        class Vertex
        {
            public char id;
            public Dictionary<int, Edge> edges;  // Edge list

            public Vertex(char id) 
            { 
                this.id = id;
                edges = new Dictionary<int,Edge>();
            }
        }

        class Edge
        {
            public Vertex item1;
            public Vertex item2;
            public int length;

            public Edge(Vertex i1, Vertex i2, int length)
            {
                item1 = i1;
                item2 = i2;
                this.length = length;
            }

        }

        class Graph
        {
            Vertex[] vertices;

            public Graph(int size)
            {
                vertices = new Vertex[size];                
            }

            public void AddVertex(char id)
            {
                int index = id - 'A';
                Debug.Assert(vertices[index] == null);
                vertices[id - 'A'] = new Vertex(id);
            }

            public void AddEdge(char id1, char id2, int length)
            {
                Console.WriteLine("  edge: " + id1 + " - " + id2 + ": "+length);
                int index1 = id1 - 'A';
                int index2 = id2 - 'A';

                Edge edge = new Edge(vertices[index1], vertices[index2], length);
                vertices[index1].edges.Add(index2, edge);
                edge = new Edge(vertices[index2], vertices[index1], length);
                vertices[index2].edges.Add(index1, edge); 
            }

            class Heap
            {
                Vertex[] data;
                Dictionary<Vertex, int> dist;
                int heapLength = 0;

                public Heap(int size)
                {
                    data = new Vertex[size];
                }

                public void MakeHeap(Vertex[] vertices, Dictionary<Vertex, int> dist)
                {
                    this.dist = dist;

                    int index = 1;
                    foreach (Vertex v in vertices)
                    {
                        if (dist[v] == 0) // Put start node in front
                        {
                            data[0] = v;
                        }
                        else
                        {
                            data[index++] = v;
                        }
                    }
                    heapLength = vertices.Length;
                }

                public Vertex PopMin()
                {
                    Vertex retval = data[0];
                    data[0] = data[heapLength - 1];
                    heapLength--;
                    SiftDown(0);
                    return retval;
                }

                private int LeftIndex(int index)
                {
                    return index * 2 + 1;
                }
                private int RightIndex(int index)
                {
                    return index * 2 + 2;
                }
                private int Parent(int index)
                {
                    return (int) (Math.Ceiling(index / 2.0)) - 1;
                }

                private void SiftDown(int index)
                {
                    if (LeftIndex(index) < heapLength - 1 && dist[data[index]] > dist[data[LeftIndex(index)]])
                    {
                        Vertex temp = data[index];
                        data[index] = data[LeftIndex(index)];
                        data[LeftIndex(index)] = temp;
                        SiftDown(LeftIndex(index));
                    }
                    else if (RightIndex(index) < heapLength - 1 && dist[data[index]] > dist[data[RightIndex(index)]])
                    {
                        Vertex temp = data[index];
                        data[index] = data[RightIndex(index)];
                        data[RightIndex(index)] = temp;
                        SiftDown(RightIndex(index));
                    }
                }

                public bool Contains(Vertex vertex)
                {
                    for (int i = 0; i < heapLength; i++)
                    {
                        if (data[i] == vertex)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                public void SiftUp(Vertex vertex)
                {
                    for (int i = 0; i < heapLength; i++)
                    {
                        if (data[i] == vertex)
                        {
                            SiftUp(i);
                            return;
                        }
                    }
                    throw new Exception("Invalid arg to sift up");
                }
                 
                private void SiftUp(int index)
                {
                    if (Parent(index) >= 0 && dist[data[index]] < dist[data[Parent(index)]])
                    {
                        Vertex temp = data[index];
                        data[index] = data[Parent(index)];
                        data[Parent(index)] = temp;
                        SiftUp(Parent(index));
                    }
                }

                public bool IsEmpty()
                {
                    return heapLength == 0;
                }


            }

            public int FindShortestCycle(char edgeStart, char edgeEnd)
            {
                Vertex start = vertices[edgeStart - 'A'];
                Vertex end = vertices[edgeEnd - 'A'];

                Dictionary<Vertex, int> dist = new Dictionary<Vertex, int>(vertices.Length);
                foreach (Vertex v in vertices)
                {
                    dist.Add(v, int.MaxValue);
                }
                dist[start] = 0;

                Heap queue = new Heap(vertices.Length);
                
                queue.MakeHeap(vertices, dist);

                while (!queue.IsEmpty())
                {
                    Vertex curr = queue.PopMin();

                    foreach (Edge edge in curr.edges.Values)
                    {
                        if (edge.item1 != start || edge.item2 != end)
                        {
                            if (dist[edge.item2] > dist[edge.item1] + edge.length)
                            {
                                dist[edge.item2]  = dist[edge.item1] + edge.length;
                                queue.SiftUp(edge.item2);
                            }
                        }
                    }

                }

                return dist[end];
            }


            public bool HasUniqueShortestPath(char edgeStart, char edgeEnd)
            {
                Vertex start = vertices[edgeStart - 'A'];
                Vertex end = vertices[edgeEnd - 'A'];

                Dictionary<Vertex, int> dist = new Dictionary<Vertex, int>(vertices.Length);
                Dictionary<Vertex, Vertex> prev = new Dictionary<Vertex, Vertex>(vertices.Length);
                Dictionary<Vertex, bool> dupePath = new Dictionary<Vertex, bool>(vertices.Length);

                foreach (Vertex v in vertices)
                {
                    dist.Add(v, int.MaxValue);
                    dupePath.Add(v, false);
                    prev.Add(v, v);
                }
                dist[start] = 0;

                Heap queue = new Heap(vertices.Length);
                
                queue.MakeHeap(vertices, dist);

                while (!queue.IsEmpty())
                {
                    Vertex curr = queue.PopMin();

                    foreach (Edge edge in curr.edges.Values)
                    {
                        Vertex vnext = edge.item1 == curr ? edge.item2 : edge.item1;
                        if (queue.Contains(vnext))
                        {
                            if (dist[vnext] > dist[curr] + edge.length)
                            {
                                dist[vnext] = dist[curr] + edge.length;
                                queue.SiftUp(vnext);
                                prev[vnext] = curr;
                            }
                            else if (dist[vnext] == dist[curr] + edge.length)
                            {
                                dupePath[vnext] = true;
                            }
                        }
                    }

                }

                if (dist[end] != int.MaxValue)
                {
                    Console.WriteLine("Shortest path: ");
                    Vertex curr = end;
                    do
                    {
                        Console.Write(curr.id + ", ");
                        curr = prev[curr];
                    } while (curr != start);
                    Console.WriteLine();


                    // Detect shortest path
                    curr = end;
                    while (curr != start)
                    {
                        if (dupePath[curr])
                        {
                            Console.WriteLine("Dupe path at: " + curr.id);
                            return false;
                        }
                        curr = prev[curr];
                    }


                    return true;
                }

                return false;
            }

        }

        static void Main(string[] args)
        {
            //FindShortestCycle();
            TestForUniqueShortestPath();
        }

        static void TestForUniqueShortestPath()
        {
            Random rand = new Random(5);
            for (int i = 0; i < 1; i++)
            {
                Graph graph = new Graph(6);
                graph.AddVertex('A');
                graph.AddVertex('B');
                graph.AddVertex('C');
                graph.AddVertex('D');
                graph.AddVertex('E');
                graph.AddVertex('F');

                const int maxWeight = 3;
                graph.AddEdge('A', 'B', rand.Next(maxWeight));
                graph.AddEdge('A', 'C', rand.Next(maxWeight));
                graph.AddEdge('B', 'D', rand.Next(maxWeight));
                graph.AddEdge('C', 'D', rand.Next(maxWeight));
                graph.AddEdge('C', 'E', rand.Next(maxWeight));
                graph.AddEdge('D', 'F', rand.Next(maxWeight));
                graph.AddEdge('E', 'F', rand.Next(maxWeight));
              


                /*
                graph.AddEdge('A', 'B', 2);
                graph.AddEdge('A', 'C', 1);
                graph.AddEdge('B', 'D', 1);
                graph.AddEdge('C', 'D', 1);
                graph.AddEdge('C', 'E', 2);
                graph.AddEdge('D', 'F', 2);
                graph.AddEdge('E', 'F', 1);
                */



                bool hasUniqueShortestPath = graph.HasUniqueShortestPath('A', 'F');
                Console.WriteLine(hasUniqueShortestPath ? "Graph has unique shortest path" : "Graph does not have unique shortest path");
            }
            Console.ReadKey();
        }

        static void FindShortestCycle()
        {
            Graph graph = new Graph(5);
            graph.AddVertex('A');
            graph.AddVertex('B');
            graph.AddVertex('C');
            graph.AddVertex('D');
            graph.AddVertex('E');
            graph.AddVertex('F');

            graph.AddEdge('A', 'B', 1);
            graph.AddEdge('A', 'C', 1);
            graph.AddEdge('B', 'D', 1);
            graph.AddEdge('C', 'D', 1);
            graph.AddEdge('C', 'E', 6);
            graph.AddEdge('D', 'F', 2);
            graph.AddEdge('E', 'F', 1);

            Console.WriteLine("Graph has " + graph.FindShortestCycle('A', 'C') + " shortest cycle");
            Console.ReadKey();
        }
    }
}
