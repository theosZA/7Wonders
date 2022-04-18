using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents a collection of resources, potentially across multiple resource types and any number of those resource types.
    /// </summary>
    public class ResourceCollection
    {
        public ResourceCollection()
        { }

        public ResourceCollection(Resource resource, int count)
        {
            resources = new Dictionary<Resource, int> { { resource, count } };
        }

        public ResourceCollection(IEnumerable<(Resource resource, int count)> resourceCounts)
        {
            resources = resourceCounts.ToDictionary(x => x.resource, x => x.count);
        }

        public ResourceCollection(ResourceCollection source)
        {
            resources = new Dictionary<Resource, int>(source.resources);
        }

        public ResourceCollection(XmlElement xmlElement)
        {
            foreach (var resource in ResourceHelper.GetAllResources())
            {
                var amount = xmlElement.GetAttribute_Int(resource.ToString().ToLowerInvariant());
                if (amount > 0)
                {
                    resources.Add(resource, amount);
                }
            }
        }

        public bool Any()
        {
            return resources.Any();
        }

        public IEnumerable<Resource> GetResources()
        {
            return resources.Keys;
        }

        public int GetResourceCount(Resource resource)
        {
            if (!resources.ContainsKey(resource))
            {
                return 0;
            }
            return resources[resource];
        }

        public int Sum()
        {
            return resources.Sum(resourceCount => resourceCount.Value);
        }

        public int Sum(Func<Resource, int, int> sumFunction)
        {
            return resources.Sum(resourceCount => sumFunction(resourceCount.Key, resourceCount.Value));
        }

        public void Add(Resource resource, int count)
        {
            if (resources.ContainsKey(resource))
            {
                resources[resource] += count;
            }
            else 
            {
                resources.Add(resource, count);
            }
        }

        public void Remove(Resource resource, int count)
        {
            if (!resources.ContainsKey(resource) || resources[resource] < count)
            {
                throw new InvalidOperationException("Trying to remove resources that are not available in the collection");
            }
            if ((resources[resource] -= count) == 0)
            {
                resources.Remove(resource);
            }            
        }

        public override string ToString()
        {
            return string.Join(", ", resources.Select(resourceCostPair => $"{resourceCostPair.Value} {resourceCostPair.Key}"));
        }

        /// <summary>
        /// True if for every resource, the LHS amount is less than or equal to the RHS amount.
        /// Missing resources from either LHS or RHS are assumed to be 0.
        /// </summary>
        public static bool operator <=(ResourceCollection lhs, ResourceCollection rhs)
        {
            return lhs.resources.All(pair => pair.Value == 0 || rhs.resources.ContainsKey(pair.Key) && pair.Value <= rhs.resources[pair.Key]);
        }

        /// <summary>
        /// True if for every resource, the LHS amount is greater than or equal to the RHS amount.
        /// Missing resources from either LHS or RHS are assumed to be 0.
        /// </summary>
        public static bool operator >=(ResourceCollection lhs, ResourceCollection rhs)
        {
            return rhs.resources.All(pair => pair.Value == 0 || lhs.resources.ContainsKey(pair.Key) && pair.Value <= lhs.resources[pair.Key]);
        }

        public static ResourceCollection operator +(ResourceCollection lhs, ResourceCollection rhs)
        {
            var result = new ResourceCollection();
            result.resources.AddUpdate(lhs.resources);
            result.resources.AddUpdate(rhs.resources);
            return result;
        }

        private IDictionary<Resource, int> resources = new Dictionary<Resource, int>();
    }
}
