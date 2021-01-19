using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// Provides RobotPlayers generated from the given DNA sequence.
    /// </summary>
    public class GeneticPlayerFactory : IPlayerFactory
    {
        public GeneticPlayerFactory(IEnumerable<int[]> weightsPerPlayer, IEnumerable<string> playerNames)
        {
            this.weightsPerPlayer = weightsPerPlayer.ToArray();
            this.playerNames = playerNames.ToArray();
        }

        public Player CreatePlayer(int index, Tableau tableau)
        {
            return new RobotPlayer(playerNames[index], tableau, weightsPerPlayer[index]);
        }

        private int[][] weightsPerPlayer;
        private string[] playerNames;
    }
}
