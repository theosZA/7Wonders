using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    internal class RobotPlayer : Player
    {
        public RobotPlayer(string name, Tableau tableau) : base(name, tableau)
        { }

        override protected IAction GetAction(IEnumerable<IAction> possibleActions)
        {
            // Simple logic for now just picks the first legal action.
            return possibleActions.First();
        }
    }
}
