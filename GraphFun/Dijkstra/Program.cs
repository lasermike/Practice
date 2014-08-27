using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicDijkstra;

namespace ConsoleApplication1
{



    class Program
    {


        static void Main(string[] args)
        {
            //RunBasicDijkstra();

//            string[] path = new string[] { "FRRFLLFLLFRRFLF" };
            //string[] path = new string[] { "RFLLF" };
            //string[] path = new string[] { "FLFRRFRFRRFLLFRRF" };
            string[] path = new string[] { 

//                "FRRFLLFLLFRRFLF"

//                "RFLLF"

//                "FLFRRFRFRRFLLFRRF"

//                "FFFFFFFFFRRFFFFFFRRFFFFF", "FLLFFFFFFLLFFFFFFRRFFFF" 

//                   "RFLLFLFLFRFRRFFFRFFRFFRRFLFFRLRRFFLFFLFLLFRFLFLRFF",   "RFFLFLFFRFFLLFLLFRFRFLRLFLRRFLRFLFFLFFFLFLFFRLFRLF",   "LLFLFLRLRRFLFLFRLFRF"

"LLFLFRLRRLRFFLRRRRFFFLRFFRRRLLFLFLLRLRFFLFRRFFFLFL",
  "RLFFRRLRLRRFFFLLLRFRLLRFFLFRLFRRFRRRFRLRLRLFFLLFLF",
  "FRFLRFRRLLLRFFRRRLRFLFRRFLFFRLFLFLFRLLLLFRLLRFLLLF",
  "FFLFRFRRFLLFFLLLFFRLLFLRRFRLFFFRRFFFLLRFFLRFRRRLLR",
  "FFFRRLLFLLRLFRRLRLLFFFLFLRFFRLRLLFLRLFFLLFFLLFFFRR",
  "LRFRRFLRRLRRLRFFFLLLLRRLRFFLFRFFRLLRFLFRRFLFLFFLFR",
  "RFRRLRRFLFFFLLRFLFRRFRFLRLRLLLLFLFFFLFRLLRFRLFRLFR",
  "LLFLFRLFFFFFFFRRLRLRLLRFLRLRRRRRRRRLFLFLFLRFLFRLFF",
  "RLFRRLLRRRRFFFRRRLLLLRRLFFLLLLLRFFFFRFRRLRRRFFFLLF",
  "FFFFLRRLRFLLRRLRLRFRRRRLFLLRFLRRFFFRFRLFFRLLFFRRLL" 

            };
            RoboCourier robo = new RoboCourier(path);
            
            Console.WriteLine("Shortest path: " + robo.timeToDeliver().ToString());
            Console.ReadKey();
        }

        static void RunBasicDijkstra()
        {
            Graph graph = new Graph();
            graph.AddConnection(0, 6, 1);
            graph.AddConnection(6, 7, 1);
            graph.AddConnection(7, 8, 1);
            graph.AddConnection(8, 9, 1);
            graph.AddConnection(9, 4, 1);

            graph.AddConnection(0, 4, 9);
            graph.AddConnection(0, 1, 3);
            graph.AddConnection(0, 2, 5);

            graph.AddConnection(1, 4, 4);
            graph.AddConnection(1, 0, 1);

            graph.AddConnection(2, 4, 9);

            graph.AddConnection(1, 4, 9);
            
            graph.ShowAdjMatrix();

            int cost = graph.Dijkstra(0, 4);
            Console.WriteLine("cost: " + (cost == int.MaxValue ? "NO PATH" : cost.ToString()) );
            Console.ReadKey();
        }
    }
}
