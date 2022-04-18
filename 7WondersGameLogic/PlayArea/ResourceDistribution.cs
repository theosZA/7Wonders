using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// A distribution of resources between a player and that player's two neighbours.
    /// </summary>
    internal class ResourceDistribution
    {
        public ResourceCollection ResourcesFromActivePlayer { get; }
        public ResourceCollection ResourcesFromLeftNeighbour { get; }
        public ResourceCollection ResourcesFromRightNeighbour { get; }

        public ResourceDistribution(IEnumerable<(Resource resource, int countFromActivePlayer, int countFromLeftNeighbour, int countFromRightNeighbour)> resourceCounts)
        {
            ResourcesFromActivePlayer = new ResourceCollection(resourceCounts.Select(x => (x.resource, x.countFromActivePlayer)));
            ResourcesFromLeftNeighbour = new ResourceCollection(resourceCounts.Select(x => (x.resource, x.countFromLeftNeighbour)));
            ResourcesFromRightNeighbour = new ResourceCollection(resourceCounts.Select(x => (x.resource, x.countFromRightNeighbour)));
        }

        public bool SatisfiedBy(Tableau activePlayer, Tableau leftNeighbour, Tableau rightNeighbour)
        {
            return activePlayer.HasResources(ResourcesFromActivePlayer)
                && leftNeighbour.HasResourcesForTrade(ResourcesFromLeftNeighbour)
                && rightNeighbour.HasResourcesForTrade(ResourcesFromRightNeighbour);
        }
    }
}
