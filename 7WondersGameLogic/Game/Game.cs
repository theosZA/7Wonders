using Extensions;
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
        public bool IsGameOver => Age == 3 && players.CardsInHand == 0;

        public int Age { get; private set; }

        public int[] VictoryPoints => players.VictoryPoints.ToArray();

        public int[] Positions => players.Positions.ToArray();

        public IReadOnlyCollection<(Player player, int victoryPoints)> Leaderboard => players.Leaderboard;

        public Game(IReadOnlyCollection<PlayerAgent> playerAgents, StartingTableauCollection availableTableaus, CardCollection allCards, BoardSide boardSide, string firstCityOverride = null)
        {
            this.allCards = allCards;
            players = new PlayerCollection(playerAgents, availableTableaus.GetTableausForSide(boardSide).ToList(), firstCityOverride);
            StartAge(1);
        }

        public Player GetPlayer(int playerIndex)
        {
            return players[playerIndex];
        }

        public string GetLeaderboardText()
        {
            return players.GetLeaderboardText();
        }

        public GameTurn PlayTurn()
        {
            var actions = players.GetActions().ToList();
            var additionalPlayerActions = actions.Select(action => (IAction)null).ToList();

            players.ApplyActions(actions, discards);

            if (players.CardsInHand == 1)
            {
                // Any player who is allowed an extra play in the age, makes that play now.
                // There should only ever be one such player (Babylon) so we only handle a single result.
                var extraAgePlay = players.GetExtraAgePlay();
                if (extraAgePlay.HasValue)
                {
                    additionalPlayerActions[extraAgePlay.Value.playerIndex] = extraAgePlay.Value.action;
                    players.ApplyAction(extraAgePlay.Value.playerIndex, extraAgePlay.Value.action, discards);
                }

                // Last card in hand is discarded.
                discards.AddRange(players.DiscardHands());
            }

            // Any player who can build from the discards, does so now.
            // There should only ever be one such player (Halikarnassos) so we only handle a single result.
            var buildFromDiscards = players.GetBuildFromDiscards(discards);
            if (buildFromDiscards.HasValue)
            {
                var action = new Build
                                 {
                                     Card = buildFromDiscards.Value.card,
                                     BuiltFromDiscards = true
                                 };
                additionalPlayerActions[buildFromDiscards.Value.playerIndex] = action;
                players.ApplyAction(buildFromDiscards.Value.playerIndex, action, discards);
            }

            IReadOnlyCollection<MilitaryResult> militaryResults = null;
            if (players.CardsInHand == 0)
            {   // End of Age.
                militaryResults = players.EvaluateMilitaryBattles(scoreForVictory: (2 * Age) - 1, scoreForDefeat: -1).ToList();
                if (Age < 3)
                {
                    StartAge(Age + 1);
                }
            }
            else
            {
                players.PassHands(Age == 2 ? Direction.Right : Direction.Left);
            }

            return new GameTurn
            {
                playerActions = actions,
                militaryResults = militaryResults,
                additionalPlayerActions = additionalPlayerActions
            };
        }

        private void StartAge(int newAge)
        {
            Age = newAge;

            players.StartAge(newAge);

            var deck = CreateAgeDeck(newAge);
            players.DealDeck(deck);
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
        private CardCollection allCards;
    }
}
