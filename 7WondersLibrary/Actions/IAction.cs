using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// Interface for an individual game action by a single player.
    /// </summary>
    public interface IAction
    {
        void Apply(PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand, IList<Card> discards);
        void WriteToConsole();
    }
}
