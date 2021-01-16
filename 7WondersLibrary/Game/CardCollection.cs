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
    internal class CardCollection
    {
        public CardCollection(XmlElement cardsElement)
        {
            cards = cardsElement.GetChildElements("Card").Select(cardElement => new Card(cardElement)).ToList();
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

        private IReadOnlyCollection<Card> cards;
    }
}
