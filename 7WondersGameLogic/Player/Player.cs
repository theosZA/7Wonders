using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// A Player is just a player agent (PlayerAgent) combined with the player's game state (PlayerState).
    /// It does not include the player's hand as that is only provided when the player must take a new action.
    /// </summary>
    public class Player
    {
        public string Name => agent.Name;

        public string CityName => state.CityName;

        public BoardSide BoardSide => state.BoardSide;

        public int WonderStagesBuilt => state.WonderStagesBuilt;

        public int WonderStagesLeft => state.WonderStagesLeft;

        public int Coins => state.Coins;

        public int Military => state.Military;

        public int MilitaryVictoryPoints => state.MilitaryVictoryPoints;

        public int TreasuryVictoryPoints => state.TreasuryVictoryPoints;

        public int ScienceVictoryPoints => state.ScienceVictoryPoints;

        public int FreeBuildsPerAge => state.FreeBuildsPerAge;

        public int FreeBuildsLeft => state.FreeBuildsLeft;

        public bool HasExtraAgePlay => state.HasExtraAgePlay;

        public bool PendingBuildFromDiscard => state.PendingBuildFromDiscard;

        public Player(PlayerAgent playerAgent, Tableau tableau)
        {
            agent = playerAgent;
            state = new PlayerState(tableau);
        }

        public void StartAge(int newAge)
        {
            state.StartAge(newAge);
        }

        public int CalculateVictoryPoints(Player leftNeighbour, Player rightNeighbour)
        {
            return state.CalculateVictoryPoints(leftNeighbour.state, rightNeighbour.state);
        }

        public int CalculateWonderVictoryPoints(Player leftNeighbour, Player rightNeighbour)
        {
            return state.CalculateWonderVictoryPoints(leftNeighbour.state, rightNeighbour.state);
        }

        public int CalculateCivilianVictoryPoints(Player leftNeighbour, Player rightNeighbour)
        {
            return state.CalculateCivilianVictoryPoints(leftNeighbour.state, rightNeighbour.state);
        }

        public int CalculateCommercialVictoryPoints(Player leftNeighbour, Player rightNeighbour)
        {
            return state.CalculateCommercialVictoryPoints(leftNeighbour.state, rightNeighbour.state);
        }

        public int CalculateGuildVictoryPoints(Player leftNeighbour, Player rightNeighbour)
        {
            return state.CalculateGuildVictoryPoints(leftNeighbour.state, rightNeighbour.state);
        }

        public void AwardMilitaryVictoryPoints(int militaryVictoryPoints)
        {
            state.AwardMilitaryVictoryPoints(militaryVictoryPoints);
        }

        /// <summary>
        /// Given all the public information plus the player agent's hand, determines what action the player's agent will take.
        /// </summary>
        /// <param name="hand">This player's hand.</param>
        /// <param name="players">
        /// Each player in clockwise order. Requires at least 3 elements.
        /// The first item in the collection must be this player.
        /// The next item in the collection must be this player's left-hand neighbour.
        /// The last item in the collection must be this player's right-hand neightbour.
        /// </param>
        /// <returns>The agent's chosen action.</returns>
        public IAction GetAction(IEnumerable<Card> hand, IEnumerable<Player> players)
        {
            return agent.GetAction(players.Select(player => new PlayerState(player.state)).ToList(), new List<Card>(hand));
        }

        public void ApplyAction(IAction action, Player leftNeighbour, Player rightNeighbour, IList<Card> hand, IList<Card> discards)
        {
            action.Apply(state, leftNeighbour.state, rightNeighbour.state, hand, discards);
        }

        /// <summary>
        /// Given all the public information plus the discards available, which discard they will take to build for free.
        /// </summary>
        /// <param name="discards">All cards that have been discarded.</param>
        /// <param name="players">
        /// Each individual's player state in clockwise order. The collection will have at least 3 elements.
        /// The first item in the collection is this player's state.
        /// The next item in the collection is this player's left-hand neighbour.
        /// The last item in the collection is this player's right-hand neightbour.
        /// </param>
        /// <returns>Any one card from the discards that has not already been built by the player.</returns>
        /// <remarks>Can return null if there are no valid cards to build.</remarks>
        public Card GetBuildFromDiscards(IEnumerable<Card> discards, IEnumerable<Player> players)
        {
            return agent.GetBuildFromDiscards(players.Select(player => new PlayerState(player.state)).ToList(), new List<Card>(discards));
        }

        private PlayerAgent agent;
        private PlayerState state;
    }
}
