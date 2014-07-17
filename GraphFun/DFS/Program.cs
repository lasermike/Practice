using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFS
{
    class Program
    {
        static void Main(string[] args)
        {


            /*Graph graph = new Graph(10);
            graph.AddConnections('A', "CH");
            graph.AddConnections('B', "AG");
            graph.AddConnections('C', "D");
            graph.AddConnections('D', "F");
            graph.AddConnections('E', "AI");
            graph.AddConnections('F', "J");
            graph.AddConnections('G', "I");
            graph.AddConnections('H', "FG");
            graph.AddConnections('I', "H");
            graph.AddConnections('J', "C");*/

            Graph graph = new Graph(12); 
            graph.AddConnections('A', "B");
            graph.AddConnections('B', "CDE");
            graph.AddConnections('C', "F");
            //graph.AddConnections('D', "F");
            graph.AddConnections('E', "BFG");
            graph.AddConnections('F', "CH");
            graph.AddConnections('G', "HJ");
            graph.AddConnections('H', "K");
            graph.AddConnections('I', "G");
            graph.AddConnections('J', "I");
            graph.AddConnections('K', "L");
            graph.AddConnections('L', "J");


            DFSSolver solver = new DFSSolver(graph);
            solver.Solve();
            solver.Print();

            solver.FindStronglyConnected();

            Console.ReadKey();
        }
    }

    class DFSSolver
    {
        public class NodeData
        {
            public Graph.Node node;
            public int pre = 0;
            public int post = 0;
            public int ccnum = 0;

            public NodeData(Graph.Node node)
            {
                this.node = node;
            }
        }

        private Graph graph;
        int clock = 1;
        int ccnum = 1;
        List<NodeData> stronlyConnected;
        private Dictionary<char, NodeData> nodeDataDict;

        public DFSSolver(Graph graph)
        {
            this.graph = graph;
            nodeDataDict = new Dictionary<char,NodeData>(graph.Size);
            stronlyConnected = new List<NodeData>();
            for (int i = 0; i < graph.Size; i++)
            {
                nodeDataDict.Add(graph[i].Name, new NodeData(graph[i]));
            }
        }

        private string PrintEdges(Graph.Node node)
        {
            StringBuilder b = new StringBuilder();
            foreach (Graph.Node edgeNode in node.Edges)
                b.Append(edgeNode.Name);

            return b.ToString();
        }

        public void Print()
        {
            Console.WriteLine("Nodes");
            for (int i = 0; i < graph.Size; i++)
            {
                NodeData data = nodeDataDict[graph[i].Name];
                Console.WriteLine(data.node.Name + ": pre=" + data.pre + " post=" + data.post + " cc=" + data.ccnum + "  edges=" + PrintEdges(data.node));
            }

            if (stronlyConnected.Count > 0)
            {
                Console.WriteLine("Strongly connected groups");
                foreach (NodeData nodeData in stronlyConnected)
                {
                    Console.WriteLine(nodeData.node.Name);
                }
            }

        }

        public void Solve()
        {
            for (int i = 0; i < graph.Size; i++)
            {
                NodeData nodeData = nodeDataDict[graph[i].Name];
                if (nodeData.pre == 0) //not  visited?
                {
                    Explore(nodeData);
                    ccnum++;
                }
            }
        }

        private void Explore(NodeData nodeData)
        {
            nodeData.pre = clock++;
            nodeData.ccnum = ccnum;
            foreach (Graph.Node node in nodeData.node.Edges)
            {
                NodeData child = nodeDataDict[node.Name];
                if (child.pre == 0) // not visited?
                {
                    Explore(child);
                }
            }
            nodeData.post = clock++;
        }

        private NodeData HighestPost()
        {
            NodeData curHighest = nodeDataDict[graph[0].Name];
            for (int i = 1; i < graph.Size; i++)
            {
                NodeData nodeData = nodeDataDict[graph[i].Name];
                if (nodeData.post > curHighest.post)
                    curHighest = nodeData;
            }
            return curHighest;
        }

        private class NodeCompareByPost : IComparer<char>
        {
            Dictionary<char, NodeData> map;

            public NodeCompareByPost(Dictionary<char, NodeData> map )
            {
                this.map = map;
            }
            public int Compare(char x, char y)
            {
                return (map[x].post > map[y].post) ? 1 : 0;
            }
        }


        public void FindStronglyConnected()
        {
            Graph rGraph = graph.Reverse();
            DFSSolver rSolver = new DFSSolver(rGraph);
            rSolver.Solve();
            Console.WriteLine("Highest post in reversed graph=" + rSolver.HighestPost().node.Name);
            rSolver.Print();
            Console.WriteLine();

            Console.WriteLine("Sorting graph by post order of first graph");

            Graph sortedGraph = graph.Sort(rSolver.nodeDataDict);
            DFSSolver sortedSolver = new DFSSolver(sortedGraph);
            sortedSolver.Solve();

            Console.WriteLine("Connected groups!");
            sortedSolver.Print();


        }

    }


    class Graph
    {
        public class NodeNameComparer : IComparer<Node>
        {
            public int Compare(Node x, Node y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        public class Node
        {
            private char name;
            private List<Node> adjList = new List<Node>();

            public Node(char name)
            {
                this.name = name;
            }
            
            public char Name
            {
                get
                {
                    return name;
                }
            }

            public void AddEdge(Node node)
            {
                adjList.Add(node);
                adjList.Sort( new NodeNameComparer() );
            }

            public List<Node> Edges
            {
                get
                {
                    return adjList;
                }
            }
        }

        private int size;
        private Node[] nodes;

        public Graph(int size)
        {
            this.size = size;
            this.nodes = new Node[size];
            for (int i = 0; i < size; i++)
            {
                this.nodes[i] = new Node( GetNodeName(i) );
            }
        }

        private Graph(Node[] nodes)
        {
            this.nodes = nodes;
            size = nodes.Length;
        }
        
        public int Size { get { return size; } }

        private char GetNodeName(int index) { return (char) (index + 'A'); }
        private Node GetNode(char name) { return this.nodes[name - 'A']; }

        public void AddConnections(char name, string connections)
        {
            Node node = GetNode(name);
            for (int i = 0; i < connections.Length; i++)
            {
                node.AddEdge(GetNode(connections[i]));
            }

        }

        public Node this[int index]
        {
            get 
            {
                return nodes[index];
            }
        }

        public Node this[char name]
        {
            get
            {
                return GetNode(name);
            }
        }

        public Graph Reverse()
        {
            Graph graph = new Graph(this.size);
            for (int i = 0; i < this.size; i++)
            {
                Node node = this.nodes[i];
                foreach (Node adjNode in node.Edges)
                {
                    graph.AddConnections(adjNode.Name, node.Name.ToString());
                }
            }

            return graph;
        }

        public Graph Sort(Dictionary<char, DFSSolver.NodeData> map)
        {
            return new Graph(nodes.OrderByDescending( x => map[x.Name].post ).ToArray());
        }
    }
}
