using _7Wonders;
using System.Collections.Generic;
using System.Linq;

namespace _7WondersEvolution
{
    internal class PlayerPool
    {
        public IEnumerable<(string name, int generation)> Info => players.Select(player => (player.Name, player.Generation));

        public IEnumerable<(int games, double averagePosition, double averageVictoryPoints)> Stats => players.Select(player => (player.Games, player.AveragePosition, player.AverageVictoryPoints));

        public bool AnyGamesPlayed => players.Any(player => player.Games > 0);

        public PlayerPool(int count)
        {
            players = Enumerable.Repeat(0, count)
                                .Select(i => new EvolvingPlayer())
                                .ToList();
        }

        public void PlayGamesWithRandomPlayers(int gameCount, int playerCount, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            for (int i = 0; i < gameCount; ++i)
            {
                PlayGameWithRandomPlayers(playerCount, availableTableaus, allCards);
            }
        }

        public void ReplaceWithNewGeneration()
        {
            // We start by keeping the best quarter of each generation.
            var newPlayers = players.OrderByDescending(player => player.AverageVictoryPoints)
                                     .Take(players.Count / 4)
                                     .Select(player => new EvolvingPlayer(player))
                                     .ToList();
            // We fill the rest of the spots by breeding random couples.
            for (int i = newPlayers.Count; i < players.Count; ++i)
            {
                var playerPair = PickTwoPlayersWeighted();
                newPlayers.Add(new EvolvingPlayer(playerPair.Item1, playerPair.Item2));
            }

            players = newPlayers;
        }

        private void PlayGameWithRandomPlayers(int playerCount, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            // Pick only from the players who've played the fewest games so far (to balance it out).
            int fewestGames = players.Min(player => player.Games);
            var currentPlayers = players.Where(player => player.Games == fewestGames)
                                        .TakeRandom(playerCount)
                                        .ToList();
            // If we need additional, take at random from the rest. They should all be no more than a game ahead.
            if (currentPlayers.Count < playerCount)
            {
                currentPlayers.AddRange(players.Except(currentPlayers)
                                               .TakeRandom(playerCount - currentPlayers.Count));
            }
            // Play with the picked players.
            PlayGame(currentPlayers, availableTableaus, allCards);
        }

        private static void PlayGame(IList<EvolvingPlayer> gamePlayers, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            var playerAgents = gamePlayers.Select(evolvingPlayer => new RobotPlayer(evolvingPlayer.Name, evolvingPlayer.Weights))
                                          .ToList();
            var game = new Game(playerAgents, availableTableaus, allCards);
            while (!game.IsGameOver)
            {
                game.PlayTurn();
            }
            var victoryPoints = game.VictoryPoints;
            var positions = game.Positions;
            for (int i = 0; i < gamePlayers.Count; ++i)
            {
                gamePlayers[i].AddGame(positions[i], victoryPoints[i]);
            }
        }

        private (EvolvingPlayer, EvolvingPlayer) PickTwoPlayersWeighted()
        {
            var player1 = PickPlayerWeighted();
            while (true)
            {
                var player2 = PickPlayerWeighted();
                if (player1 != player2)
                {
                    return (player1, player2);
                }
            }
        }

        private EvolvingPlayer PickPlayerWeighted()
        {
            double totalRandomWeight = players.Sum(player => player.AverageVictoryPoints);
            double weight = ThreadSafeRandom.ThisThreadsRandom.NextDouble() * totalRandomWeight;
            return players.TakeWhenSumExceeds(weight, player => player.AverageVictoryPoints);

        }

        private IReadOnlyCollection<EvolvingPlayer> players;
    }
}
