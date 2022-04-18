using System;

namespace Extensions
{
    public static class TupleExtensions
    {
        /// <summary>
        /// The '=' comparison is defined by (a,b) = (c,d) iff a=b and c=d.
        /// </summary>
        public static bool IsEqualTo<T>(this (T, T) lhs, (T, T) rhs) where T : IComparable
        {
            return lhs.Item1.CompareTo(rhs.Item1) == 0
                && lhs.Item2.CompareTo(rhs.Item2) == 0;
        }

        /// <summary>
        /// The '<=' comparison that is the partial order defined by (a,b) <= (c,d) iff (a <= c) and (b <= d).
        /// </summary>
        public static bool IsLessThanOrEqualTo<T>(this (T, T) lhs, (T, T) rhs) where T : IComparable
        {
            return lhs.Item1.CompareTo(rhs.Item1) <= 0
                && lhs.Item2.CompareTo(rhs.Item2) <= 0;
        }

        /// <summary>
        /// The '<' comparison is defined by P <= Q and not P = Q.
        /// </summary>
        public static bool IsLessThan<T>(this (T, T) lhs, (T, T) rhs) where T : IComparable
        {
            return lhs.IsLessThanOrEqualTo(rhs) && !lhs.IsEqualTo(rhs);
        }
    }
}
