using Extensions;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents the cost of playing a card: either a resource cost plus coin cost OR what card must be in the tableau to play for free.
    /// </summary>
    public class Cost
    {
        public ResourceCollection Resources { get; }
        public int Coins { get; }
        public string FreeWithOtherCard { get; }

        public Cost(XmlElement costElement)
        {
            if (costElement == null)
            {
                Resources = new ResourceCollection();
            }
            else
            {
                Resources = new ResourceCollection(costElement);
                Coins = costElement.GetAttribute_Int("coins");
                FreeWithOtherCard = costElement.GetAttribute_String("free");
            }
        }
    }
}
