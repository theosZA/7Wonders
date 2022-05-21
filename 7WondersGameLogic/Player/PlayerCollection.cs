using Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _7Wonders
{
    /// <summary>
    /// The player collection represents the player agents, their game states and the hands that are passed around.
    /// </summary>
    internal class PlayerCollection
    {
        public Player this[int i] => players[i];

        public int Count => players.Length;

        public int CardsInHand => hands[0].Count;   // All hands should have the same number of cards.

        public IEnumerable<int> VictoryPoints => players.Select((player, i) => player.CalculateVictoryPoints(GetLeftNeighbour(i), GetRightNeighbour(i)));

        public IEnumerable<int> Positions
        {
            get
            {
                var leaderboard = Leaderboard.ToList();
                foreach (var player in players)
                {
                    yield return leaderboard.FindIndex(pair => player == pair.player) + 1; // position 1-7
                }
            }
        }

        public IReadOnlyCollection<(Player player, int victoryPoints)> Leaderboard => players.Select((player, i) => (player, player.CalculateVictoryPoints(GetLeftNeighbour(i), GetRightNeighbour(i))))
                                                                                             .AsQueryable()
                                                                                             .OrderByDescending(playerScore => playerScore.Item2)
                                                                                             .ThenByDescending(playerScore => playerScore.player.Coins)
                                                                                             .ToList();

        public PlayerCollection(IReadOnlyCollection<PlayerAgent> playerAgents, IReadOnlyCollection<Tableau> availableTableaus, string firstCityOverride = null)
        {
            IEnumerable<Tableau> tableaus;
            if (!string.IsNullOrEmpty(firstCityOverride))
            {
                // Force the first tableau to have the given city name (e.g. so that the human player has it).
                var chosenTableau = availableTableaus.First(tableau => tableau.CityName == firstCityOverride);
                tableaus = availableTableaus.Where(tableau => tableau.CityName != firstCityOverride)
                                            .Shuffle()
                                            .Prepend(chosenTableau);
            }
            else
            {
                // Fully random.
                tableaus = availableTableaus.Shuffle(); 
            }

            players = CreatePlayers(playerAgents, tableaus).ToArray();
        }

        public void StartAge(int newAge)
        {
            foreach (var player in players)
            {
                player.StartAge(newAge);
            }
        }

        public string GetLeaderboardText()
        {
            var text = new StringBuilder();

            var leaderboard = Leaderboard.ToList();
            for (int i = 0; i < leaderboard.Count; ++i)
            {
                if (i > 0 && leaderboard[i].victoryPoints == leaderboard[i - 1].victoryPoints && leaderboard[i].player.Coins == leaderboard[i - 1].player.Coins)
                {   // Tie - leave out position number.
                    text.Append("   ");
                }
                else
                {
                    text.Append($"{i + 1}. ");
                }
                text.Append($"{leaderboard[i].player.Name} ({leaderboard[i].player.CityName}): {leaderboard[i].victoryPoints} VPs");
                if ((i > 0 && leaderboard[i].victoryPoints == leaderboard[i - 1].victoryPoints) ||
                    (i < leaderboard.Count - 1 && leaderboard[i].victoryPoints == leaderboard[i + 1].victoryPoints))
                {   // VPs tied with another player - list coins as well.
                    int coins = leaderboard[i].player.Coins;
                    text.Append($" ({coins} {(coins == 1 ? "coin" : "coins")})");
                }
                text.AppendLine();
            }

            return text.ToString();
        }

        public void DealDeck(Deck deck)
        {
            const int handSize = 7;
            hands = players.Select(player => deck.DrawCards(handSize).ToList())
                           .ToList();
        }

        public void PassHands(Direction passDirection)
        {
            if (passDirection == Direction.Right)
            {
                var tempHand = hands.First();
                hands.RemoveAt(0);
                hands.Add(tempHand);
            }
            else    // pass left
            {
                var tempHand = hands.Last();
                hands.RemoveAt(hands.Count - 1);
                hands.Insert(0, tempHand);
            }
        }

        public IEnumerable<Card> DiscardHands()
        {
            var discards = hands.SelectMany(card => card).ToList();
            hands = players.Select(player => new List<Card>()).ToList();
            return discards;
        }

        public IEnumerable<IAction> GetActions()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return players[i].GetAction(hands[i], CyclePlayers(i));
            }
        }

        public void ApplyActions(IList<IAction> actions, IList<Card> discards)
        {
            for (int i = 0; i < Count; ++i)
            {
                ApplyAction(i, actions[i], discards);
            }
        }

        public void ApplyAction(int playerIndex, IAction action, IList<Card> discards)
        {
            players[playerIndex].ApplyAction(action, GetLeftNeighbour(playerIndex), GetRightNeighbour(playerIndex), hands[playerIndex], discards);
        }

        public (int playerIndex, IAction action)? GetExtraAgePlay()
        {
            var playerIndex = players.FindIndex(player => player.HasExtraAgePlay);
            if (!playerIndex.HasValue)
            {
                return null;
            }

            int i = playerIndex.Value;
            return (i, players[i].GetAction(hands[i], CyclePlayers(i)));
        }

        public (int playerIndex, Card card)? GetBuildFromDiscards(IList<Card> discards)
        {
            var playerIndex = players.FindIndex(player => player.PendingBuildFromDiscard);
            if (!playerIndex.HasValue)
            {
                return null;
            }

            var cardToBuild = players[playerIndex.Value].GetBuildFromDiscards(discards, CyclePlayers(playerIndex.Value));
            if (cardToBuild == null)
            {
                return null;
            }

            return (playerIndex.Value, cardToBuild);
        }

        public IEnumerable<MilitaryResult> EvaluateMilitaryBattles(int scoreForVictory, int scoreForDefeat)
        {
            for (int i = 0; i < Count; ++i)
            {
                var rightPlayer = players[i];
                var leftPlayer = GetLeftNeighbour(i);
                var militaryResult = MilitaryResult.EvaluateMilitaryBattle(rightPlayer, leftPlayer, scoreForVictory, scoreForDefeat);
                if (militaryResult.HasValue)
                {
                    militaryResult.Value.winningPlayer.AwardMilitaryVictoryPoints(scoreForVictory);
                    militaryResult.Value.losingPlayer.AwardMilitaryVictoryPoints(scoreForDefeat);
                    yield return militaryResult.Value;
                }
            }
        }

        private Player GetLeftNeighbour(int playerIndex)
        {
            return players[(playerIndex + 1) % players.Length];
        }

        private Player GetRightNeighbour(int playerIndex)
        {
            return players[(playerIndex + players.Length - 1) % players.Length];
        }

        private static IEnumerable<Player> CreatePlayers(IEnumerable<PlayerAgent> playerAgents, IEnumerable<Tableau> tableaus)
        {
            return playerAgents.Zip(tableaus, (playerAgent, tableau) => new Player(playerAgent, tableau));
        }

        /// <summary>
        /// Returns a new sequence of players where the first player corresponds to the player in the root index of our existing player array.
        /// This new sequence can be passed down to that player who can think of themselves as player 0.
        /// </summary>
        private IEnumerable<Player> CyclePlayers(int rootIndex)
        {
            return Enumerable.Range(0, Count)
                             .Select(offset => players[(rootIndex + offset) % Count]);
        }

        private Player[] players;
        private List<List<Card>> hands;
    }
}
