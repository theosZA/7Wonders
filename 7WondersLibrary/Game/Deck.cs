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
            this.cards = new Queue<Card>(cards.Shuffle());
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
