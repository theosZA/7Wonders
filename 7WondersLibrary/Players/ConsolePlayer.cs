using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    internal class ConsolePlayer : Player
    {
        public ConsolePlayer(string name, Tableau tableau) : base(name, tableau)
        {}

        override protected IAction GetAction(IEnumerable<IAction> possibleActions)
        {
            WriteStateToConsole();

            var actions = possibleActions.ToList();
            for (int actionIndex = 0; actionIndex < actions.Count; ++actionIndex)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write($"{actionIndex + 1}: ");
                actions.ElementAt(actionIndex).WriteToConsole();
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            var pick = ConsoleHelper.ReadIntFromConsole(1, actions.Count);
            return actions.ElementAt(pick - 1);
        }
    }
}
