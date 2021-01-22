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
        public int winningVictoryPoints;
        public int winningMilitary;

        public Player losingPlayer;
        public int losingVictoryPoints;
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
                winningVictoryPoints = scoreForVictory,
                winningMilitary = Math.Max(militaryA, militaryB),

                losingPlayer = militaryA > militaryB ? playerB : playerA,
                losingVictoryPoints = scoreForDefeat,
                losingMilitary = Math.Min(militaryA, militaryB)
            };
        }
    }
}
