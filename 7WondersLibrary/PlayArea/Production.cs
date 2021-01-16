using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents the production made available by a single card (or a player's wonder board).
    /// </summary>
    public class Production
    {
        public Production(IEnumerable<XmlElement> productionElements)
        {
            resourceProductionOptions = productionElements.Select(productionElement => new ResourceCollection(productionElement))
                                                          .ToList();
        }

        public bool Any()
        {
            return resourceProductionOptions.Any() && resourceProductionOptions.First().Any();
        }

        public Resource GetFirstResource()
        {
            return resourceProductionOptions.First().GetResources().First();
        }

        /// <summary>
        /// Returns the amount of the resource produced by this card as long as it's the only option, and 0 otherwise.
        /// </summary>
        public int GetSingleProduction(Resource resource)
        {
            if (resourceProductionOptions.Count() != 1)
            {
                return 0;
            }
            return resourceProductionOptions.First().GetResourceCount(resource);
        }

        /// <summary>
        /// If there is more than one production option and all production options produce a single unit of a resource, then a sequence of all possible produced resources are returned.
        /// Otherwise null is returned.
        /// </summary>
        public IEnumerable<Resource> GetMultipleProduction()
        {
            if (resourceProductionOptions.Count() < 2)
            {   // Fewer than 2 production options, so not a multiple production.
                return null;
            }
            if (resourceProductionOptions.Any(production => production.Sum() > 1))
            {   // A production option produces more than one type of resource or more than one a single resource.
                return null;
            }
            return resourceProductionOptions.Select(productionMapping => productionMapping.GetResources().First());
        }

        public IEnumerable<ResourceCollection> Combine(IEnumerable<ResourceCollection> newResources)
        {
            return resourceProductionOptions.SelectMany(x => newResources, (x, y) => x + y).Distinct();

        }

        private IReadOnlyCollection<ResourceCollection> resourceProductionOptions;
    }
}
