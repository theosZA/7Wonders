using Helper;
using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which a player builds the next wonder stage.
    /// </summary>
    public class BuildWonderStage : IAction
    {
        public Card CardToSpend { get; }

        public int CoinsToLeftNeighbour { get; }
        public int CoinsToRightNeighbour { get; }

        public BuildWonderStage(WonderStage wonderStage, Card cardToSpend, int coinsToLeftNeighbour = 0, int coinsToRightNeighbour = 0)
        {
            this.wonderStage = wonderStage;
            CardToSpend = cardToSpend;
            CoinsToLeftNeighbour = coinsToLeftNeighbour;
            CoinsToRightNeighbour = coinsToRightNeighbour;
        }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(CardToSpend);
            actingPlayer.PayCoins(wonderStage.Cost.Coins);
            actingPlayer.PayCoins(CoinsToLeftNeighbour);
            leftNeighbour.AddCoins(CoinsToLeftNeighbour);
            actingPlayer.PayCoins(CoinsToRightNeighbour);
            rightNeighbour.AddCoins(CoinsToRightNeighbour);

            actingPlayer.BuildNextWonderStage();

            wonderStage.OnPlayerGain(actingPlayer, leftNeighbour, rightNeighbour);
        }

        public void WriteToConsole()
        {
            Console.Write("Build next wonder stage discarding ");
            ConsoleHelper.WriteCardToConsole(CardToSpend);
            Console.Write($" ({wonderStage.Cost})");
            if (CoinsToLeftNeighbour > 0 || CoinsToRightNeighbour > 0)
            {
                Console.Write(" - paying ");
                if (CoinsToLeftNeighbour > 0)
                {
                    Console.Write($"{CoinsToLeftNeighbour} {TextHelper.Pluralize("coin", CoinsToLeftNeighbour)} to trade left");
                }
                if (CoinsToLeftNeighbour > 0 && CoinsToRightNeighbour > 0)
                {
                    Console.Write(" and ");
                }
                if (CoinsToRightNeighbour > 0)
                {
                    Console.Write($"{CoinsToRightNeighbour} {TextHelper.Pluralize("coin", CoinsToRightNeighbour)} to trade right");
                }
            }
            Console.WriteLine();
        }

        public bool IsWorseOrEqualThan(BuildWonderStage compareBuild)
        {
            return CoinsToLeftNeighbour >= compareBuild.CoinsToLeftNeighbour && CoinsToRightNeighbour >= compareBuild.CoinsToRightNeighbour;
        }

        private WonderStage wonderStage;
    }
}
