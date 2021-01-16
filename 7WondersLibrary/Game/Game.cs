using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace _7Wonders
{
    /// <summary>
    /// Class used for playing an entire game of 7 Wonders.
    /// Call PlayTurn() repeatedly until IsGameOver is true.
    /// </summary>
    public class Game
    {
        public bool IsGameOver => age == 3 && players.CardsInHand == 0;

        public Game(int playerCount, int robotCount)
        {
            var cardsXmlDocument = new XmlDocument();
            cardsXmlDocument.Load("Cards.xml");
            allCards = new CardCollection((XmlElement)cardsXmlDocument.SelectSingleNode("Cards"));

            var citiesXmlDocument = new XmlDocument();
            citiesXmlDocument.Load("Cities.xml");
            var citiesElement = (XmlElement)citiesXmlDocument.SelectSingleNode("Cities");
            var cityElements = citiesElement.GetChildElements("City");

            // For now we just use one common tableau, replicated across all our players. TBD implement all city boards.
            var tableaus = Enumerable.Range(0, 7).Select(i => new Tableau(cityElements.First())).ToList();

            players = new PlayerCollection(playerCount, robotCount, tableaus);

            StartAge(1);
        }

        public void WriteStateToConsole()
        {
            Console.WriteLine($"Age {age}");
            Console.WriteLine();
            players.WriteStateToConsole();
            if (IsGameOver)
            {
                players.WriteLeaderboardToConsole();
            }
            Console.WriteLine();
        }

        public GameTurn PlayTurn()
        {
            // Actions

            var actions = players.GetActions();
            players.ApplyActions(actions, discards);

            // Check for end of age

            IReadOnlyCollection<MilitaryResult> militaryResults = null;
            if (players.CardsInHand == 1)
            {
                militaryResults = EndAge();
                if (age < 3)
                {
                    StartAge(age + 1);
                }
            }
            else
            {
                players.PassHands(age == 2 ? Direction.Right : Direction.Left);
            }

            return new GameTurn
            {
                playerActions = actions,
                militaryResults = militaryResults
            };
        }

        private void StartAge(int newAge)
        {
            age = newAge;

            var deck = CreateAgeDeck(newAge);
            players.DealDeck(deck);
        }

        private IReadOnlyCollection<MilitaryResult> EndAge()
        {
            players.DiscardHands(discards);
            return players.EvaluateMilitaryBattles(scoreForVictory: (2 * age) - 1, scoreForDefeat: -1);
        }

        private Deck CreateAgeDeck(int age)
        {
            var ageCards = allCards.GetCardsForAge(age, players.Count).ToList();
            var cardsForDeck = ageCards.Where(card => card.Colour != Colour.Purple).ToList();
            if (age == 3)
            {
                var guildCards = ageCards.Where(card => card.Colour == Colour.Purple).ToList();
                guildCards.Shuffle();
                cardsForDeck.AddRange(guildCards.Take(players.Count + 2));
            }
            return new Deck(cardsForDeck);
        }

        private PlayerCollection players;
        private List<Card> discards = new List<Card>();
        private int age;
        private CardCollection allCards;
    }
}
