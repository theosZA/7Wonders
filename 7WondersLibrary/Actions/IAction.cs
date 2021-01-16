using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// Interface for an individual game action by a single player.
    /// </summary>
    public interface IAction
    {
        void Apply(Player actingPlayer, Player leftNeighbour, Player rightNeighbour, IList<Card> discards);
        void WriteToConsole();
    }
}
