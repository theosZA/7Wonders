using System;
using System.Collections.Generic;

namespace _7Wonders
{
    internal static class EnumerableExtensions
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
    }
}
