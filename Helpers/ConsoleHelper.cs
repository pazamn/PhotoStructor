using System;

namespace PhotoStructurer.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void WriteLine(string message, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            Console.WriteLine(message);

            Console.ForegroundColor = consoleColor;
        }
    }
}