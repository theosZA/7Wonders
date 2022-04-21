using Helper;
using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which a player builds the selected card.
    /// </summary>
    public class Build : IAction
    {
        public Card Card { get; }

        public int CoinsToLeftNeighbour { get; }
        public int CoinsToRightNeighbour { get; }

        public Build(Card card, int coinsToLeftNeighbour = 0, int coinsToRightNeighbour = 0)
        {
            Card = card;
            CoinsToLeftNeighbour = coinsToLeftNeighbour;
            CoinsToRightNeighbour = coinsToRightNeighbour;
        }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(Card);
            actingPlayer.AddCardToTableau(Card);
            actingPlayer.PayCoins(Card.Cost.Coins);
            actingPlayer.PayCoins(CoinsToLeftNeighbour);
            leftNeighbour.AddCoins(CoinsToLeftNeighbour);
            actingPlayer.PayCoins(CoinsToRightNeighbour);
            rightNeighbour.AddCoins(CoinsToRightNeighbour);

            // Gain coins immediately from the card (if applicable).
            actingPlayer.AddCoins(Card.CoinGain);
            foreach (var gain in Card.EvaluatedGains)
            {
                actingPlayer.AddCoins(gain.GetCoinsGained(actingPlayer, leftNeighbour, rightNeighbour));
            }
        }

        public void WriteToConsole()
        {
            Console.Write("Build ");
            ConsoleHelper.WriteCardToConsole(Card);
            var costText = Card.Cost.ToString();
            if (!string.IsNullOrEmpty(costText))
            {
                Console.Write($" ({costText})");
            }
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

        public bool IsWorseOrEqualThan(Build compareBuild)
        {
            return CoinsToLeftNeighbour >= compareBuild.CoinsToLeftNeighbour && CoinsToRightNeighbour >= compareBuild.CoinsToRightNeighbour;
        }
    }
}
