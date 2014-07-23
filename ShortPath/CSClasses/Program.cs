using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph graph = new Graph();
            graph.Add("CS101", new string[] { });
            graph.Add("CS102", new string[] { "CS101" });
            graph.Add("CS103", new string[] { "CS102" });
            graph.Add("CS201", new string[] { "CS103" });
            graph.Add("CS202", new string[] { "CS103" });
            graph.Add("CS203", new string[] { "CS103" });

        }
    }

    class ClassNode
    {
        string name;
        string[] prereqs;
        public ClassNode(string name, string[] prereqs)
        {
            this.name = name;
            this.prereqs = prereqs;
        }

        List<ClassNode> preqs = new List<ClassNode>();
    }

    class Graph
    {
        public Graph() 
        {
            classes = new Dictionary<string, ClassNode>();

        }

        private Dictionary<string, ClassNode> classes;

        public void Add(string className, string[] prereqs)
        {
            classes.Add(className, new ClassNode(className, prereqs));
        }

    }
}
