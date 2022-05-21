namespace _7Wonders
{
    internal class Evolution
    {
        public int Generation { get; private set; } = 0;

        public Evolution(int playersPerCity, int gamesPerGeneration, BoardSide boardSide)
        {
            this.playersPerCity = playersPerCity;
            this.gamesPerGeneration = gamesPerGeneration;
            this.boardSide = boardSide;

            availableTableaus = new StartingTableauCollection("..\\..\\..\\Cities.xml");
            allCards = new CardCollection("..\\..\\..\\Cards.xml");
            players = new PlayerPool(playersPerCity, availableTableaus.CityNames);
        }

        public PlayerPool GetCopyOfPlayers()
        {
            return new PlayerPool(players);
        }

        public void AdvanceGeneration()
        {
            ++Generation;
            if (Generation > 1)
            {
                players.ReplaceWithNewGeneration(Generation, playersPerCity);
            }
            players.PlayGamesWithRandomPlayers(gameCount: gamesPerGeneration, playerCount: 7, availableTableaus, allCards, boardSide);
        }

        private readonly PlayerPool players;
        private readonly StartingTableauCollection availableTableaus;
        private readonly CardCollection allCards;
        private readonly int playersPerCity;
        private readonly int gamesPerGeneration;
        private readonly BoardSide boardSide;
    }
}
