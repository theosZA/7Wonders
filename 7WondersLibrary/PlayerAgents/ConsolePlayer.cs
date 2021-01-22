using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    public class ConsolePlayer : PlayerAgent
    {
        public string Name { get; }

        public ConsolePlayer(string name)
        {
            Name = name;
        }

        public IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand)
        {
            var actingPlayer = playerStates[0];
            var leftNeighbour = playerStates[1];
            var rightNeighbour = playerStates[playerStates.Count - 1];

            ConsoleHelper.ClearConsoleColours();
            Console.WriteLine(Name);
            Console.WriteLine();

            actingPlayer.WriteStateToConsole(hand, leftNeighbour, rightNeighbour);
            Console.WriteLine();

            ConsoleHelper.WriteCardsToConsole(hand);
            Console.WriteLine();

            var actions = actingPlayer.GetAllActions(hand, leftNeighbour, rightNeighbour).ToList();
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
