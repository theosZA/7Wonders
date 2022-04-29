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
        /// True if the card is built for free using an earned ability to build for free.
        /// If set to true any cost of the card or coins sent to neighbours will be ignored.
        /// (This is not related to the ability to build for free if you already have a pre-requisite card.)
        /// </summary>
        public bool UsesFreeBuild { get; set; }
        /// <summary>
        /// True if the card is built for free from the discards. 
        /// If set to true any cost of the card or coins sent to neighbours will be ignored.
        /// </summary>
        public bool BuiltFromDiscards { get; set; }

        public void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            // Move the card.
            if (BuiltFromDiscards)
            {
                discards.Remove(Card);
            }
            else
            {
                hand.Remove(Card);
            }
            actingPlayer.AddCardToTableau(Card);

            // Make payments (or use up the free build).
            if (UsesFreeBuild)
            {
                actingPlayer.UseFreeBuild();
            }
            if (BuiltFromDiscards)
            {
                actingPlayer.PendingBuildFromDiscard = false;
            }
            if (!UsesFreeBuild && !BuiltFromDiscards)
            {
                actingPlayer.PayCoins(Card.Cost.Coins);
                actingPlayer.PayCoins(CoinsToLeftNeighbour);
                leftNeighbour.AddCoins(CoinsToLeftNeighbour);
                actingPlayer.PayCoins(CoinsToRightNeighbour);
                rightNeighbour.AddCoins(CoinsToRightNeighbour);
            }

            // Trigger any immediate benefits.
            Card.OnPlayerGain(actingPlayer, leftNeighbour, rightNeighbour);
        }
    }
}
