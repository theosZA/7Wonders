using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    internal class RobotPlayer : Player
    {
        public RobotPlayer(string name, Tableau tableau, IEnumerable<int> weights) : base(name, tableau)
        {
            this.weights = weights.ToArray();
        }

        public RobotPlayer(RobotPlayer player) : base(player)
        {
            weights = (int[])player.weights.Clone();
        }

        override protected IAction GetAction(IEnumerable<IAction> possibleActions)
        {
            // Consider all the given actions and pick the one with the highest evaluation score.
            return possibleActions.MaxElement(action => Evaluate(action));
        }

        private double Evaluate(IAction action)
        {
            var actingPlayerClone = new RobotPlayer(this);
            action.Apply(actingPlayerClone, GetNeighbourClone(Direction.Left), GetNeighbourClone(Direction.Right), discards: new List<Card>());

            int production = ResourceHelper.GetAllResources().Sum(resource => actingPlayerClone.GetResourceCount(resource));

            switch (GetAge())
            {
                case 1:
                    return production * weights[0] + actingPlayerClone.Military * weights[1] + actingPlayerClone.VictoryPoints * weights[2];

                case 2:
                    return production * weights[3] + actingPlayerClone.Military * weights[4] + actingPlayerClone.VictoryPoints * weights[5];

                default:
                    return production * weights[6] + actingPlayerClone.Military * weights[7] + actingPlayerClone.VictoryPoints * weights[8];
            }
        }

        int[] weights;
    }
}
