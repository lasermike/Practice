// Finds random prime numbers

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
                // Find a random 64 bit number
                Int64 num = ( random.Next(Int32.MaxValue) << 32 ) | random.Next(Int32.MaxValue) ;

                // Test if prime
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

        /// <summary>
        /// Tests if given number A is prime by raising a known prime to the power of A - 1 mod A.
        /// Result should be 1 if the given number is prime.  Not completely accurate so we do it with a few known primes to be sure.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Raises the given number A to the exponent EXP modulus MOD.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="exp"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
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
