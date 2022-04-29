using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// This player just always plays the first possible action available to it.
    /// </summary>
    public class DefaultPlayer : PlayerAgent
    {
        public string Name { get; }

        public DefaultPlayer(string name)
        {
            Name = name;
        }

        public IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand)
        {
            var actingPlayer = playerStates[0];
            var leftNeighbour = playerStates[1];
            var rightNeighbour = playerStates[playerStates.Count - 1];

            return actingPlayer.GetAllActions(hand, leftNeighbour, rightNeighbour).First();
        }

        public Card GetBuildFromDiscards(IList<PlayerState> playerStates, IList<Card> discards)
        {
            return playerStates[0].GetAllBuildableCards(discards).FirstOrDefault();
        }
    }
}
