using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triple = System.Tuple<short, short, short>;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            //string msg = "S";
            string msg = "Sally slipped between the sheets";

            RSA rsa = new RSA();
            short[] cypher = rsa.Encrypt(msg);
            Console.WriteLine(rsa.Decrypt(cypher));
            Console.ReadKey();
        }
    }

    class RSA
    {
        private short p; // first prime
        private short q; // second prime
        private short N; // modulous used for encryption/decryption
        private short e; // encrypt key
        private short d = 0; // decrypt key
        private Random random;

        public RSA() 
        {
            random = new Random();

            do
            {
                p = FindPrime();
                do
                {
                    q = FindPrime();
                } while (q == p);

                // Ensure modulous fits within a short.  This could also have been done by choosing 8 bit primes.
                int group = p * q; 
                if (group < short.MaxValue)
                {
                    N = (short)group;
                    break;
                }
                Console.WriteLine("Modulus " + group + " exceeds bit space");
            } while (true);

            Console.WriteLine("p = " + p);
            Console.WriteLine("q = " + q);
            Console.WriteLine("N = " + N);

            // Find encrypt/decrypt keys
            short totient = (short) ((p - 1) * (q - 1));

            Console.WriteLine("totient = " + totient);

            // Find keys
            for (int i = 3; i < totient; i += 2)
            {
                e = (short) i;
                // Find decrypt key as modular inverse of encrypt key mod totient
                d = Inverse(e, totient);

                if (d != 1)  // found valid inverse
                    break;
            }
            short d2 = Inverse2(e, totient); // Used to validate Euclids result
            Console.WriteLine("d2 = " + d2);
            if (d != d2)
                throw new Exception("Euclid algorithm validation error");

            if (d == 0)
            {
                throw new Exception("Could not find decrypt key");
            }

            Console.WriteLine("e = " + e);
            Console.WriteLine("d = " + d);
            short testCrypt = ExponentModulus(83 /* rand */, e, N);
            Console.WriteLine("  validation: pow(83,e) = " + testCrypt);
            short testDecrypt = ExponentModulus(testCrypt, d, N);
            Console.WriteLine("  validation: pow(pow(83,e),d) = " + testDecrypt);
            if (testDecrypt != 83)
                throw new Exception("Error computing keys");

        }

        short Inverse(short a, short n)
        { 
            Triple retTriple = EuclidCoefficients(a, n);
            short retval = retTriple.Item1;
            return (retval < 0) ? (short)(retval + n) : retval;
        }

        Triple EuclidCoefficients(short a, short b)
        {
            if (b == 0)
                return new Triple(1, 0, a);

            Triple ans = EuclidCoefficients(b, (short) (a % b));
            return new Triple(
                ans.Item2,
                (short)(ans.Item1 - Math.Floor(a / (float)b) * ans.Item2),
                ans.Item3);
        }

        short Inverse2(short a, short n)
        {
            short t = 0;
            short newt = 1;    
            short r = n;
            short newr = a;    

            while (newr != 0)
            {
                short quotient = (short) (r / newr);

                short oldt = newt;
                newt = (short) (t - quotient * newt);
                t = oldt;

                short oldr = newr;
                newr = (short) (r - quotient * newr);
                r = oldr;
            }

            if (r > 1) 
            {
                //a is not invertible mod n
                return 0;
            }

            if (t < 0)
            {
                t = (short) (t + n);
            }

            return t;
        }

        private short FindPrime()
        {
            do
            {
                short prime = (short) random.Next(3, short.MaxValue / 2);
                if (IsPrime(prime))
                {
                    return prime;
                }

            } while (true);
        }

        private bool IsPrime(short num)
        {
            short[] primes = new short[] { 2, 3, 5, 7};
            for (int i = 0; i < primes.Length; i++)
            {
                if (!IsRelativelyPrime(primes[i], num))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsRelativelyPrime(short a, short modulus)
        {
            return (ExponentModulus(a, (short) (modulus - 1), modulus)) == 1;
        }

        public short[] Encrypt(string msg)
        {
            short[] ans = new short[msg.Length];

            Console.Write("encrypt: ");
            for (int i = 0; i < msg.Length; i++)
            {
                ans[i] = ExponentModulus((short) msg[i], e, N);
                Console.Write("(" + (short) msg[i] + ", " + ans[i] + "), ");
            }
            Console.WriteLine("");

            return ans;
        }

        public string Decrypt(short[] cypher)
        {
            StringBuilder ans = new StringBuilder(cypher.Length);

            Console.Write("decrypt: ");
            for (int i = 0; i < cypher.Length; i++)
            {
                short retval = ExponentModulus(cypher[i], d, N);
                ans.Append((char)retval);
                Console.Write("(" + cypher[i] + ", " + retval + "), ");
            }
            Console.WriteLine("");

            return ans.ToString();
        }

        private short ExponentModulus(short a, short b, short modulus)
        {
            if (b == 0)
                return 1;

            short z = ExponentModulus(a, (short) Math.Floor(b / 2.0f), modulus);

            if (b % 2 == 0)
                return (short) ((z * z) % modulus);
            else
                return (short) ( ( ( (z * z) % modulus) * a) % modulus);
        }
    }
}

