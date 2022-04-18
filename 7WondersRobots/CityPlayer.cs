using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// This player holds multiple different personalities. The one that's ultimately
    /// used depends on which city the player receives. This is useful when the player
    /// logic is dependent on the city.
    /// </summary>
    public class CityPlayer : PlayerAgent
    {
        public string Name { get; }

        public CityPlayer(string name, IDictionary<string, PlayerAgent> playerAgentsByCityName)
        {
            Name = name;
            this.playerAgentsByCityName = playerAgentsByCityName;
        }

        public IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand)
        {
            var playerAgent = playerAgentsByCityName[playerStates[0].CityName];
            return playerAgent.GetAction(playerStates, hand);
        }

        private IDictionary<string, PlayerAgent> playerAgentsByCityName;
    }
}
