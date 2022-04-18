using _7Wonders;
using System;
using System.Collections.Generic;

namespace _7Wonders
{
    public static class ConsoleHelper
    {
        static ConsoleHelper()
        {
            colourMapping = new Dictionary<Colour, ConsoleColor>
            {
                { Colour.None, ConsoleColor.Black },
                { Colour.Brown, ConsoleColor.DarkYellow },
                { Colour.Gray, ConsoleColor.DarkGray },
                { Colour.Blue, ConsoleColor.DarkBlue },
                { Colour.Yellow, ConsoleColor.Yellow },
                { Colour.Green, ConsoleColor.DarkGreen },
                { Colour.Red, ConsoleColor.Red },
                { Colour.Purple, ConsoleColor.DarkMagenta }
            };
        }

        public static void SetConsoleColours(Colour colour)
        {
            Console.ForegroundColor = GetForegroundColor(colour);
            Console.BackgroundColor = GetBackgroundColor(colour);
        }

        public static void ClearConsoleColours()
        {
            SetConsoleColours(Colour.None);
        }

        public static void WriteCardToConsole(Card card)
        {
            var originalForegroundColor = Console.ForegroundColor;
            var originalBackgroundColor = Console.BackgroundColor;
            SetConsoleColours(card.Colour);
            Console.Write(card.Name);
            if (card.VictoryPoints > 0)
            {
                SetConsoleColours(Colour.Blue);
                Console.Write($" ({card.VictoryPoints})");
            }
            Console.ForegroundColor = originalForegroundColor;
            Console.BackgroundColor = originalBackgroundColor;
        }

        public static void WriteCardsToConsole(IEnumerable<Card> cards)
        {
            if (cards != null)
            {
                bool firstCard = true;
                foreach (var card in cards)
                {
                    if (!firstCard)
                    {
                        Console.Write(", ");
                    }
                    WriteCardToConsole(card);
                    firstCard = false;
                }
            }
            Console.WriteLine();
        }

        public static ConsoleColor GetForegroundColor(Colour colour)
        {
            switch (colour)
            {
                case Colour.None:
                    return ConsoleColor.Gray;

                case Colour.Yellow:
                    return ConsoleColor.Black;

                default:
                    return ConsoleColor.White;
            }
        }

        public static ConsoleColor GetBackgroundColor(Colour colour)
        {
            return colourMapping[colour];
        }

        public static int ReadIntFromConsole(int min, int max)
        {
            int value;
            while (!int.TryParse(Console.ReadLine(), out value) || value < min || value > max)
            {
                Console.WriteLine("Invalid input");
            }
            return value;
        }

        private static readonly IDictionary<Colour, ConsoleColor> colourMapping;
    }
}
