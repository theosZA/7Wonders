using System.Collections.Generic;
using System.IO;
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
		var playerAgents = new List<PlayerAgent>();
		playerAgents.Add(humanPlayer);
		playerAgents.AddRange(Enumerable.Range(0, instance.playerCount - 1)
									 	.Select(i =>instance.robotPlayerFactory.CreatePlayer($"Robot {i + 1}", 'A')));

		return new Game(playerAgents, instance.availableTableaus, instance.allCards, instance.wonderChoice);
    }

    private static readonly GameFactory instance = new GameFactory();

    static GameFactory()    
    {}

    private GameFactory()    
    {}   

    private int playerCount = 7;
    private string wonderChoice;

    private StartingTableauCollection availableTableaus = File.Exists("Cities.xml") ? new StartingTableauCollection("Cities.xml") : new StartingTableauCollection("..\\Cities.xml");
    private CardCollection allCards = File.Exists("Cards.xml") ? new CardCollection("Cards.xml") : new CardCollection("..\\Cards.xml");
    private RobotPlayerFactory robotPlayerFactory = File.Exists("Robots.xml") ? new RobotPlayerFactory("Robots.xml") : new RobotPlayerFactory("..\\Robots.xml");
}    