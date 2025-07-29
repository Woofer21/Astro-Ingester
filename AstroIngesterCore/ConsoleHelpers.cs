namespace AstroIngesterCore
{
    public class ConsoleHelpers
    {

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
    }
}
