using Extensions;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// A 7 Wonders card. There only needs to be one of each card (i.e. even though there are multiple Stone Pits cards, they can
    /// all use the same Card instance).
    /// </summary>
    public class Card : TableauElement
    {
        public Colour Colour { get; }
        public int[] NumberByPlayerCount { get; }
        public int Age { get; }

        public Card(XmlElement cardElement)
        : base(cardElement.GetAttribute("name"), cardElement)
        {
            Colour = cardElement.GetAttribute_Enum<Colour>("colour").Value;
            NumberByPlayerCount = ReadNumberByPlayerCount(cardElement);
            Age = cardElement.GetAttribute_Int("age");
        }

        private static int[] ReadNumberByPlayerCount(XmlElement cardElement)
        {
            var increaseOnPlayerCount = cardElement.GetAttribute_IntSequence("players");
            return Enumerable.Range(0, 8).Select(i => increaseOnPlayerCount.Count(x => i >= x)).ToArray();
        }
    }
}
