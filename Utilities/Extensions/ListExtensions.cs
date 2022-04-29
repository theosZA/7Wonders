using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Returns the index of the element that satisfies the given predicate, or null if there is no such element.
        /// </summary>
        public static int? FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (predicate(list[i]))
                {
                    return i;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if the pair at the given index is a minimal pair of the given collection of pairs. The minimal elements are those for
        /// which there are no other elements less than them using the partial order (a, b) <= (c, d) iff (a <= c) and (b <= d).
        /// </summary>
        public static bool IsMinimalElement<T>(this IList<(T, T)> listOfPairs, int index) where T : IComparable
        {
            for (int i = 0; i < listOfPairs.Count; ++i)
            {
                if (i != index && listOfPairs[i].IsLessThan(listOfPairs[index]))
                {   // There's at least one other pair that is less than the given pair.
                    return false;
                }
            }
            return true;
        }
    }
}
