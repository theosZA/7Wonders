using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    internal class PlayerCollection
    {
        public PlayerCollection(IEnumerable<PlayerType> playerTypes, IList<Tableau> tableaus)
        {
            tableaus.Shuffle();
            players = playerTypes.Select((playerType, i) => PlayerFactory.CreatePlayer(playerType, $"Player {i + 1}", tableaus[i]))
                                 .ToArray();
            for (int i = 0; i < players.Length; ++i)
            {
                players[i].SetNeighbours(GetLeftNeighbour(i), GetRightNeighbour(i));
            }
        }

        public int Count => players.Length;

        public int CardsInHand => players[0].CardsInHand;   // Players should always have the same number of cards in hand.

        public IReadOnlyCollection<Player> Leaderboard => players.OrderByDescending(player => player.VictoryPoints).ThenByDescending(player => player.Coins).ToList();

        public void WriteStateToConsole()
        {
            foreach (var player in players)
            {
                player.WriteStateToConsole();
                Console.WriteLine("======================================================================");
                Console.WriteLine();
            }
        }

        public void WriteLeaderboardToConsole()
        {
            var leaderboard = Leaderboard.ToList();
            for (int i = 0; i < leaderboard.Count; ++i)
            {
                if (i > 0 && leaderboard[i].VictoryPoints == leaderboard[i - 1].VictoryPoints && leaderboard[i].Coins == leaderboard[i - 1].Coins)
                {   // Tie - leave out position number.
                    Console.Write("   ");
                }
                else
                {
                    Console.Write($"{i + 1}. ");
                }
                Console.Write($"{leaderboard[i].Name}: {leaderboard[i].VictoryPoints} VPs");
                if ((i > 0 && leaderboard[i].VictoryPoints == leaderboard[i - 1].VictoryPoints) ||
                    (i < leaderboard.Count - 1 && leaderboard[i].VictoryPoints == leaderboard[i + 1].VictoryPoints))
                {   // VPs tied with another player - list coins as well.
                    Console.Write($" ({leaderboard[i].Coins} coins)");
                }
                Console.WriteLine();
            }
        }

        public void DealDeck(Deck deck)
        {
            const int handSize = 7;
            foreach (var player in players)
            {
                var hand = deck.DrawCards(handSize);
                player.SwapHand(hand.ToList());
            }
        }

        public void PassHands(Direction passDirection)
        {
            IList<Card> tempHand = null;
            if (passDirection == Direction.Right)
            {
                for (int i = players.Length - 1; i >= 0; --i)
                {
                    tempHand = players[i].SwapHand(tempHand);
                }
                players[players.Length - 1].SwapHand(tempHand);
            }
            else  // pass left
            {
                for (int i = 0; i < players.Length; ++i)
                {
                    tempHand = players[i].SwapHand(tempHand);
                }
                players[0].SwapHand(tempHand);
            }
        }

        public void DiscardHands(List<Card> discards)
        {
            foreach (var player in players)
            {
                discards.AddRange(player.SwapHand(null));
            }
        }

        public IReadOnlyDictionary<Player, IAction> GetActions()
        {
            var actions = new Dictionary<Player, IAction>();
            foreach (var player in players)
            {
                actions[player] = player.GetAction();
            }
            return actions;
        }

        public void ApplyActions(IReadOnlyDictionary<Player, IAction> actions, IList<Card> discards)
        {
            for (int i = 0; i < players.Length; ++i)
            {
                var actingPlayer = players[i];
                var leftNeighbour = GetLeftNeighbour(i);
                var rightNeightbour = GetRightNeighbour(i);
                actions[actingPlayer].Apply(actingPlayer, leftNeighbour, rightNeightbour, discards);
            }
        }

        public IReadOnlyCollection<MilitaryResult> EvaluateMilitaryBattles(int scoreForVictory, int scoreForDefeat)
        {
            var militaryResults = new List<MilitaryResult>();
            for (int i = 0; i < players.Length; ++i)
            {
                var rightPlayer = players[i];
                var leftPlayer = GetLeftNeighbour(i);
                var militaryResult = MilitaryResult.EvaluateMilitaryBattle(leftPlayer, rightPlayer, scoreForVictory, scoreForDefeat);
                if (militaryResult.HasValue)
                {
                    militaryResult.Value.winningPlayer.AwardMilitaryVictoryPoints(scoreForVictory);
                    militaryResult.Value.losingPlayer.AwardMilitaryVictoryPoints(scoreForDefeat);
                    militaryResults.Add(militaryResult.Value);
                }
            }
            return militaryResults;
        }

        private static Player CreatePlayer(string playerName, Tableau tableau, bool isRobot)
        {
            if (isRobot)
            {
                return new RobotPlayer(playerName, tableau);
            }
            return new ConsolePlayer(playerName, tableau);
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
    }
}
