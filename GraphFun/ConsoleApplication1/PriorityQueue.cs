
using System;
using System.Collections;
namespace Heaps
{
    interface IPriorityQueueNode
    {
        int Cost();
    }

    class PriorityQueueHeap<T> where T : IPriorityQueueNode
    {
        int maxSize = 0;
        int currentSize = 0;
        T[] queue;

        public PriorityQueueHeap(int size)
        {
            maxSize = size;
            queue = new T[maxSize];
        }


        public void Add(T node)
        {
            queue[currentSize] = node;
            currentSize++;
            SiftUp();
        }

        public T Top()
        {
            if (IsEmpty()) throw new ArgumentOutOfRangeException();
            return queue[0];
        }

        public void Pop()
        {
            if (IsEmpty())
                throw new ArgumentOutOfRangeException();

            queue[0] = queue[--currentSize];
            SiftDown();
        }

        public bool IsEmpty()
        {
            return currentSize == 0;
        }

        int Parent(int node) { return node / 2; }
        int Left(int node) { return node * 2 + 1; }
        int Right(int node) { return node * 2 + 2; }

        void SiftDown()
        {
            int current = 0;

            while (current < currentSize - 1)
            {
                if (Left(current) == currentSize - 1)
                {
                    if (queue[current].Cost() > queue[Left(current)].Cost())
                    {
                        Swap(current, Left(current));
                    }
                    break; // At the end of the tree
                }
                else if (Right(currentSize) < currentSize)
                {
                    if (Right(current) < Left(current))
                    {
                        if (queue[current].Cost() > queue[Right(current)].Cost())
                        {
                            Swap(current, Right(current));
                        }
                        current = Right(current);
                    }
                    else if (Left(current) < currentSize)
                    {
                        if (queue[current].Cost() > queue[Left(current)].Cost())
                        {
                            Swap(current, Left(current));
                        }
                        current = Left(current);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        void SiftUp()
        {
            int current = currentSize - 1;

            while (current > 0)
            {
                if (queue[current].Cost() < queue[Parent(current)].Cost())
                {
                    Swap(current, Parent(current));
                }
                current = Parent(current);
            }
        }

        void Swap(int i, int j)
        {
            T temp = queue[i];
            queue[i] = queue[j];
            queue[j] = temp;

        }
    }


    class PriorityQueueList<T> where T : IPriorityQueueNode
    {
        ArrayList list = new ArrayList();

        public void Add(T node)
        {
            int i;
            for (i = 0; i < list.Count; i++)
            {
                if (node.Cost() < ((T)list[i]).Cost())
                {
                    break;
                }
            }
            list.Insert(i, node);
        }
        public T Top()
        {
            return (T)list[0];
        }

        public void Pop()
        {
            list.RemoveAt(0);
        }

        public bool IsEmpty()
        {
            return list.Count == 0;
        }
    }
}
