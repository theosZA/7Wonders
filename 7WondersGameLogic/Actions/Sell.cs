using System;
using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which the selected card is discarded for 3 coins.
    /// </summary>
    internal class Sell : IAction
    {
        public Sell(Card card)
        {
            this.card = card;
        }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(card);
            discards.Add(card);
            actingPlayer.AddCoins(3);
        }

        public void WriteToConsole()
        {
            Console.Write("Sell ");
            ConsoleHelper.WriteCardToConsole(card);
            Console.WriteLine(" for 3 coins");
        }

        private Card card;
    }
}
