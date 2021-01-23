using System;
using System.Collections.Generic;
using System.Linq;

namespace _7Wonders
{
    /// <summary>
    /// Class used for playing an entire game of 7 Wonders.
    /// Call PlayTurn() repeatedly until IsGameOver is true.
    /// </summary>
    public class Game
    {
        public bool IsGameOver => age == 3 && players.CardsInHand == 0;

        public int[] VictoryPoints => players.VictoryPoints.ToArray();

        public int[] Positions => players.Positions.ToArray();

        public Game(IReadOnlyCollection<PlayerAgent> playerAgents, StartingTableauCollection availableTableaus, CardCollection allCards)
        {
            this.allCards = allCards;
            players = new PlayerCollection(playerAgents, availableTableaus.GetCopyOfTableaus().ToList());
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

            var actions = players.GetActions().ToList();
            players.ApplyActions(actions, discards);

            // Check for end of age

            IReadOnlyCollection<MilitaryResult> militaryResults = null;
            if (players.CardsInHand == 1)
            {
                militaryResults = EndAge().ToList();
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

        private IEnumerable<MilitaryResult> EndAge()
        {
            discards.AddRange(players.DiscardHands());
            return players.EvaluateMilitaryBattles(scoreForVictory: (2 * age) - 1, scoreForDefeat: -1);
        }

        private Deck CreateAgeDeck(int age)
        {
            var ageCards = allCards.GetCardsForAge(age, players.Count).ToList();
            var cardsForDeck = ageCards.Where(card => card.Colour != Colour.Purple).ToList();
            if (age == 3)
            {
                var guildCards = ageCards.Where(card => card.Colour == Colour.Purple)
                                         .Shuffle()
                                         .Take(players.Count + 2);
                cardsForDeck.AddRange(guildCards);
            }
            return new Deck(cardsForDeck);
        }

        private PlayerCollection players;
        private List<Card> discards = new List<Card>();
        private int age;
        private CardCollection allCards;
    }
}
