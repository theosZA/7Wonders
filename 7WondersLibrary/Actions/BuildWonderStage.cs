using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which a player builds the next wonder stage.
    /// </summary>
    internal class BuildWonderStage : IAction
    {
        public BuildWonderStage(WonderStage wonderStage, Card cardToSpend, int coinsToLeftNeighbour = 0, int coinsToRightNeighbour = 0)
        {
            this.wonderStage = wonderStage;
            this.cardToSpend = cardToSpend;
            this.coinsToLeftNeighbour = coinsToLeftNeighbour;
            this.coinsToRightNeighbour = coinsToRightNeighbour;
        }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(cardToSpend);
            actingPlayer.PayCoins(wonderStage.Cost.Coins);
            actingPlayer.PayCoins(coinsToLeftNeighbour);
            leftNeighbour.AddCoins(coinsToLeftNeighbour);
            actingPlayer.PayCoins(coinsToRightNeighbour);
            rightNeighbour.AddCoins(coinsToRightNeighbour);

            actingPlayer.BuildNextWonderStage();
        }

        public void WriteToConsole()
        {
            Console.Write("Build next wonder stage discarding ");
            ConsoleHelper.WriteCardToConsole(cardToSpend);
            Console.Write($" ({wonderStage.Cost})");
            if (coinsToLeftNeighbour > 0 || coinsToRightNeighbour > 0)
            {
                Console.Write(" - paying ");
                if (coinsToLeftNeighbour > 0)
                {
                    Console.Write($"{coinsToLeftNeighbour} {TextHelper.Pluralize("coin", coinsToLeftNeighbour)} to trade left");
                }
                if (coinsToLeftNeighbour > 0 && coinsToRightNeighbour > 0)
                {
                    Console.Write(" and ");
                }
                if (coinsToRightNeighbour > 0)
                {
                    Console.Write($"{coinsToRightNeighbour} {TextHelper.Pluralize("coin", coinsToRightNeighbour)} to trade right");
                }
            }
            Console.WriteLine();
        }

        public bool IsWorseOrEqualThan(BuildWonderStage compareBuild)
        {
            return coinsToLeftNeighbour >= compareBuild.coinsToLeftNeighbour && coinsToRightNeighbour >= compareBuild.coinsToRightNeighbour;
        }

        private WonderStage wonderStage;
        private Card cardToSpend;
        private int coinsToLeftNeighbour;
        private int coinsToRightNeighbour;
    }
}
