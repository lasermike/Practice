using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace List
{
    class PriorityQueueHeap
    {
        private Node[] _nodes = new Node[1];
        int _numNodes = 0;

        public void Add(int priority, int value)
        {
            if (_nodes.Length < _numNodes + 1)
            {
                Array.Resize<Node>(ref _nodes, _nodes.Length * 2);
            }

            _nodes[_numNodes] = new Node() { Priority = priority, Value = value };
            _numNodes++;
            SiftUp();
        }

        public Node Pop()
        {
            Node node = null;
            if (_numNodes > 0)
            {
                node = _nodes[0];
                Swap(0, _numNodes - 1);
                _numNodes--;
                SiftDown(0);
            }
            return node;
        }

        private int Parent(int i) { return (i - 1) / 2; }
        private int Left(int i) { return i * 2 + 1; }
        private int Right(int i) { return i * 2 + 2; }

        private void Swap(int i, int j)
        {
            Node node = _nodes[i];
            _nodes[i] = _nodes[j];
            _nodes[j] = node;
        }

        private void SiftUp()
        {
            SiftUp(_numNodes - 1);
        }

        private void SiftDown(int index)
        {
            int left = Left(index);
            int right = Right(index);

            if (right < _numNodes && left < _numNodes )
            {
                if (_nodes[left].Priority > _nodes[right].Priority && _nodes[left].Priority > _nodes[index].Priority)
                {
                    Swap(index, left);
                    SiftDown(left);
                }
                else if (_nodes[right].Priority > _nodes[left].Priority && _nodes[right].Priority > _nodes[index].Priority)
                {
                    Swap(index, right);
                    SiftDown(right);
                }
            }
            else if (left < _numNodes && _nodes[left].Priority > _nodes[index].Priority)
            {
                Swap(index, left);
                SiftDown(left);
            }

        }

        private void SiftUp(int index)
        {
            int parent = Parent(index);
            if (parent >= 0)
            {
                if (_nodes[parent].Priority < _nodes[index].Priority)
                {
                    Swap(index, parent);
                    SiftUp(parent);
                }
            }
        }
    }

}
