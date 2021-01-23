using System.Collections.Generic;

namespace _7Wonders
{
    internal static class DistributionHelper
    {
        /// <summary>
        /// Finds all possible ways of dividing the required resources between 3 players.
        /// </summary>
        public static IEnumerable<ResourceDistribution> CalculateAllResourceDistributions(ResourceCollection requiredResources)
        {
            var distributions = new List<ResourceDistribution>();
            RecursiveAddResourceDistributions(distributions, requiredResources, new Stack<Resource>(requiredResources.GetResources()),
                                              new Stack<(Resource resource, int countFromActivePlayer, int countFromLeftNeighbour, int countFromRightNeighbour)>());
            return distributions;
        }

        /// <summary>
        /// Finds all distributions of three non-negative integers which sum to the given total.
        /// </summary>
        private static IEnumerable<(int, int, int)> CalculateAllTripleDistributions(int total)
        {
            for (int i = 0; i <= total; ++i)
            {
                for (int j = 0; j <= total - i; ++j)
                {
                    int k = total - i - j;
                    yield return (i, j, k);
                }
            }
        }

        private static void RecursiveAddResourceDistributions(IList<ResourceDistribution> distributions, ResourceCollection requiredResources, Stack<Resource> resourcesToConsider,
                                                              Stack<(Resource resource, int countFromActivePlayer, int countFromLeftNeighbour, int countFromRightNeighbour)> resourceCounts)
        {
            if (resourcesToConsider.Count == 0)
            {
                distributions.Add(new ResourceDistribution(resourceCounts));
                return;
            }

            var resource = resourcesToConsider.Pop();
            
            int count = requiredResources.GetResourceCount(resource);
            var distributionsOfThisResource = CalculateAllTripleDistributions(count);
            foreach (var distributionOfThisResource in distributionsOfThisResource)
            {
                resourceCounts.Push((resource, distributionOfThisResource.Item1, distributionOfThisResource.Item2, distributionOfThisResource.Item3));
                RecursiveAddResourceDistributions(distributions, requiredResources, resourcesToConsider, resourceCounts);
                resourceCounts.Pop();
            }

            resourcesToConsider.Push(resource);
        }
    }
}
