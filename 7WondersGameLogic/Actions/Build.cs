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

        public Build(Card card, int coinsToLeftNeighbour = 0, int coinsToRightNeighbour = 0)
        {
            Card = card;
            this.coinsToLeftNeighbour = coinsToLeftNeighbour;
            this.coinsToRightNeighbour = coinsToRightNeighbour;
        }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(Card);
            actingPlayer.AddCardToTableau(Card);
            actingPlayer.PayCoins(Card.Cost.Coins);
            actingPlayer.PayCoins(coinsToLeftNeighbour);
            leftNeighbour.AddCoins(coinsToLeftNeighbour);
            actingPlayer.PayCoins(coinsToRightNeighbour);
            rightNeighbour.AddCoins(coinsToRightNeighbour);

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

        public bool IsWorseOrEqualThan(Build compareBuild)
        {
            return coinsToLeftNeighbour >= compareBuild.coinsToLeftNeighbour && coinsToRightNeighbour >= compareBuild.coinsToRightNeighbour;
        }

        private int coinsToLeftNeighbour;
        private int coinsToRightNeighbour;
    }
}
