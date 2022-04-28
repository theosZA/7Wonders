using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// An action in which a player builds the selected card.
    /// </summary>
    public class Build : IAction
    {
        public Card Card { get; set; }

        public int CoinsToLeftNeighbour { get; set; }
        public int CoinsToRightNeighbour { get; set; }

        /// <summary>
        /// True if the card is built for free using an earned ability to build for free. If set to true
        /// any cost of the card or coins sent to neighbours will be ignored.
        /// (This is not related to the ability to build for free if you already have a pre-requisite card.)
        /// </summary>
        public bool UsesFreeBuild { get; set; }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            hand.Remove(Card);
            actingPlayer.AddCardToTableau(Card);
            if (UsesFreeBuild)
            {
                actingPlayer.UseFreeBuild();
            }
            else
            {
                actingPlayer.PayCoins(Card.Cost.Coins);
                actingPlayer.PayCoins(CoinsToLeftNeighbour);
                leftNeighbour.AddCoins(CoinsToLeftNeighbour);
                actingPlayer.PayCoins(CoinsToRightNeighbour);
                rightNeighbour.AddCoins(CoinsToRightNeighbour);
            }

            Card.OnPlayerGain(actingPlayer, leftNeighbour, rightNeighbour);
        }
    }
}
