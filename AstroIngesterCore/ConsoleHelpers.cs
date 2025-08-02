using System.Diagnostics;

namespace AstroIngesterCore
{
    public class ConsoleHelpers
    {
        private static object _sync = new object();

        private static void Write(string message, bool newLine)
        {
            if (newLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
        }

        public static void Log(string message, bool newLine = true, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Write(message, newLine);
            Console.ResetColor();
        }

        public static void Error(string message, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Write(message, newLine);
            Console.ResetColor();
        }

        public static void Info(string message, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Write(message, newLine);
            Console.ResetColor();
        }

        public static void Muted(string message, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Write(message, newLine);
            Console.ResetColor();
        }

        public static void Success(string message, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Write(message, newLine);
            Console.ResetColor();
        }

        public static void ClearLines(int number)
        {
            for (int i = 0; i < number; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new String(' ', Console.BufferWidth));
            }
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        public static void SyncWrite(string message, bool newLine = true, ConsoleColor color = ConsoleColor.White)
        {
            lock (_sync)
            {
                Log(message, newLine, color);
            }
        }
        public static void SyncWrite(int clearCount, string message, bool newLine = true, ConsoleColor color = ConsoleColor.White)
        {
            lock (_sync)
            {
                ClearLines(clearCount);
                Log(message, newLine, color);
            }
        }
    }
}
