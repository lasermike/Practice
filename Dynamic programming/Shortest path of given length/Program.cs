using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortest_path_of_given_length
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph();

            graph.AddEdge('S', 'A', 1);
            graph.AddEdge('S', 'B', 3);
            graph.AddEdge('A', 'C', 2);
            graph.AddEdge('A', 'D', 4);
            graph.AddEdge('B', 'D', 3);
            graph.AddEdge('C', 'D', 1);
            graph.AddEdge('C', 'T', 5);
            graph.AddEdge('D', 'T', 2);
            graph.FindShortestPathOfLength('S', 'T', 3);

        }

        class Graph
        {
            struct Edge
            {
                public Edge(Vertex dest, int cost) 
                {
                    this.dest = dest;
                    this.cost = cost;
                }

                public Vertex dest;
                public int cost;
            }
            struct Vertex 
            {
                public char id;
                public List<Edge> edges;

                public Vertex(char id)
                {
                    this.id = id;
                    edges = new List<Edge>();
                }
            }

            Dictionary<char, Vertex> vertices = new Dictionary<char,Vertex>();

            public Graph() { }

            public void AddEdge(char c1, char c2, int len)
            {
                Vertex v1;
                Vertex v2;

                if (!vertices.TryGetValue(c1, out v1))
                {
                    v1 = new Vertex(c1);
                    vertices.Add(c1, v1);
                }
                if (!vertices.TryGetValue(c2, out v2))
                {
                    v2 = new Vertex(c2);
                    vertices.Add(c2, v2);
                }

                v1.edges.Add(new Edge(v2, len));
                v2.edges.Add(new Edge(v1, len));
            }

            public void FindShortestPathOfLength(char start, char end, int maxLen)
            {
                Dictionary<Vertex, int[]> dist = new Dictionary<Vertex, int[]>();

                // init
                foreach (Vertex v in vertices.Values)
                {
                    int[] lens = new int[maxLen + 2];
                    for (int l = 0; l < maxLen + 2; l++)
                    {
                        lens[l] = v.id == start ? 0 : int.MaxValue;
                    }
                    dist.Add(v, lens);
                }

                Debug.Assert(dist.First().Key.id == 'S');
                Debug.Assert(dist.Last().Key.id == 'T');

                for (int j = 1; j <= maxLen + 1; j++)
                {
                    foreach(Vertex v in vertices.Values)
                    {
                        foreach(Edge e in v.edges)
                        {
                            if (dist[v][j - 1] == int.MaxValue)
                            { 
                                dist[e.dest][j] = Math.Min(dist[e.dest][j - 1], dist[e.dest][j]);
                            }
                            else
                            {
                                int min = Math.Min(dist[e.dest][j - 1], dist[v][j - 1] + e.cost);
                                dist[e.dest][j] = Math.Min(dist[e.dest][j], min);
                            }
                        }
                    }
                }



                for (int j = 0; j <= maxLen + 1; j++)
                {
                    foreach (Vertex v in dist.Keys)
                    {
                        Console.Write( (dist[v][j] == int.MaxValue ? "-" : dist[v][j].ToString()) + ", ");
                    }
                    Console.WriteLine();
                }

                Console.ReadKey();
            }

        }
    }
}
