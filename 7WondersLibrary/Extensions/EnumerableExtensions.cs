using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the element in the collection that results in the highest evaluation using the evaluation function.
        /// </summary>
        public static T MaxElement<T>(this IEnumerable<T> collection, Func<T, double> evaluationFunction) where T: class
        {
            T currentMaxItem = null;
            double currentMaxEvalutaion = double.MinValue;
            foreach (T item in collection)
            {
                double evaluation = evaluationFunction(item);
                if (evaluation >= currentMaxEvalutaion)
                {
                    currentMaxItem = item;
                    currentMaxEvalutaion = evaluation;
                }
            }
            return currentMaxItem;
        }

        /// <summary>
        /// Returns a randomly ordered sequence of the original collection using the Knuth shuffle algorithm.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            var list = collection.ToList();
            for (int i = list.Count; i > 0; i--)
            {
                var index = ThreadSafeRandom.ThisThreadsRandom.Next(i);
                var temp = list[index];
                list[index] = list[i - 1];
                list[i - 1] = temp;
                yield return temp;
            }
        }

        /// <summary>
        /// Returns a number of randomly chosen items from the given collection.
        /// </summary>
        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> collection, int count)
        {
            return collection.Shuffle().Take(count);
        }

        /// <summary>
        /// Returns the first element in the collection where the cumulative value calculated by the provided function exceeds the value specified.
        /// </summary>
        public static T TakeWhenSumExceeds<T>(this IEnumerable<T> collection, double value, Func<T, double> function) where T : class
        {
            double sum = 0.0;
            foreach (T item in collection)
            {
                sum += function(item);
                if (sum > value)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
