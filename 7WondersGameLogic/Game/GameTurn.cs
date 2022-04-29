using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// Information about all that happened in a game turn.
    /// </summary>
    public struct GameTurn
    {
        public IReadOnlyCollection<IAction> playerActions;
        public IReadOnlyCollection<IAction> additionalPlayerActions;    // for when a player gets an additional action in a turn, e.g. from a wonder ability
        public IReadOnlyCollection<MilitaryResult> militaryResults;

    }
}
