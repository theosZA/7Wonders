﻿using System;
using System.Collections.Generic;
using System.Linq;
using Threading;

namespace Extensions
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
        /// Returns the element in the collection that results in the lowest evaluation using the evaluation function.
        /// </summary>
        public static T MinElement<T>(this IEnumerable<T> collection, Func<T, double> evaluationFunction) where T : class
        {
            return collection.MaxElement(x => -evaluationFunction(x));
        }

        /// <summary>
        /// Randomly chooses an element in the collection from all elements with the highest evaluation using the evaluation function.
        /// </summary>
        public static T MaxElementRandom<T>(this IEnumerable<T> collection, Func<T, double> evaluationFunction) where T : class
        {
            double max = collection.Max(x => evaluationFunction(x));
            return collection.Where(x => evaluationFunction(x) == max)
                             .TakeRandom(1)
                             .First();
        }

        /// <summary>
        /// Randomly chooses an element in the collection from all elements with the lowest evaluation using the evaluation function.
        /// </summary>
        public static T MinElementRandom<T>(this IEnumerable<T> collection, Func<T, double> evaluationFunction) where T : class
        {
            double min = collection.Min(x => evaluationFunction(x));
            return collection.Where(x => evaluationFunction(x) == min)
                             .TakeRandom(1)
                             .First();
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

        /// <summary>
        /// Returns all minimal elements of a collection of ordered pairs. The minimal elements are those for which there are no other elements
        /// less than them using the partial order (a, b) <= (c, d) iff (a <= c) and (b <= d).
        /// </summary>
        public static IEnumerable<(T, T)> MinimalElements<T>(this IEnumerable<(T, T)> collection) where T: IComparable
        {
            // TBD: Look at http://www.cs.yorku.ca/~jarek/papers/vldbj06/lessII.pdf which deals with this problem.
            // For now we just have an O(n^2) algorithm where we compare each element against every other element.

            var sourceList = collection.Distinct().ToList();
            for (int i = 0; i < sourceList.Count; ++i)
            {
                if (sourceList.IsMinimalElement(i))
                {
                    yield return sourceList[i];
                }
            }
        }
    }
}
