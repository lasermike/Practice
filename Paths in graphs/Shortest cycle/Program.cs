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
                int index1 = id1 - 'A';
                int index2 = id2 - 'A';

                Edge edge = new Edge(vertices[index1], vertices[index2], length);
                vertices[index1].edges.Add(index2, edge); 
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
                    if (dist[data[index]] > dist[data[LeftIndex(index)]])
                    {
                        Vertex temp = data[index];
                        data[index] = data[LeftIndex(index)];
                        data[LeftIndex(index)] = temp;
                        SiftDown(LeftIndex(index));
                    }
                    else if (dist[data[index]] > dist[data[RightIndex(index)]])
                    {
                        Vertex temp = data[index];
                        data[index] = data[RightIndex(index)];
                        data[RightIndex(index)] = temp;
                        SiftDown(RightIndex(index));
                    }
                }

                public void SiftUp(Vertex vertex)
                {
                    for (int i = 0; i < data.Length; i++)
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
                    if (dist[data[index]] < dist[data[Parent(index)]])
                    {
                        Vertex temp = data[index];
                        data[index] = data[Parent(index)];
                        data[Parent(index)] = temp;
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
        }

        static void Main(string[] args)
        {
            Graph graph = new Graph(5);
            graph.AddVertex('A');
            graph.AddVertex('B');
            graph.AddVertex('C');
            graph.AddVertex('D');
            graph.AddVertex('E');

            graph.AddEdge('A', 'B', 1);
            graph.AddEdge('A', 'C', 2);
            graph.AddEdge('A', 'D', 2);
            graph.AddEdge('B', 'C', 2);
            graph.AddEdge('C', 'E', 2);
            graph.AddEdge('D', 'E', 1);

            int shortCycleLen =  graph.FindShortestCycle('A', 'C');
            Console.WriteLine("Shorted cycle is " + shortCycleLen);
            Console.ReadKey();
        }
    }
}
