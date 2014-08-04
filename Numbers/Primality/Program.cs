using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primality
{
    class Program
    {
        static void Main(string[] args)
        {
            Primes.TestPrimality(24);

            Primes primes = new Primes();
            primes.Find();

            Console.ReadKey();
        }
    }

    class Primes
    {
        private static int[] primes = new int[] { 2, 3, 5 };
        Random random = new Random();

        public Primes()
        {
        }

        public Int64 Find()
        {
            do
            {
                Int64 num = ( random.Next(Int32.MaxValue) << 32 ) | random.Next(Int32.MaxValue) ;
                if (Primes.TestPrimality(num))
                {
                    Console.WriteLine(num + " is prime ("+Convert.ToString(num, 2)+")");
                    return num;
                }
                else
                {
                    //Console.WriteLine(num + " is not prime");
                }

            } while (true);
        }

        public static bool TestPrimality(Int64 a)
        {
            bool isPrime = true;

            foreach (int prime in primes)
            {
                Int64 ans = Primes.ExpontentMod(prime, a - 1, a);
                if (ans != 1)
                {
                    isPrime = false;
                    break;
                }
            }
            //Console.WriteLine(a.ToString() + " is " + (isPrime ? " prime " : "not prime") );
            return isPrime;
        }

        private static Int64 ExpontentMod(Int64 a, Int64 exp, Int64 mod)
        {
            if (exp == 0)
                return 1;

            Int64 ans = ExpontentMod(a, (Int64)Math.Floor(exp / 2.0f), mod);
            if (exp % 2 == 1)
                return (a * ans * ans) % mod;
            else
                return (ans * ans) % mod;
        }
    }
}
