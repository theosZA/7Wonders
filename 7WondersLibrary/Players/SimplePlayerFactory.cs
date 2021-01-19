using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    public class SimplePlayerFactory : IPlayerFactory
    {
        public SimplePlayerFactory(IEnumerable<PlayerType> playerTypes, IEnumerable<string> playerNames)
        {
            this.playerTypes = playerTypes.ToArray();
            this.playerNames = playerNames.ToArray();
        }

        public Player CreatePlayer(int index, Tableau tableau)
        {
            switch (playerTypes[index])
            {
                case PlayerType.Robot:
                    return new RobotPlayer(playerNames[index], tableau, new int[] { 8, 2, 1, 5, 3, 1, 2, 4, 1 });

                case PlayerType.Console:
                    return new ConsolePlayer(playerNames[index], tableau);

                default:
                    return new DefaultPlayer(playerNames[index], tableau);
            }
        }

        private PlayerType[] playerTypes;
        private string[] playerNames;
    }
}