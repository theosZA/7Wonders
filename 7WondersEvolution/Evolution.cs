﻿using _7Wonders;

namespace _7WondersEvolution
{
    internal class Evolution
    {
        public int Generation { get; private set; } = 0;

        public Evolution(int playerCount, int gamesPerGeneration)
        {
            availableTableaus = new StartingTableauCollection("..\\..\\..\\Cities.xml");
            allCards = new CardCollection("..\\..\\..\\Cards.xml");
            players = new PlayerPool(playerCount);
            this.gamesPerGeneration = gamesPerGeneration;
        }

        public PlayerPool GetCopyOfPlayers()
        {
            return new PlayerPool(players);
        }

        public void AdvanceGeneration()
        {
            if (Generation > 0)
            {
                players.ReplaceWithNewGeneration();
            }
            players.PlayGamesWithRandomPlayers(gameCount: gamesPerGeneration, playerCount: 7, availableTableaus, allCards);
            ++Generation;
        }

        private readonly PlayerPool players;
        private readonly StartingTableauCollection availableTableaus;
        private readonly CardCollection allCards;
        private int gamesPerGeneration;
    }
}
