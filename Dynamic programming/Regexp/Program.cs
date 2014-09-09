using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regexp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Supported special chars
            // *   .    /d    

            Reg reg = new Reg(AlgorithmType.Recursive);
            bool match;

            match = reg.Eval(@"\d\d\d-\d\d\d-\d\d\d\d", "206-111-2222");
            Check(match, true, reg.stream, reg.pattern);

            match = reg.Eval(@"\d\d\d-\d\d\d-\d\d\d\d", "2067-111-2222");
            Check(match, false, reg.stream, reg.pattern);

            match = reg.Eval(@"\d\d\d-\d*-\d.\d\d", "206-111-2222");
            Check(match, true, reg.stream, reg.pattern);

            match = reg.Eval(@"\d*_..\d\d", "123_xy45");
            Check(match, true, reg.stream, reg.pattern);

            match = reg.Eval(@"\d*_..\d\d", "_xy45");
            Check(match, true, reg.stream, reg.pattern);

            match = reg.Eval(@"\dxyz\d*", "1xyz");
            Check(match, true, reg.stream, reg.pattern);


            Console.ReadKey();
        }

        static void Check(bool result, bool expected, string stream, string pattern)
        {
            string text = "string '" + stream + "' with expression '" + pattern + "' " + (result ? "does" : "does not") + " match\n";
            Console.WriteLine(text);
            if (result != expected)
                throw new Exception(text);
        }

        enum AlgorithmType
        {
            Dynamic,
            Recursive
        };

        /// <summary>
        /// Evaluates simple regular expressions
        /// </summary>
        class Reg
        {
            // Cached for easy display at the end
            public string stream;
            public string pattern;
            private AlgorithmType algorithmType;

            public Reg(AlgorithmType algorithmType) { this.algorithmType = algorithmType; }

            /// <summary>
            /// Types of elements that can be matched
            /// </summary>

            public bool Eval(string exp, string str)
            {
                stream = str;
                pattern = exp;

                MatchNode[] format = MatchNode.Generate(exp);

                if (algorithmType == AlgorithmType.Recursive)
                    return EvalRecursive(format, str);
                else
                    return EvalDynamic(format, str);
            }

            private bool EvalRecursive(MatchNode[] format, string str)
            {
                if (str.Length == 0 && format.Length == 0)
                    return true;

                // Handle case of 'repeat zero or more times element' at the end of the format string
                if (str.Length == 0 && format[0].repeated)
                {
                    return EvalRecursive(format.Skip(1).ToArray(), str);
                }

                // Does the current first character match the current first pattern element?
                if (str.Length > 0 && format.Length > 0)
                {
                    if (DoesCharacterMatch(str[0], format[0]))
                    {
                        // Check next character with next pattern element
                        if (EvalRecursive(format.Skip(1).ToArray(), str.Substring(1).ToString()))
                            return true;

                        if (format[0].repeated)
                        {
                            // Check next character with the current pattern element
                            if (EvalRecursive(format, str.Substring(1).ToString()))
                                return true;
                        }
                    }
                    else if (format[0].repeated)
                    {
                        // Repeated can be zero matches, so try just advancing the pattern
                        return EvalRecursive(format.Skip(1).ToArray(), str);
                    }
                }
                return false;
            }

            private bool EvalDynamic(MatchNode[] format, string str)
            {
                // Initial the match table.  We only need to init the left and top edge because we fill in the rest dynamically
                bool[,] match = new bool[str.Length + 1, format.Length + 1];
                for (int i = 0; i < str.Length + 1; i++)
                    match[i,0] = false;
                for (int j = 0; j < format.Length + 1; j++)
                    match[0,j] = false;
                match[0, 0] = true; // Assume a zero length string will match a zero length pattern

                for (int j = 1; j < format.Length + 1; j++)
                {
                    for (int i = 1; i < str.Length + 1; i++)
                    {
                        // Is there a direct or '.' character match?
                        if (match[i - 1, j - 1] && DoesCharacterMatch(str[i - 1], format[j - 1]))
                            match[i , j] = true;

                        // Did the previous character match, and it is a repeating pattern, and the current character also matches it?
                        if (match[i - 1, j] && DoesCharacterMatch(str[i - 1], format[j - 1]) && format[j - 1].repeated)
                            match[i, j] = true;

                        // Handle case were '*' matches zero elements
                        if (j > 1 && match[i - 1, j - 2] && format[j - 2].repeated)
                            match[i, j] = true;

                    }
                }

                // Write out the match table for debugging purposes
                Console.Write("    ");
                for (int i = 0; i < str.Length; i++)
                {
                    Console.Write(str[i] + " ");
                }
                Console.WriteLine();
                for (int j = 0; j < format.Length + 1; j++)
                {
                    Console.Write((j > 0 ? format[j-1].value.ToString() : " ") + " ");
                    for (int i = 0; i < str.Length + 1; i++)
                    {
                        Console.Write((match[i,j] ? "1" : "0") + " ");
                    }
                    Console.WriteLine();
                }

                return match[str.Length, format.Length];
            }

            /// <summary>
            /// Determines if the given character matches the given node
            /// </summary>
            private bool DoesCharacterMatch(char character, MatchNode matchClass) 
            {
                switch (matchClass.type)
                {
                    case CharClass.Digit:
                        if (character >= '0' && character <= '9')
                            return true;
                        break;
                    case CharClass.Literal:
                        if (character == matchClass.value)
                            return true;
                        break;
                    case CharClass.Any:
                        return true;
                }

                return false;
            }

            enum CharClass
            {
                Init = 0,
                Literal = 1,
                Digit = 2,
                Any = 3
            };

            /// <summary>
            /// A single character class to be matched.
            /// </summary>
            class MatchNode
            {
                public CharClass type = CharClass.Init;
                public char value; // If literal
                public bool repeated = false;

                private MatchNode(CharClass type, char value, bool repeated)
                {
                    this.type = type;
                    this.value = value;
                    this.repeated = repeated;
                }

                private MatchNode(CharClass type) { this.type = type; }
                private MatchNode(CharClass type, char value) { this.type = type; this.value = value; }

                /// <summary>
                /// Creates an array of elements of a pattern to be matched
                /// </summary>
                public static MatchNode[] Generate(string exp)
                {
                    List<MatchNode> nodes = new List<MatchNode>();
                    for (int i = 0; i < exp.Length; i++)
                    {
                        MatchNode node;
                        if (exp[i] == '\\')
                        {
                            if (exp[++i] == 'd')
                                node = new MatchNode(CharClass.Digit, 'd');
                            else
                                throw new Exception("Unknown character class");
                        }
                        else if (exp[i] == '.')
                            node = new MatchNode(CharClass.Any, '.');
                        else
                            node = new MatchNode(CharClass.Literal, exp[i]);

                        if (i < exp.Length - 1 && exp[i + 1] == '*')
                        {
                            node.repeated = true;
                            i++;
                        }

                        nodes.Add(node);
                    }

                    return nodes.ToArray();
                }
            }
        }
    }
}
