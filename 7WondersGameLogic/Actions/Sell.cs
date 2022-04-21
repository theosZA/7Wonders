using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which the selected card is discarded for 3 coins.
    /// </summary>
    public class Sell : IAction
    {
        public Card Card { get; }

        public Sell(Card card)
        {
            Card = card;
        }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(Card);
            discards.Add(Card);
            actingPlayer.AddCoins(3);
        }

        public void WriteToConsole()
        {
            Console.Write("Sell ");
            ConsoleHelper.WriteCardToConsole(Card);
            Console.WriteLine(" for 3 coins");
        }
    }
}
