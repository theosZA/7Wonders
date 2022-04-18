using System;
using System.Linq;

namespace _7Wonders
{
    class Program
    {
        static void Main()
        {
            const int robotCount = 3;
            const char citySide = 'A';

            var availableTableaus = new StartingTableauCollection("..\\..\\..\\Cities.xml");
            var allCards = new CardCollection("..\\..\\..\\Cards.xml");
            var playerFactory = new RobotPlayerFactory("..\\..\\..\\Robots.xml");

            var playerAgents = Enumerable.Range(1, robotCount)
                                         .Select(i => (PlayerAgent)playerFactory.CreatePlayer($"Robot {i}", citySide))
                                         .ToList();
            playerAgents.Add(new ConsolePlayer("YOU"));

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
    }
}
