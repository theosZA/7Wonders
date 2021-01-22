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
            return playerStates[0].GetAllActions(hand, leftNeighbour, rightNeighbour)
                                  .MaxElement(action => Evaluate(action, new PlayerState(actingPlayer), new PlayerState(leftNeighbour), new PlayerState(rightNeighbour), new List<Card>(hand)));
        }

        private double Evaluate(IAction action, PlayerState actingPlayer, PlayerState leftNeighbour, PlayerState rightNeighbour, IList<Card> hand)
        {
            int turn = ((hand[0].Age - 1) * 6) + (7 - hand.Count);

            action.Apply(actingPlayer, leftNeighbour, rightNeighbour, hand, discards: new List<Card>());

            int offset = weightsPerTurn * turn;
            return weights[offset + 0] * actingPlayer.GetResourceCount(Resource.Clay)
                 + weights[offset + 1] * actingPlayer.GetResourceCount(Resource.Ore)
                 + weights[offset + 2] * actingPlayer.GetResourceCount(Resource.Stone)
                 + weights[offset + 3] * actingPlayer.GetResourceCount(Resource.Wood)
                 + weights[offset + 4] * actingPlayer.GetResourceCount(Resource.Glass)
                 + weights[offset + 5] * actingPlayer.GetResourceCount(Resource.Loom)
                 + weights[offset + 6] * actingPlayer.GetResourceCount(Resource.Papyrus)
                 + weights[offset + 7] * actingPlayer.Military
                 + weights[offset + 8] * actingPlayer.CalculateVictoryPoints(leftNeighbour, rightNeighbour);
        }

        const int weightsPerTurn = 9;
        int[] weights;
    }
}
