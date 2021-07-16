using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Compiler
{
    class Program
    {
        public static Dictionary<string, int> commands = new Dictionary<string, int>();

        public static int numOfVariables = 0;
        public static Dictionary<int, int> variablesValues = new Dictionary<int, int>();

        public static int endOfCode;
        public static int[] buffer;
        public static List<string> toCompile = new List<string>();
        public static string compiled = "";

        static void Main(string[] args)
        {
            string inputString = "";
            SetCommands();

            do {
                inputString = Console.ReadLine();
                string[] words = inputString.Split(" ");

                if(words[0] == "@BIND")
                {
                    if(words.Length > 1)
                    {
                        variablesValues.Add(numOfVariables, Convert.ToInt32(words[1]));
                    }
                    else
                    {
                        variablesValues.Add(numOfVariables, 0);
                    }
                    numOfVariables++;
                }
                else
                {
                    string toAdd = commands[words[0]].ToString();
                    if(words[0] != "End")
                    {
                        toAdd += " " + words[1].ToString() + " " + words[2].ToString();
                    }
                    toCompile.Add(toAdd);
                }
            }
            while (!inputString.Contains("End"));
            endOfCode = (toCompile.Count - 1) * 9;
            Compile();
        }

        static void Compile()
        {
            int position = 0;
            buffer = new int[endOfCode + variablesValues.Count * 4 + 1];

            foreach (string line in toCompile)
            {
                string[] numbers = line.Split(" ");

                buffer[position] = Convert.ToInt32(numbers[0]);
                position++;
                if (numbers.Length > 1)
                {
                    if (numbers[1][0] == '!')
                    {
                        buffer[position] = (endOfCode + 1 + Convert.ToInt32(numbers[1].Replace("!", "")) * 4);
                        position += 4;
                    }
                    else
                    {
                        buffer[position] = (Convert.ToInt32(numbers[1]));
                        position += 4;
                    }

                    if (numbers[2][0] == '!')
                    {
                        buffer[position] = (endOfCode + 1 + Convert.ToInt32(numbers[2].Replace("!", "")) * 4);
                        position += 4;
                    }
                    else
                    {
                        buffer[position] = (Convert.ToInt32(numbers[2]));
                        position += 4;
                    }
                }
            }
            for (int i = 0; i < variablesValues.Count; i++)
            {
                buffer[position] = (variablesValues[i]);
                position += 4;
            }

            foreach (int number in buffer)
            {
                Console.Write("Dec: " + number + " ");
                string hexNumber = number.ToString("X2");
                Console.WriteLine("Hex: " + hexNumber + " ");

                compiled += hexNumber;
            }
            Console.Write("Full: " + compiled);

            string path = Directory.GetCurrentDirectory() + "/main.uce";
            using (FileStream stream = File.Create(path))
            {
                //byte[] info = new UTF8Encoding(true).GetBytes(compiled);
                byte[] info = Enumerable.Range(0, compiled.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(compiled.Substring(x, 2), 16)).ToArray();
                stream.Write(info, 0, info.Length);
            }
            if (!File.Exists(path))
            {
                Console.WriteLine("File wasn't saved.");
            }
        }

        public static void SetCommands()
        {
            commands.Add("Mov", 1);
            commands.Add("Add", 2);
            commands.Add("Sub", 3);
            commands.Add("Mul", 4);
            commands.Add("Div", 5);
            commands.Add("Jmp", 6);
            commands.Add("End", 7);
            commands.Add("Cpm", 8);
            commands.Add("Set", 9);
            commands.Add("Adr", 10);
            commands.Add("Get", 11);
            commands.Add("Sys", 12);
        }
    }
}
