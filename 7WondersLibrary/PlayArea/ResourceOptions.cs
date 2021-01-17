using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// Because some cards give a player the choice of which resource that card produces (for a given play), we need
    /// to represent all possible ways that a player can generate resources for his play.
    /// </summary>
    internal class ResourceOptions
    {
        public ResourceOptions()
        {
            resourceOptions = new List<ResourceCollection> { new ResourceCollection() };
        }

        public ResourceOptions(ResourceOptions source)
        {
            resourceOptions = new List<ResourceCollection>(source.resourceOptions);
        }

        public void AddProduction(Production production)
        {
            if (production.Any())
            {
                resourceOptions = production.Combine(resourceOptions).ToList();
            }
        }

        public bool HasResources(ResourceCollection resources)
        {
            return resourceOptions.Any(x => resources <= x);
        }

        private List<ResourceCollection> resourceOptions;
    }
}
