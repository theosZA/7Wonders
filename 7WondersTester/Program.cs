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
            int robots = 3;
            var playerAgents = Enumerable.Range(0, robots)
                                         .Select(i => new RobotPlayer($"Robot {i + 1}", CreateRandomWeights(RobotPlayer.WeightsRequired)))
                                         .Cast<PlayerAgent>()
                                         .ToList();
            playerAgents.Add(new ConsolePlayer("YOU"));

            var availableTableaus = new StartingTableauCollection("..\\..\\..\\Cities.xml");
            var allCards = new CardCollection("..\\..\\..\\Cards.xml");

            var game = new Game(playerAgents, availableTableaus, allCards);
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
