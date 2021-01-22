using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// The player collection represents the player agents, their game states and the hands that are passed around.
    /// </summary>
    internal class PlayerCollection
    {
        public PlayerCollection(IReadOnlyCollection<PlayerAgent> playerAgents, IList<Tableau> availableTableaus)
        {
            players = playerAgents.Zip(availableTableaus.TakeRandom(playerAgents.Count()), (playerAgent, tableau) => new Player(playerAgent, tableau))
                                  .ToArray();
        }

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

        public void WriteStateToConsole()
        {
            for (int i = 0; i < Count; ++i)
            {
                players[i].WriteStateToConsole(hands[i], GetLeftNeighbour(i), GetRightNeighbour(i));
                Console.WriteLine("======================================================================");
                Console.WriteLine();
            }
        }

        public void WriteLeaderboardToConsole()
        {
            var leaderboard = Leaderboard.ToList();
            for (int i = 0; i < leaderboard.Count; ++i)
            {
                if (i > 0 && leaderboard[i].victoryPoints == leaderboard[i - 1].victoryPoints && leaderboard[i].player.Coins == leaderboard[i - 1].player.Coins)
                {   // Tie - leave out position number.
                    Console.Write("   ");
                }
                else
                {
                    Console.Write($"{i + 1}. ");
                }
                Console.Write($"{leaderboard[i].player.Name}: {leaderboard[i].victoryPoints} VPs");
                if ((i > 0 && leaderboard[i].victoryPoints == leaderboard[i - 1].victoryPoints) ||
                    (i < leaderboard.Count - 1 && leaderboard[i].victoryPoints == leaderboard[i + 1].victoryPoints))
                {   // VPs tied with another player - list coins as well.
                    Console.Write($" ({leaderboard[i].player.Coins} coins)");
                }
                Console.WriteLine();
            }
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
                // Create a new sequence of players such that the current player is the first in the sequence.
                var cycledPlayers = Enumerable.Range(0, Count)
                                              .Select(offset => players[(i + offset) % Count]);

                yield return players[i].GetAction(hands[i], cycledPlayers);
            }
        }

        public void ApplyActions(IEnumerable<IAction> actions, IList<Card> discards)
        {
            var actionList = actions.ToList();
            for (int i = 0; i < Count; ++i)
            {
                players[i].ApplyAction(actionList[i], GetLeftNeighbour(i), GetRightNeighbour(i), hands[i], discards);
            }
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

        private Player[] players;
        private List<List<Card>> hands;
    }
}
