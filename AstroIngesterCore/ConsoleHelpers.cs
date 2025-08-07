using System.Text.RegularExpressions;

namespace AstroIngesterCore
{
    public class ConsoleHelpers
    {
        private static object _sync = new object();

        private static readonly Dictionary<string, ConsoleColor> _colorMaps = new()
        {
            {"black", ConsoleColor.Black},
            {"darkblue", ConsoleColor.DarkBlue},
            {"darkgreen", ConsoleColor.DarkGreen},
            {"darkcyan", ConsoleColor.DarkCyan},
            {"darkred", ConsoleColor.DarkRed},
            {"darkmagenta", ConsoleColor.DarkMagenta},
            {"darkyellow", ConsoleColor.DarkYellow},
            {"gray", ConsoleColor.Gray},
            {"darkgray", ConsoleColor.DarkGray},
            {"blue", ConsoleColor.Blue},
            {"green", ConsoleColor.Green},
            {"cyan", ConsoleColor.Cyan},
            {"red", ConsoleColor.Red},
            {"magenta", ConsoleColor.Magenta},
            {"yellow", ConsoleColor.Yellow},
            {"white", ConsoleColor.White},
            {"info", ConsoleColor.Blue},
            {"success", ConsoleColor.Green},
            {"muted", ConsoleColor.DarkGray},
            {"warning", ConsoleColor.Yellow},
            {"error", ConsoleColor.Red},
        };

        private static void Write(string message, bool newLine)
        {
            if (newLine)
                Console.WriteLine(message);
            else
                Console.Write(message);
        }

        private static void Parser(string message, bool newLine, ConsoleColor baseColor)
        {
            //Set base color for the message & get the matches
            Console.ForegroundColor = baseColor;
            MatchCollection matches = Regex.Matches(message, @"\w+\([^)]+.\)");
            Regex firstParenRegex = new(@"\(");
            Regex lastParenRegex = new(@"\)");

            if (matches.Count > 0)
            {
                int captureCount = matches.Count;
                int lastIndx = captureCount - 1;

                // Going through each match that we got
                for (int i = 0; i < captureCount; i++)
                {
                    Capture capture = matches[i];
                    string value = capture.Value;
                    int captureStart = capture.Index;

                    //Split out the color & the actual text content
                    int endOfColorIndx = value.IndexOf('(');
                    string colorCode = value.Substring(0, endOfColorIndx);
                    ConsoleColor partColor = _colorMaps.GetValueOrDefault(colorCode, ConsoleColor.White);

                    string textContent = value.Substring(endOfColorIndx);
                    string coloredText = firstParenRegex.Replace(lastParenRegex.Replace(textContent, "", 1, textContent.Length - 1), "", 1).Trim();


                    if (i == 0)
                    { // First Capture
                        // Print out the text before the first capture
                        Write(message.Substring(0, captureStart), false);

                        // Print out the colored content
                        Console.ForegroundColor = partColor;
                        Write(coloredText, false);
                        Console.ForegroundColor = baseColor;
                    }
                    else
                    { // All other captures
                        // print out the text between captures
                        int prevCaptureEndIndx = matches[i - 1].Index + matches[i - 1].Length;
                        Write(message.Substring(prevCaptureEndIndx, captureStart - prevCaptureEndIndx), false);
                        
                        // Print out the colored content
                        Console.ForegroundColor = partColor; 
                        Write(coloredText, false);
                        Console.ForegroundColor = baseColor;
                    }

                    // Last capture so we need to get the rest of the message
                    if (i == lastIndx)
                        Write(message.Substring(capture.Index + capture.Length), newLine);
                }
            }
            else
                Write(message, newLine);

            //Set Color back to default
            Console.ResetColor();
        }

        public static void Log(string message, bool newLine = true, ConsoleColor baseColor = ConsoleColor.White)
        {
            Parser(message, newLine, baseColor);
        }

        public static void Error(string message, bool newLine = true)
        {
            Parser(message, newLine, _colorMaps["error"]);
            Console.ResetColor();
        }

        public static void Info(string message, bool newLine = true)
        {
            Parser(message, newLine, _colorMaps["info"]);
            Console.ResetColor();
        }

        public static void Muted(string message, bool newLine = true)
        {
            Parser(message, newLine, _colorMaps["muted"]);
            Console.ResetColor();
        }

        public static void Success(string message, bool newLine = true)
        {
            Parser(message, newLine, _colorMaps["success"]);
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
