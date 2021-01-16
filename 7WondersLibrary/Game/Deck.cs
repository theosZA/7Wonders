using System.Collections.Generic;

namespace _7Wonders
{
    /// <summary>
    /// A shuffled deck of cards.
    /// </summary>
    internal class Deck
    {
        public Deck(IEnumerable<Card> cards)
        {
            var newDeck = new List<Card>(cards);
            newDeck.Shuffle();
            this.cards = new Queue<Card>(newDeck);
        }

        public Card DrawNextCard()
        {
            return cards.Dequeue();
        }

        public IEnumerable<Card> DrawCards(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return cards.Dequeue();
            }
        }

        private Queue<Card> cards;
    }
}
