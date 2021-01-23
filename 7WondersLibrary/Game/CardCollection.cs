using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Represents all cards possible in the game, whether they end up being used or not.
    /// Only one of each card is in the collection, but calling GetCardsForAge() can
    /// return multiples of the same card instance if there are enough players.
    /// </summary>
    public class CardCollection
    {
        public CardCollection(string cardsXmlFileName)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(cardsXmlFileName);
            cards = ReadCardsFromXml(xmlDocument.DocumentElement).ToList();
        }

        public CardCollection(XmlElement cardsElement)
        {
            cards = ReadCardsFromXml(cardsElement).ToList();
        }

        public IEnumerable<Card> GetCardsForAge(int age, int playerCount)
        {
            foreach (var card in cards.Where(card => card.Age == age))
            {
                for (int i = 0; i < card.NumberByPlayerCount[playerCount]; ++i)
                {
                    yield return card;
                }
            }
        }

        private static IEnumerable<Card> ReadCardsFromXml(XmlElement cardsElement)
        {
            return cardsElement.GetChildElements("Card").Select(cardElement => new Card(cardElement));
        }

        private IReadOnlyCollection<Card> cards;
    }
}
