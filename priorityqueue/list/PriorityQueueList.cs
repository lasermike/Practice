using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace List
{
     
    class PriorityQueueList
    {
        private List<Node> _list = new List<Node>();

        public void Add(int priority, int value)
        {
            int index = 0;
            foreach (Node node in _list)
            {
                if (node.Priority < priority)
                {
                    break;
                }
                index++;
            }
            _list.Insert(index, new Node() { Priority = priority, Value = value });
        }

        public Node Pop()
        {
            Node node = null;

            if (_list.Count > 0)
            {
                node = _list[0];
                _list.RemoveAt(0);
            }
            return node;
        }

    }
}

