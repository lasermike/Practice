using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triple = System.Tuple<int, int, int>;

namespace GCD
{
    class Program
    {
        static void Main(string[] args)
        {
            GCD gcd = new GCD();

            gcd.Compute(100, 64);
            gcd.ComputeCofficients(100, 64);
            Console.ReadKey();
        }
    }

    class GCD
    {
        public GCD()
        { }

        public int Compute(int a, int b)
        {
            int retval = Euclid(a, b);
            Console.WriteLine("GCD of "+a+" and "+b+" is: " + retval);
            return retval;
        }
        private int Euclid(int a, int b)
        {
            if (b == 0)
                return a;
            return Euclid(b, a % b);
        }


        public int ComputeCofficients(int a, int b)
        {
            Triple retval = EuclidCoefficients(a, b);
            Console.WriteLine("GCD of " + a + " and " + b + " is: " + retval.Item3 + " with cofficients " + retval.Item1 + " and " + retval.Item2);
            return retval.Item3;

        }

        private Triple EuclidCoefficients(int a, int b)
        {
            if (b == 0)
                return new Triple(1, 0, a);
            Triple retval = EuclidCoefficients(b, a % b);
            return new Triple(retval.Item2,
                              retval.Item1 - (int) Math.Floor((float) a / (float)b) * retval.Item2, 
                              retval.Item3);
        }
    }
}
