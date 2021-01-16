using System.Collections.Generic;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents the trading discount that a card like East Trading Post can provide.
    /// </summary>
    public class TradeBonus
    {
        public TradeBonus(XmlElement tradeElement)
        {
            tradeCost = tradeElement.GetAttribute_Int("cost");
            directions = tradeElement.GetAttribute_EnumSet<Direction>("direction");
            resources = tradeElement.GetAttribute_EnumSet<Resource>("resources");
        }

        public int TradeCost(Resource resource, Direction direction)
        {
            if (directions.Contains(direction) && resources.Contains(resource))
            {
                return tradeCost;
            }
            return 2;   // No bonus to this resource in this direction, so the appicable trade cost is 2. (Other cards may improve this.)
        }

        private int tradeCost;
        private ISet<Direction> directions;
        private ISet<Resource> resources;
    }
}
