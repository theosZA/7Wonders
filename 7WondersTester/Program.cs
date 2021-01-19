using _7Wonders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _7WondersTester
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var playerTypes = new[] { PlayerType.Default,
                                      PlayerType.Robot,
                                      PlayerType.Robot,
                                      PlayerType.Default,
                                      PlayerType.Robot,
                                      PlayerType.Default,
                                      PlayerType.Robot };
            var playerNames = new[] { "Dumbo",
                                      "Joe",
                                      "James",
                                      "Dunce",
                                      "Jeremiah",
                                      "Dimwit",
                                      "Jesse" };
            var playerFactory = new SimplePlayerFactory(playerTypes, playerNames);*/

            int playerCount = 7;
            var weightsPerPlayer = Enumerable.Range(0, playerCount)
                                             .Select(i => CreateRandomWeights(9).ToArray());
            var playerNames = weightsPerPlayer.Select(weights => string.Join("-", weights));
            var playerFactory = new GeneticPlayerFactory(weightsPerPlayer, playerNames);

            var game = new Game(playerCount, playerFactory);
            for (int age = 1; age <= 3; ++age)
            {
                for (int i = 0; i < 6; ++i)
                {
                    Console.WriteLine($"Age {age}, Turn {i + 1}");
                    game.PlayTurn().WriteToConsole();
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
                game.WriteStateToConsole();
            }
        }

        private static IEnumerable<int> CreateRandomWeights(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return ThreadSafeRandom.ThisThreadsRandom.Next(0, 255);
            }
        }
    }
}
