using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// This player just always plays the first possible action available to it.
    /// </summary>
    internal class DefaultPlayer : Player
    {
        public DefaultPlayer(string name, Tableau tableau) : base(name, tableau)
        {}

        public DefaultPlayer(Player player) : base(player)
        {}

        override protected IAction GetAction(IEnumerable<IAction> possibleActions)
        {
            return possibleActions.First();
        }
    }
}
