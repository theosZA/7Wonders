using _7Wonders;
using System;

namespace _7WondersTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var playerTypes = new[] { PlayerType.Default,
                                      PlayerType.Robot,
                                      PlayerType.Robot,
                                      PlayerType.Default,
                                      PlayerType.Robot,
                                      PlayerType.Default,
                                      PlayerType.Robot };
            var game = new Game(playerTypes);
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
