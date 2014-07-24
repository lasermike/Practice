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

            graph.Solve();
            Console.ReadKey();
        }
    }

    class ClassNode
    {
        public string name;
        public string[] prereqs;
        public bool visited = false;
        public int preCount = 0;
        public int postCount = 0;
        public int ccnum = 0; 

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
        private int ccnum;

        public Graph() 
        {
            classes = new Dictionary<string, ClassNode>();
            count = 1;
            ccnum = 0;
        }

        public void Add(string className, string[] prereqs)
        {
            ClassNode existing;
            if (classes.TryGetValue(className, out existing))
            {
                if (existing.prereqs.Length > 0)
                {
                    string[] newArray = new string[existing.prereqs.Length + prereqs.Length];
                    existing.prereqs.CopyTo(newArray, 0);
                    prereqs.CopyTo(newArray, existing.prereqs.Length);
                    existing.prereqs = newArray;
                }
                else
                    existing.prereqs = prereqs;
            }
            else
            {
                classes.Add(className, new ClassNode(className, prereqs));
            }
        }

        bool AllTaken(string[] prereqs, Dictionary<string,bool> taken)
        {
            foreach (string pre in prereqs)
            {
                bool test;
                if (!taken.TryGetValue(pre, out test) || !test)
                    return false;
            }
            return true;
        }

        public void Solve()
        {
            Graph rGraph = this.ReverseGraph();
            rGraph.DFS();
            ClassNode[] ordered = rGraph.classes.OrderByDescending(item => item.Value.postCount).Select(item => item.Value).ToArray();
            Console.WriteLine(ordered.Print(n => n.postCount.ToString()));

            // Groups
            Console.WriteLine("Groupings");

            Dictionary<string, bool> taken = new Dictionary<string, bool>(ordered.Length);
            int groupNum = 1;

            for (int i = 0; i < ordered.Length;i++)
            {
                List<string> semester = new List<string>();
                semester.Add(ordered[i].name);
                Console.WriteLine(ordered[i].name + "("+groupNum+")");

                while (true)
                {
                    i++;
                    if (i >= ordered.Length || !AllTaken( classes[ordered[i].name].prereqs, taken))
                    {
                        groupNum++;
                        i--;
                        break;
                    }
                    semester.Add(ordered[i].name);
                    Console.WriteLine(ordered[i].name + "(" + groupNum + ")");
                }

                semester.ForEach(cls => taken[cls] = true);
            }
        }

        private Graph ReverseGraph()
        {
            Graph rGraph = new Graph();
            foreach (var cls in classes)
            {
                foreach (var prereq in cls.Value.prereqs)
                {
                    rGraph.Add(prereq, new string[] { cls.Key });
                }
                rGraph.Add(cls.Key, new string[] { } );
            }

            return rGraph;
        }

        private void DFS()
        {
            Stack<string> stack = new Stack<string>();
            foreach (string key in classes.Keys) 
            {
                if (classes[key].visited)
                    continue;

                this.ccnum++;

                stack.Push(key);

                while (stack.Count > 0)
                {
                    string current = stack.Peek();

                    if (!classes[current].visited)
                    {
                        classes[current].visited = true;
                        classes[current].preCount = this.count++;
                        classes[current].ccnum = this.ccnum;
                        foreach (string prereqKey in classes[current].prereqs)
                        {
                            if (!classes[prereqKey].visited)
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
        }


    }

    static class ExtensionClass
    {
        public static string Print(this ClassNode[] nodes, Func<ClassNode, string> f)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var node in nodes)
                sb.Append(node.name + "("+ f(node) +")\n");
            return sb.ToString();
        }
    }
}
