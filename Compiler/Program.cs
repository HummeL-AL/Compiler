using System;
using System.Collections.Generic;

namespace Compiler
{
    class Program
    {
        public static Dictionary<string, int> commands = new Dictionary<string, int>();

        static void Main(string[] args)
        {
            List<string> inputCode = new List<string>();

            SetCommands();

            for (; ; )
            {
                string inputString = Console.ReadLine();
                if (!inputString.Contains("End"))
                {
                    inputCode.Add(inputString);
                }
                else
                {
                    inputCode.Add(inputString);
                    Console.WriteLine("Code accepted");
                    CompileCode(inputCode);
                    Console.WriteLine("Input ended");
                }
            }
        }

        public static void CompileCode(List<string> codeToCompile)
        {
            List<string> preCompiledCode = new List<string>();
            List<string> fullCompiledCode = new List<string>();
            Dictionary<int, string> variableNames = new Dictionary<int, string>();
            Dictionary<int, string> variableValues = new Dictionary<int, string>();
            int memoryByte = 0;
            int endOfCodeByte = 0;

            preCompiledCode.Add("this.memory = Buffer.alloc(RAM_SIZE);");
            preCompiledCode.Add("this.finished = false;");
            preCompiledCode.Add("this.IPPointer = 0;");
            preCompiledCode.Add("this.CommandTable = loadCommandTable();");

            foreach (string line in codeToCompile)
            {
                string[] words = line.Split(' ');

                if (commands.ContainsKey(words[0]))
                {
                    preCompiledCode.Add("this.memory[" + memoryByte + "] = " + commands[words[0]] + ";");
                    memoryByte++;

                    if (words[0] != "End")
                    {
                        preCompiledCode.Add("this.memory.writeInt32LE(" + words[1] + ", " + memoryByte + ");");
                        memoryByte += 4;
                        preCompiledCode.Add("this.memory.writeInt32LE(" + words[2] + ", " + memoryByte + ");");
                        memoryByte += 4;
                    }
                    else
                    {
                        endOfCodeByte = memoryByte;
                    }
                }
                else
                {
                    if (words[0] == "@BIND")
                    {
                        if (words.Length > 2)
                        {
                            variableValues.Add(variableNames.Count + 1, words[2]);
                        }
                        else
                        {
                            variableValues.Add(variableNames.Count + 1, "0");
                        }

                        variableNames.Add(variableNames.Count + 1, words[1]);
                    }
                    else
                    {
                        Console.WriteLine("Ти явно написав якусь Х**НЮ");
                        break;
                    }
                }
            }

            for (int i = 1; i <= variableNames.Count; i++)
            {
                preCompiledCode.Add("this.memory.writeInt32LE(" + variableValues[i] + ", " + memoryByte + "); //" + variableNames[i]);
                memoryByte += 4;
            }

            foreach (string line in preCompiledCode)
            {
                fullCompiledCode.Add(line);
            }

            int kostyl = 0;
            foreach (string line in preCompiledCode)
            {
                for (int i = 1; i <= variableNames.Count; i++)
                {
                    fullCompiledCode[kostyl] = fullCompiledCode[kostyl].Replace(("!" + i.ToString()), (endOfCodeByte + (i-1) * 4).ToString());
                }
                kostyl++;
            }

            foreach(string line in fullCompiledCode)
            {
                Console.WriteLine(line);
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
        }
    }
}
