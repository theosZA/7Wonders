using _7Wonders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PerformanceTester
{
    class Program
    {
        static void Main(string[] args)
        {
            const int playerCount = 7;

            // Time games for 10 seconds.
            Console.WriteLine("Playing games for 10 seconds...");
            int gameCount = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                for (int g = 0; g < 10; ++g)
                {
                    var playerAgents = Enumerable.Range(0, playerCount)
                                                 .Select(i => new RobotPlayer($"Robot {i + 1}", CreateRandomWeights(RobotPlayer.WeightsRequired)))
                                                 .Cast<PlayerAgent>()
                                                 .ToList();
                    var game = new Game(playerAgents);
                    while (!game.IsGameOver)
                    {
                        game.PlayTurn();
                    }
                    ++gameCount;
                }
            }
            stopwatch.Stop();

            Console.WriteLine($"{gameCount} games played in {stopwatch.ElapsedMilliseconds / 1000.0:0.000} seconds: {gameCount / (stopwatch.ElapsedMilliseconds / 1000.0):0.000} games/second");
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
