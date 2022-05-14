using System.Collections.Generic;
using System.Linq;
using _7Wonders;

public sealed class GameFactory    
{    
    public static int PlayerCount
    {
        get => instance.playerCount;

        set
        {
            instance.playerCount = value;
        }
    }

    public static string WonderChoice
    {
        get => instance.wonderChoice;

        set
        {
            instance.wonderChoice = value;
        }
    }

    public static IEnumerable<string> AvailableWonders => instance.availableTableaus.CityNames;

    public static Game CreateGame(PlayerAgent humanPlayer)
    {
        return instance.CreateGameInternal(humanPlayer);
    }

    private static readonly GameFactory instance = new GameFactory();

    static GameFactory()    
    {}

    private GameFactory()    
    {}   

    private Game CreateGameInternal(PlayerAgent humanPlayer)
    {
		var playerAgents = new List<PlayerAgent>();
		playerAgents.Add(humanPlayer);
		playerAgents.AddRange(Enumerable.Range(0, playerCount - 1)
									 	.Select(i => robotPlayerFactory.CreatePlayer($"Robot {i + 1}", 'A')));

		return new Game(playerAgents, availableTableaus, allCards, wonderChoice);
    }

    private int playerCount = 7;
    private string wonderChoice;

    private StartingTableauCollection availableTableaus = new StartingTableauCollection("..\\Cities.xml");
    private CardCollection allCards = new CardCollection("..\\Cards.xml");
    private RobotPlayerFactory robotPlayerFactory = new RobotPlayerFactory("..\\Robots.xml");
}    