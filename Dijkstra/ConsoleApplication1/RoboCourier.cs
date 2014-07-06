using Heaps;
using System;
using System.Collections;

using Coord = System.Tuple<int, int>;

namespace BasicDijkstra
{
    enum Exit
    {
        Top,
        TopRight,
        BottomRight,
        Bottom,
        BottomLeft,
        TopLeft
    }


    class Hex
    {
        public int x;
        public int y;
        public bool[] scouted;
        public bool visited;

        public Hex(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.scouted = new bool[6];
            visited = false;
        }

        public Coord GetConnection(Exit exit)
        {
            int newX;
            int newY;

            bool odd = this.x % 2 == 1;

            switch (exit)
            {
                case Exit.Top:
                    newX = this.x;
                    newY = this.y + 1;
                    break;
                case Exit.Bottom:
                    newX = this.x;
                    newY = this.y - 1;
                    break;
                case Exit.TopRight:
                    newX = this.x + 1;
                    newY = odd ? this.y : this.y + 1;
                    break;
                case Exit.BottomRight:
                    newX = this.x + 1;
                    newY = odd ? this.y - 1 : this.y;
                    break;
                case Exit.BottomLeft:
                    newX = this.x - 1;
                    newY = odd ? this.y - 1 : this.y;
                    break;
                case Exit.TopLeft:
                    newX = this.x - 1;
                    newY = odd ? this.y : this.y + 1;
                    break;
                default:
                    throw new Exception();
            }
            return new Coord(newX, newY);
        }

    }

    class RoboCourier
    {
        Hex[,] map;
        Hex start;
        Hex end;

        Hex scountCurrent;
        Exit scoutDir;
        
        public RoboCourier(string[] path)
        {
            map = new Hex[100, 100];
            for (int j = 0; j < 100; j++)
            {
                for (int i = 0; i < 100; i++)
                {
                    map[i, j] = new Hex(i, j);
                }
            }
            start = map[10, 10]; //start in the middle;

            scountCurrent = start;
            scoutDir = Exit.Top;
            Console.WriteLine("Scouted: (" + scountCurrent.x.ToString() + ", " + scountCurrent.y.ToString() + ")");

            foreach (string s in path)
            {
                InitializeMap(s);
            }

            end = scountCurrent;

        }

        void InitializeMap(string path)
        {
            foreach (char c in path)
            {
                if (c == 'F')
                {
                    Tuple<int, int> coord = scountCurrent.GetConnection(scoutDir);
                    Hex next = map[coord.Item1, coord.Item2];
                    scountCurrent.scouted[(int)scoutDir] = true;
                    next.scouted[(int) ( ((int)scoutDir) >= 3 ? scoutDir - 3 : scoutDir + 3)] = true;
                    scountCurrent = next;
                    Console.WriteLine("Scouted: (" + coord.Item1.ToString() + ", " + coord.Item2.ToString() + ")");
                }
                else if (c == 'L')
                {
                    scoutDir--;
                    if ((int)scoutDir == -1)
                        scoutDir = Exit.TopLeft;
                }
                else if (c == 'R')
                { 
                    scoutDir++;
                    if ((int)scoutDir == 6)
                        scoutDir = Exit.Top;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        class HexHeapNode : IPriorityQueueNode
        {
            public HexHeapNode(Hex hex, Exit dir, bool turned, string costByStep, int cost, string move, bool momentum)
            {
                this.hex = hex;
                this.courierDir = dir;
                this.turned = turned;
                this.costByStep = costByStep;
                this.cost = cost;
                this.move = move;
                this.momentum = momentum;
            }

            public int Cost() { return cost; }

            public Hex hex;
            public Exit courierDir;
            public bool turned;
            public string costByStep;
            public int cost;
            public string move;
            public bool momentum;
        }

        public int timeToDeliver()
        {
            //PriorityQueueHeap<HexHeapNode> heap = new PriorityQueueHeap<HexHeapNode>(50);
            PriorityQueueList<HexHeapNode> heap = new PriorityQueueList<HexHeapNode>();

            heap.Add(new HexHeapNode(start, Exit.Top, true, "", 0, "", false));

            while (!heap.IsEmpty())
            {
                HexHeapNode node = heap.Top();
                heap.Pop();

                if (node.hex == end)
                {
                    int finalCost = node.Cost() + (node.momentum ? 2 : 0);
                    Console.WriteLine("Path: " + node.move);
                    Console.WriteLine("Path: " + node.costByStep);
                    return finalCost; // DONE!
                }


                Console.WriteLine("Node (" + node.hex.x + ", " + node.hex.y + ") cost: " + node.cost + "  turned:" + node.turned + " dir:" + node.courierDir);

                for (int i = 0; i < 6; i++) // For each path out of this hex
                {
                    Coord coord = node.hex.GetConnection((Exit)i);
                    Hex next = map[coord.Item1, coord.Item2];
                    if (node.hex.scouted[i] && !next.visited) // can move into this square
                    {
                        string move = "";
                        int turnCost = Math.Abs((int) node.courierDir - i);
                        if (turnCost == 5) turnCost = 1;
                        else if (turnCost == 4) turnCost = 2;

                        char oneMove = (node.courierDir - i > 0) ? 'L' : 'R';
                        string costByStep = node.costByStep;
                        for (int t = 0; t < turnCost; t++)
                        {
                            move = move + oneMove;
                            costByStep = costByStep + " ";
                        }
                        move = move + "F";

                        turnCost *= 3;

                        int moveCost = (turnCost != 0 ||  node.hex == start) ? 4 : 2; // Not turing and did not turn last time
                        bool momentum = moveCost == 2;
                        moveCost += (node.momentum && turnCost != 0) ? 2 : 0;   // Turning but was using momentum last turn
                        moveCost += turnCost;

                        if (moveCost >= 10)
                            move = move + " ";
                        heap.Add(new HexHeapNode(next, (Exit)i, turnCost != 0, /*cost by step*/ costByStep + moveCost.ToString(), node.Cost() + moveCost, node.move + move, momentum));
                    }
                }

                node.hex.visited = true;

            }

            Console.WriteLine("No solution!!!");

            return 0;

        }

    }



}