using System.Collections.Generic;

namespace _7Wonders
{
    internal class DistributionHelper
    {
        public IEnumerable<ResourceDistribution> ResourceDistributions
        {
            get
            {
                if (resourceDistributions == null)
                {
                    CalculateAllResourceDistributions();
                }
                return resourceDistributions;
            }
        }

        public DistributionHelper(ResourceCollection requiredResources, Tableau activePlayer, Tableau leftNeighbour, Tableau rightNeighbour)
        {
            this.requiredResources = requiredResources;
            this.activePlayer = activePlayer;
            this.leftNeighbour = leftNeighbour;
            this.rightNeighbour = rightNeighbour;
        }

        /// <summary>
        /// Finds all possible ways of dividing the required resources between 3 players.
        /// </summary>
        private void CalculateAllResourceDistributions()
        {
            resourceDistributions = new List<ResourceDistribution>();
            RecursiveAddResourceDistributions(new Stack<Resource>(requiredResources.GetResources()),
                                              new Stack<(Resource resource, int countFromActivePlayer, int countFromLeftNeighbour, int countFromRightNeighbour)>());
        }

        /// <summary>
        /// Finds all possible ways of dividing the required resources between 3 players given that the resources not in resourcesToConsider have already been
        /// divided as determined by resourceCounts.
        /// </summary>
        private void RecursiveAddResourceDistributions(Stack<Resource> resourcesToConsider, Stack<(Resource resource, int countFromActivePlayer, int countFromLeftNeighbour, int countFromRightNeighbour)> resourceCounts)
        {
            if (resourcesToConsider.Count == 0)
            {
                resourceDistributions.Add(new ResourceDistribution(resourceCounts));
                return;
            }

            var resource = resourcesToConsider.Pop();
            int count = requiredResources.GetResourceCount(resource);
            foreach (var distributionOfThisResource in CalculateAllTripleDistributions(resource, count))
            {
                resourceCounts.Push((resource, distributionOfThisResource.Item1, distributionOfThisResource.Item2, distributionOfThisResource.Item3));
                RecursiveAddResourceDistributions(resourcesToConsider, resourceCounts);
                resourceCounts.Pop();
            }

            resourcesToConsider.Push(resource);
        }

        /// <summary>
        /// Finds all theoretically possible distributions of the given resource among the three players.
        /// </summary>
        private IEnumerable<(int, int, int)> CalculateAllTripleDistributions(Resource resource, int total)
        {
            for (int rightCount = 0; rightCount <= total && rightNeighbour.HasResourcesForTrade(new ResourceCollection(resource, rightCount)); ++rightCount)
            {
                for (int leftCount = 0; leftCount <= total - rightCount && leftNeighbour.HasResourcesForTrade(new ResourceCollection(resource, leftCount)); ++leftCount)
                {
                    int activeCount = total - (leftCount + rightCount);
                    if (activePlayer.HasResources(new ResourceCollection(resource, activeCount)))
                    {
                        yield return (activeCount, leftCount, rightCount);
                    }
                }
            }
        }

        private ResourceCollection requiredResources;
        private Tableau activePlayer;
        private Tableau leftNeighbour;
        private Tableau rightNeighbour;

        private IList<ResourceDistribution> resourceDistributions;
    }
}
