using System;

namespace _7Wonders
{
    /// <summary>
    /// Holds the details of one military conflict that takes place at the end of an age.
    /// Note that this can't represent a draw.
    /// </summary>
    public struct MilitaryResult
    {
        public Player winningPlayer;
        public Player losingPlayer;
        public int winningVictoryPoints;
        public int losingVictoryPoints;
        public int winningMilitary;
        public int losingMilitary;

        public static MilitaryResult? EvaluateMilitaryBattle(Player playerA, Player playerB, int scoreForVictory, int scoreForDefeat)
        {
            var militaryA = playerA.Military;
            var militaryB = playerB.Military;
            if (militaryA == militaryB)
            {
                return null;
            }
            return new MilitaryResult
            {
                winningPlayer = militaryA > militaryB ? playerA : playerB,
                losingPlayer = militaryA > militaryB ? playerB : playerA,
                winningVictoryPoints = scoreForVictory,
                losingVictoryPoints = scoreForDefeat,
                winningMilitary = Math.Max(militaryA, militaryB),
                losingMilitary = Math.Min(militaryA, militaryB)
            };
        }
    }
}
