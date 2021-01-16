using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// A 7 Wonders card. There only needs to be one of each card (i.e. even though there are multiple Stone Pits cards, they can
    /// all use the same Card instance).
    /// </summary>
    public class Card
    {
        public string Name { get; }
        public Colour Colour { get; }
        public int[] NumberByPlayerCount { get; }
        public int Age { get; }
        public int CoinGain { get; }
        public int VictoryPoints { get; }
        public int Military { get; }
        public ScienceSymbol? Science { get; }
        public Cost Cost { get; }
        public Production Production { get; }
        public TradeBonus TradeBonus { get; }

        public IReadOnlyCollection<Gain> EvaluatedGains { get; }

        public Card(XmlElement cardElement)
        {
            Name = cardElement.GetAttribute("name");
            Colour = cardElement.GetAttribute_Enum<Colour>("colour").Value;
            NumberByPlayerCount = ReadNumberByPlayerCount(cardElement);
            Age = cardElement.GetAttribute_Int("age");
            CoinGain = cardElement.GetAttribute_Int("coins");
            VictoryPoints = cardElement.GetAttribute_Int("victoryPoints");
            Military = cardElement.GetAttribute_Int("military");
            Science = cardElement.GetAttribute_Enum<ScienceSymbol>("science");
            Cost = new Cost(cardElement.GetChildElement("Cost"));
            Production = new Production(cardElement.GetChildElements("Production"));
            var tradeElement = cardElement.GetChildElement("Trade");
            if (tradeElement != null)
            {
                TradeBonus = new TradeBonus(tradeElement);
            }

            EvaluatedGains = cardElement.GetChildElements("Gain").Select(gainElement => new Gain(gainElement)).ToList();
        }

        public int EvaluateVictoryPoints(Player actingPlayer, Player leftNeightbour, Player rightNeighbour)
        {
            return VictoryPoints + EvaluatedGains.Sum(gain => gain.GetVictoryPointsGained(actingPlayer, leftNeightbour, rightNeighbour));
        }

        private static int[] ReadNumberByPlayerCount(XmlElement cardElement)
        {
            var increaseOnPlayerCount = cardElement.GetAttribute_IntSequence("players");
            return Enumerable.Range(0, 8).Select(i => increaseOnPlayerCount.Count(x => i >= x)).ToArray();
        }
    }
}
