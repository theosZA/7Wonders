using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// Enum for the 4 raw and 3 manufactered resource types in the game.
    /// </summary>
    public enum Resource
    {
        Clay,
        Ore,
        Stone,
        Wood,
        Glass,
        Loom,
        Papyrus
    }

    internal static class ResourceHelper
    {
        public static IEnumerable<Resource> GetAllResources()
        {
            return Enum.GetValues(typeof(Resource)).Cast<Resource>();
        }

        public static bool IsRawMaterial(Resource resource)
        {
            return resource == Resource.Clay || resource == Resource.Ore || resource == Resource.Stone || resource == Resource.Wood;
        }

        public static bool IsManufacteredGood(Resource resource)
        {
            return resource == Resource.Glass || resource == Resource.Loom || resource == Resource.Papyrus;
        }
    }
}
