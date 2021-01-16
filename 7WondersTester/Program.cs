using System;

namespace _7WondersTester
{
    class Program
    {
        static void Main(string[] args)
        {
            const int playerCount = 7;
            const int humanCount = 0;
            var game = new _7Wonders.Game(playerCount, playerCount - humanCount);
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
