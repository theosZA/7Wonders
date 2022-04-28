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

        private WonderStage wonderStage;
    }
}
