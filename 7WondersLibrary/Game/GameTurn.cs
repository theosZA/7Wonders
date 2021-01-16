using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// Information about all that happened in a game turn.
    /// </summary>
    public struct GameTurn
    {
        public IReadOnlyDictionary<Player, IAction> playerActions;
        public IReadOnlyCollection<MilitaryResult> militaryResults;

        public void WriteToConsole()
        {
            foreach (var action in playerActions)
            {
                ConsoleHelper.ClearConsoleColours();
                Console.Write($"{action.Key.Name}: ");
                action.Value.WriteToConsole();
            }

            if (militaryResults != null)
            {
                foreach (var result in militaryResults)
                {
                    Console.WriteLine($"{result.winningPlayer.Name} ({result.winningMilitary}) defeats {result.losingPlayer.Name} ({result.losingMilitary}) in battle");
                }
            }
        }
    }
}
