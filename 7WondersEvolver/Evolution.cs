namespace _7Wonders
{
    internal class Evolution
    {
        public int Generation { get; private set; } = 0;

        public Evolution(int playerCount, int gamesPerGeneration)
        {
            availableTableaus = new StartingTableauCollection("..\\..\\..\\Cities.xml");
            allCards = new CardCollection("..\\..\\..\\Cards.xml");
            players = new PlayerPool(playerCount, availableTableaus.CityNames);
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
