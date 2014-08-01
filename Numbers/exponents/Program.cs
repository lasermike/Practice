using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exponents
{
    class Program
    {
        static void Main(string[] args)
        {
            //1.11. Is pow(4,1536) - pow(9,4824) divisible by 35?

            int a_ans = Maths.ExponentModulus(4, 1536, 35);
            int b_ans = Maths.ExponentModulus(9, 4824, 35);

            Console.WriteLine("answer: " +  (a_ans == 1 && b_ans == 1).ToString());
            Console.ReadKey();
        }

    }

    class Maths
    {
        public static int ExponentModulus(int a, int b, int modulus)
        {
            if (b == 0)
                return 1;

            int val = ExponentModulus(a, (int) Math.Floor( b / 2.0f), modulus);

            if (b % 2 == 1)
                return (a * val * val) % 35;
            else
                return (val * val) % 35;
        }
    }
}
