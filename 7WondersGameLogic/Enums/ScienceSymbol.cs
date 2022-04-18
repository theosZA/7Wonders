using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// Enum for the 3 science symbols, and a wild value which can substitute for any of them.
    /// </summary>
    public enum ScienceSymbol
    {
        Tablet,
        Compass,
        Cog,
        Wild
    }

    internal static class ScienceSymbolHelper
    {
        public static IEnumerable<ScienceSymbol> GetAllBasicScienceSymbols()
        {
            return Enum.GetValues(typeof(ScienceSymbol))
                       .Cast<ScienceSymbol>()
                       .Take(3);
        }
    }
}