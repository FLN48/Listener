using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener
{
    internal static class Helpers
    {
        public static ConsoleInfo consoleInfo = new ConsoleInfo();
        static int outCol, outRow, outHeight = 1000;
        public static IEnumerable<double> MovingAverage(this IEnumerable<double> source, int windowSize)
        {
            var queue = new Queue<double>(windowSize);

            foreach (double d in source)
            {
                if (queue.Count == windowSize)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(d);
                yield return queue.Average();
            }
        }

        public static void WriteOut(string msg, bool appendNewLine = false) 
        {
            int inCol, inRow;
            inCol = Console.CursorLeft;
            inRow = Console.CursorTop;

            int outLines = getMsgRowCount(outCol, msg) + (appendNewLine ? 1 : 0);
            int outBottom = outRow + outLines;
            if (outBottom > outHeight)
                outBottom = outHeight;
            if (inRow <= outBottom)
            {
                int scrollCount = outBottom - inRow + 1;
                Console.MoveBufferArea(0, inRow, Console.BufferWidth, 1, 0, inRow + scrollCount);
                inRow += scrollCount;
            }
            if (outRow + outLines > outHeight)
            {
                int scrollCount = outRow + outLines - outHeight;
                Console.MoveBufferArea(0, scrollCount, Console.BufferWidth, outHeight - scrollCount, 0, 0);
                outRow -= scrollCount;
                Console.SetCursorPosition(outCol, outRow);
            }
            Console.SetCursorPosition(outCol, outRow);
            if (appendNewLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);
            outCol = Console.CursorLeft;
            outRow = Console.CursorTop;
            Console.SetCursorPosition(inCol, inRow);
        }

        public static int getMsgRowCount(int startCol, string msg)
        {
            string[] lines = msg.Split('\n');
            int result = 0;
            foreach (string line in lines)
            {
                result += (startCol + line.Length) / Console.BufferWidth;
                startCol = 0;
            }
            return result + lines.Length - 1;
        }
        static void ConsoleWriter()
        {
            while (true)
            {
                lock (consoleInfo)
                {
                    Console.Clear();

                    if (consoleInfo.outputBuffer[0].Length > 20)
                    {
                        consoleInfo.outputBuffer[0] = "Running.";
                    }
                    else
                    {
                        consoleInfo.outputBuffer[0] += ".";
                    }

                    foreach (var item in consoleInfo.outputBuffer)
                    {
                        Console.WriteLine(item);
                    }

                    Console.WriteLine("--------------------------------------------------------------");

                    if (consoleInfo.commandReaty)
                    {
                        consoleInfo.commandReaty = false;
                        consoleInfo.lastCommand = consoleInfo.sbRead.ToString();
                        consoleInfo.sbRead.Clear();
                        consoleInfo.lastResult.Clear();
                        switch (consoleInfo.lastCommand)
                        {
                            case "command1":
                                consoleInfo.outputBuffer[2] = ".. status of bar = good";
                                consoleInfo.lastResult.Append("command #1 performed");

                                break;
                            case "command2":
                                consoleInfo.outputBuffer[2] = ".. status of bar = bad";
                                consoleInfo.lastResult.Append("command #2 performed");
                                break;
                            case "?":
                                consoleInfo.lastResult.AppendLine("Available commands are:");
                                consoleInfo.lastResult.AppendLine("command1     sets bar to good");
                                consoleInfo.lastResult.AppendLine("command1     sets bar to bad");
                                break;
                            default:
                                consoleInfo.lastResult.Append("invalid command, type ? to see command list");
                                break;
                        }
                    }

                    Console.WriteLine(consoleInfo.lastCommand);
                    Console.WriteLine(consoleInfo.lastResult);
                    Console.WriteLine();
                    Console.Write(">");
                    Console.WriteLine(consoleInfo.sbRead.ToString());
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                }

                Thread.Sleep(250);
            }
        }
        public class ConsoleInfo
        {
            public bool commandReaty { get; set; }
            public StringBuilder sbRead { get; set; }
            public List<string> outputBuffer { get; set; }
            public string lastCommand { get; set; }
            public StringBuilder lastResult { get; set; }

            public ConsoleInfo()
            {
                sbRead = new StringBuilder();
                outputBuffer = new List<string>();
                commandReaty = false;
                lastResult = new StringBuilder();
            }
        }
    }
}
