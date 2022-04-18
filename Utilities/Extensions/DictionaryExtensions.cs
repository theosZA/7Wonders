using System.Collections.Generic;

namespace Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// The dictionary is updated such that each key maps to the sum of the values in the two dictionaries.
        /// </summary>
        public static void AddUpdate<TKey>(this IDictionary<TKey, int> lhs, IDictionary<TKey, int> rhs)
        {
            foreach (var pair in rhs)
            {
                if (lhs.ContainsKey(pair.Key))
                {
                    lhs[pair.Key] += pair.Value;
                }
                else
                {
                    lhs.Add(pair);
                }
            }
        }
    }
}
