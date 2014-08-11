using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "Sally slipped between the sheets";

            RSA rsa = new RSA();
            Int16[] cypher = rsa.Encrypt(msg);
            Console.WriteLine(rsa.Decrypt(cypher));
            Console.ReadKey();
        }
    }

    class RSA
    {
        private byte p;
        private byte q;
        private Int16 N = 6000;
        private Int16 e = 1;
        private Int16 d = 1;

        public RSA() 
        {
            p = FindPrime();
            q = FindPrime();

        }

        byte FindPrime()
        {
            
        }

        public Int16[] Encrypt(string msg)
        {
            Int16[] ans = new Int16[msg.Length];

            for (int i = 0; i < msg.Length; i++)
            {
                ans[i] = ExponentModulus((Int16) msg[i], e, N);
            }

            return ans;
        }

        public string Decrypt(Int16[] cypher)
        {
            StringBuilder ans = new StringBuilder(cypher.Length);

            for (int i = 0; i < cypher.Length; i++)
            {
                ans.Append((char) ExponentModulus(cypher[i], e, N) );
            }

            return ans.ToString();
        }

        private Int16 ExponentModulus(Int16 a, Int16 b, Int16 modulus)
        {
            if (b == 0)
                return 1;

            Int16 z = ExponentModulus(a, (Int16) Math.Floor(b / 2.0f), modulus);

            if (b % 2 == 0)
                return (short) ((z * z) % modulus);
            else
                return (short) ((z * z * a) % modulus);
        }
    }
}

