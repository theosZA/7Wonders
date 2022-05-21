using _7Wonders;
using System;
using System.Diagnostics;
using System.Linq;

namespace PerformanceTester
{
    class Program
    {
        static void Main()
        {
            const int playerCount = 7;
            const BoardSide boardSide = BoardSide.A;

            var availableTableaus = new StartingTableauCollection("..\\..\\..\\Cities.xml");
            var allCards = new CardCollection("..\\..\\..\\Cards.xml");
            var playerFactory = new RobotPlayerFactory("..\\..\\..\\Robots.xml");

            // Time games for 10 seconds.
            Console.WriteLine("Playing games for 10 seconds...");
            int gameCount = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 10000)
            {
                for (int g = 0; g < 10; ++g)
                {
                    var playerAgents = Enumerable.Range(1, playerCount)
                                                 .Select(i => (PlayerAgent)playerFactory.CreatePlayer($"Robot {i}", boardSide))
                                                 .ToList();
                    var game = new Game(playerAgents, availableTableaus, allCards, boardSide);
                    while (!game.IsGameOver)
                    {
                        game.PlayTurn();
                    }
                    ++gameCount;
                }
            }
            stopwatch.Stop();

            Console.WriteLine($"{gameCount} games played in {stopwatch.ElapsedMilliseconds / 1000.0:0.000} seconds: {gameCount / (stopwatch.Elapsed.TotalMilliseconds / 1000.0):0.000} games/second");
        }
    }
}
