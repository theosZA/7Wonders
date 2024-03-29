﻿using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// Interface that all player agent classes (such as humans or AIs) must implement.
    /// </summary>
    public interface PlayerAgent
    {
        /// <summary>
        /// A unique name for this player agent instance.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Given all the public information plus the player agent's hand, what action will they take.
        /// </summary>
        /// <param name="playerStates">
        /// Each individual's player state in clockwise order. The collection will have at least 3 elements.
        /// The first item in the collection is this player's state.
        /// The next item in the collection is this player's left-hand neighbour.
        /// The last item in the collection is this player's right-hand neightbour.
        /// </param>
        /// <param name="hand">This player's hand.</param>
        /// <returns>A valid action.</returns>
        IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand);

        /// <summary>
        /// Given all the public information plus the discards available, which discard they will take to build for free.
        /// </summary>
        /// <param name="playerStates">
        /// Each individual's player state in clockwise order. The collection will have at least 3 elements.
        /// The first item in the collection is this player's state.
        /// The next item in the collection is this player's left-hand neighbour.
        /// The last item in the collection is this player's right-hand neightbour.
        /// </param>
        /// <param name="discards">All cards that have been discarded.</param>
        /// <returns>Any one card from the discards that has not already been built by the player.</returns>
        /// <remarks>Can return null if there are no valid cards to build.</remarks>
        Card GetBuildFromDiscards(IList<PlayerState> playerStates, IList<Card> discards);
    }
}
