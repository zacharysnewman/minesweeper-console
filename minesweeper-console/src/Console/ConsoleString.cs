using System;
namespace consoleminesweeper
{
    public struct ConsoleString
    {
        public readonly string text;
        public readonly Color foregroundColor;
        public readonly Color backgroundColor;

        public ConsoleString(string text, Color foregroundColor, Color backgroundColor)
        {
            this.text = text;
            this.foregroundColor = foregroundColor;
            this.backgroundColor = backgroundColor;
        }

        public void Write()
        {
            var prevForegroundColor = Console.ForegroundColor;
            var prevBackgroundColor = Console.BackgroundColor;

            if (this.foregroundColor != Color.None)
                Console.ForegroundColor = (ConsoleColor)(int)this.foregroundColor;
            if (this.backgroundColor != Color.None)
                Console.BackgroundColor = (ConsoleColor)(int)this.backgroundColor;

            Console.Write(this.text);
            Console.ForegroundColor = prevForegroundColor;
            Console.BackgroundColor = prevBackgroundColor;
        }

        public static implicit operator ConsoleString((string, Color, Color) tuple)
        {
            (string text, Color foregroundColor, Color backgroundColor) = tuple;
            return new ConsoleString(text, foregroundColor, backgroundColor);
        }
    }
}
