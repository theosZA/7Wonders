using Extensions;
using System.Collections.Generic;
using System.Linq;
using Threading;

namespace _7Wonders
{
    internal class PlayerPool
    {
        public IEnumerable<(string name, string cityName, int generation)> Info => players.Select(player => (player.Name, player.CityName, player.Generation));

        public IEnumerable<(int games, double averagePosition, double averageVictoryPoints)> Stats => players.Select(player => (player.Games, player.AveragePosition, player.AverageVictoryPoints));

        public bool AnyGamesPlayed => players.Any(player => player.Games > 0);

        public PlayerPool(PlayerPool source)
        {
            players = source.players.Select(player => player.Clone())
                                    .ToList();
        }

        public PlayerPool(int countPerCity, IEnumerable<string> cityNames)
        {
            players = Enumerable.Repeat(0, countPerCity)
                                .SelectMany(_ => cityNames, (_, cityName) => new EvolvingPlayer(cityName))
                                .ToList();
        }

        public void PlayGamesWithRandomPlayers(int gameCount, int playerCount, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            for (int i = 0; i < gameCount; ++i)
            {
                PlayGameWithRandomPlayers(playerCount, availableTableaus, allCards);
            }
        }

        public void ReplaceWithNewGeneration(int newGenerationNumber, int playersPerCity)
        {
            var playersByCity = players.GroupBy(player => player.CityName);
            var bestPlayerForEachCity = playersByCity.Select(playerGroup => (playerGroup.Key, playerGroup.MinElementRandom(player => player.AveragePosition)));

            players = playersByCity.Select(cityPlayers => CreateNewGeneration(newGenerationNumber,
                                                                              cityPlayers.Key,
                                                                              playersPerCity,
                                                                              cityPlayers,
                                                                              bestPlayerForEachCity.Where(bestPlayerForCity => bestPlayerForCity.Key != cityPlayers.Key)
                                                                                                   .Select(bestPlayerForCity => bestPlayerForCity.Item2)))
                                   .SelectMany(x => x)
                                   .ToList();
        }

        private void PlayGameWithRandomPlayers(int playerCount, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            // For each city pick the player who has played the fewest games so far (to balance it out).
            var fewestGamePlayers = players.GroupBy(player => player.CityName)
                                           .Select(cityPlayers => cityPlayers.MinElementRandom(player => player.Games))
                                           .ToList();

            PlayGame(playerCount, fewestGamePlayers, availableTableaus, allCards);
        }

        private static void PlayGame(int playerCount, IReadOnlyCollection<EvolvingPlayer> cityPlayers, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            // Set up player agents. We need to wrap the individual city-specific agents in a smart agent that will select
            // the city-specific agent based on which city it is playing.
            var playerAgentsByCity = cityPlayers.ToDictionary(evolvingPlayer => evolvingPlayer.CityName,
                                                              evolvingPlayer => (PlayerAgent)new RobotPlayer(evolvingPlayer.Name, evolvingPlayer.Weights));
            var gamePlayers = Enumerable.Range(1, playerCount)
                                        .Select(i => (PlayerAgent)new CityPlayer($"Player {i}", playerAgentsByCity))
                                        .ToList();

            // Play the game.
            var game = new Game(gamePlayers, availableTableaus, allCards);
            while (!game.IsGameOver)
            {
                game.PlayTurn();
            }

            // Update the (evolving) players with the results of the game.
            for (int i = 0; i < playerCount; ++i)
            {
                string cityName = game.GetPlayer(i).CityName;
                int position = game.Positions[i];
                int victoryPoints = game.VictoryPoints[i];

                cityPlayers.First(player => player.CityName == cityName).AddGame(position, victoryPoints);
            }
        }

        private IReadOnlyCollection<EvolvingPlayer> CreateNewGeneration(int newGenerationNumber, string cityName, int newPlayersCount, IEnumerable<EvolvingPlayer> players, IEnumerable<EvolvingPlayer> bestPlayersOfOtherCities)
        {
            var orderedPlayers = players.OrderBy(player => player.AveragePosition)
                                        .ToList();

            // We start by keeping the best quarter of each generation.
            var newPlayers = orderedPlayers.Take(orderedPlayers.Count / 4)
                                           .Select(player => new EvolvingPlayer(player))
                                           .ToList();

            // Every 10th generation, also add in the best players from other cities.
            if (newGenerationNumber % 10 == 0)
            {
                newPlayers.AddRange(bestPlayersOfOtherCities.Select(player => new EvolvingPlayer(cityName, player)));
            }

            // We fill the rest of the spots by breeding random couples from the top half.
            var breedingPlayers = orderedPlayers.Take(orderedPlayers.Count / 2)
                                                .ToList();
            while (newPlayers.Count < newPlayersCount)
            {
                var playerPair = PickTwoPlayersWeighted(breedingPlayers);
                newPlayers.Add(new EvolvingPlayer(playerPair.Item1, playerPair.Item2));
            }

            return newPlayers;
        }

        private static (EvolvingPlayer, EvolvingPlayer) PickTwoPlayersWeighted(IReadOnlyCollection<EvolvingPlayer> players)
        {
            var player1 = PickPlayerWeighted(players);
            while (true)
            {
                var player2 = PickPlayerWeighted(players);
                if (player1 != player2)
                {
                    return (player1, player2);
                }
            }
        }

        private static EvolvingPlayer PickPlayerWeighted(IReadOnlyCollection<EvolvingPlayer> players)
        {
            double minAverageVictoryPoints = players.Min(player => player.AverageVictoryPoints);
            double totalRandomWeight = players.Sum(player => player.AverageVictoryPoints - minAverageVictoryPoints);
            double weight = ThreadSafeRandom.ThisThreadsRandom.NextDouble() * totalRandomWeight;
            return players.TakeWhenSumExceeds(weight, player => player.AverageVictoryPoints - minAverageVictoryPoints);

        }

        private IReadOnlyCollection<EvolvingPlayer> players;
    }
}
