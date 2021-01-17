using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    internal class RobotPlayer : Player
    {
        public RobotPlayer(string name, Tableau tableau) : base(name, tableau)
        { }

        public RobotPlayer(Player player) : base(player)
        { }

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
                    return production * 8 + actingPlayerClone.Military * 2 + actingPlayerClone.VictoryPoints;

                case 2:
                    return production * 5 + actingPlayerClone.Military * 3 + actingPlayerClone.VictoryPoints;

                default:
                    return production * 2 + actingPlayerClone.Military * 4 + actingPlayerClone.VictoryPoints;
            }
        }
    }
}
