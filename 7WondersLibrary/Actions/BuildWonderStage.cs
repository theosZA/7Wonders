using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which a player builds the next wonder stage.
    /// </summary>
    internal class BuildWonderStage : IAction
    {
        public BuildWonderStage(WonderStage wonderStage, Card cardToDiscard, int coinsToLeftNeighbour = 0, int coinsToRightNeighbour = 0)
        {
            this.wonderStage = wonderStage;
            this.cardToDiscard = cardToDiscard;
            this.coinsToLeftNeighbour = coinsToLeftNeighbour;
            this.coinsToRightNeighbour = coinsToRightNeighbour;
        }

        public void Apply(Player actingPlayer, Player leftNeighbour, Player rightNeighbour, IList<Card> discards)
        {
            actingPlayer.RemoveCardFromHand(cardToDiscard);
            discards.Add(cardToDiscard);
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
            ConsoleHelper.WriteCardToConsole(cardToDiscard);
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
        private Card cardToDiscard;
        private int coinsToLeftNeighbour;
        private int coinsToRightNeighbour;
    }
}
