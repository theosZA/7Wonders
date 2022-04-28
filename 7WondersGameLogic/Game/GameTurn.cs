using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// Information about all that happened in a game turn.
    /// </summary>
    public struct GameTurn
    {
        public IReadOnlyCollection<IAction> playerActions;
        public IReadOnlyCollection<MilitaryResult> militaryResults;
    }
}
