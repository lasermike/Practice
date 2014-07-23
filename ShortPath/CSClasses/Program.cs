// Algorithms 3.16

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
            graph.Add("CS102", new string[] {  });
            graph.Add("CS103", new string[] { });
            graph.Add("CS201", new string[] { "CS101" });
            graph.Add("CS202", new string[] { "CS102", "CS201" });
            graph.Add("CS203", new string[] { "CS103", "CS202" });
            graph.Add("CS301", new string[] { "CS201", "CS203" });
            graph.Add("CS302", new string[] { "CS203" });
            graph.Add("CS401", new string[] { "CS302" });
            graph.Add("CS402", new string[] { "CS302" });
            graph.Add("CS403", new string[] { "CS302" });
            graph.Add("CS500", new string[] { "CS402", "CS403" } );
        }
    }

    class ClassNode
    {
        public string name;
        public string[] prereqs;
        public bool visited = false;
        public int preCount = 0;
        public int postCount = 0; 

        public ClassNode(string name, string[] prereqs)
        {
            this.name = name;
            this.prereqs = prereqs;
        }
    }

    class Graph
    {
        private Dictionary<string, ClassNode> classes;
        private int count;
        public Graph() 
        {
            classes = new Dictionary<string, ClassNode>();
            count = 0;
        }


        public void Add(string className, string[] prereqs)
        {
            classes.Add(className, new ClassNode(className, prereqs));
        }

        public string Solve()
        {
            Stack<string> stack = new Stack<string>();
            foreach (string key in classes.Keys) 
            {
                if (classes[key].visited)
                    continue;

                stack.Push(key);

                while (stack.Count > 0)
                {
                    string current = stack.Peek();

                    if (!classes[current].visited)
                    {
                        classes[current].visited = true;
                        classes[current].preCount = this.count++;
                        foreach (string prereqKey in classes[current].prereqs)
                        {
                            if (!classes[current].visited)
                                stack.Push(prereqKey);
                        }
                    }
                    else
                    {
                        stack.Pop();
                        classes[current].postCount = this.count++;
                    }
                }
                
            }

            return "";
        }


    }
}
