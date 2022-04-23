using Extensions;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    public class RobotPlayer : PlayerAgent
    {
        public string Name { get; }

        public static int WeightsRequired => weightsPerTurn * 18;

        public RobotPlayer(string name, IEnumerable<int> weights)
        {
            this.weights = weights.ToArray();
            Name = name;
        }

        public IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand)
        {
            var actingPlayer = playerStates[0];
            var leftNeighbour = playerStates[1];
            var rightNeighbour = playerStates[playerStates.Count - 1];

            // Consider all the given actions and pick the one with the highest evaluation score.
            return actingPlayer.GetAllActions(hand, leftNeighbour, rightNeighbour)
                               .MaxElement(action => Evaluate(action, new PlayerState(actingPlayer), new PlayerState(leftNeighbour), new PlayerState(rightNeighbour), new List<Card>(hand)));
        }

        private double Evaluate(IAction action, PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand)
        {
            int turn = ((hand[0].Age - 1) * 6) + (7 - hand.Count);
            int offset = weightsPerTurn * turn;

            action.Apply(actingPlayer, leftNeighbour, rightNeighbour, hand, discards: new List<Card>());

            return EvaluateResources(offset + 0, actingPlayer)
                 + EvaluateScience(offset + 28, actingPlayer)
                 + weights[offset + 31] * actingPlayer.Military
                 + weights[offset + 32] * actingPlayer.Coins
                 + weights[offset + 33] * actingPlayer.CalculateVictoryPoints(leftNeighbour, rightNeighbour);
        }

        private double EvaluateResources(int resourceOffset, PlayerState actingPlayer)
        {
            double score = 0.0;
            for (int i = 0; i < 7; ++i)
            {
                double count = actingPlayer.GetResourceCount((Resource)i);
                for (int compareCount = 0; compareCount < 4; ++compareCount)
                {
                    if (count > compareCount)
                    {
                        score += weights[resourceOffset + (4 * i) + compareCount];
                    }
                }
            }
            return score;
        }

        private double EvaluateScience(int scienceOffset, PlayerState actingPlayer)
        {
            var scienceSymbolCounts = actingPlayer.GetScienceCountsWithWildsAllocated().ToList();
            return weights[scienceOffset + 2] * scienceSymbolCounts[0]
                 + weights[scienceOffset + 1] * scienceSymbolCounts[1]
                 + weights[scienceOffset + 0] * scienceSymbolCounts[2];
        }

        const int weightsPerTurn = 34;
        readonly int[] weights;
    }
}
