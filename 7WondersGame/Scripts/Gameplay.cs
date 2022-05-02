using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;

public class Gameplay : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		const int playerCount = 7;

		// Either set the city board for the human to play, or set it to null for random.
		//const string humanCity = "Halikarnassos";
		const string humanCity = null;

		godotHumanPlayer = new GodotHumanPlayer("Human");

		InitializeGame(playerCount, humanCity);
		InitializePlayerAreas(playerCount);
		InitializeWindows();

		AdvanceGame();
	}

	private void InitializeGame(int playerCount, string humanCity)
	{
		var availableTableaus = new StartingTableauCollection("..\\Cities.xml");
		var allCards = new CardCollection("..\\Cards.xml");
		var playerFactory = new RobotPlayerFactory("..\\Robots.xml");

		var playerAgents = new List<PlayerAgent>();
		playerAgents.Add(godotHumanPlayer);
		playerAgents.AddRange(Enumerable.Range(0, playerCount - 1)
									 	.Select(i => playerFactory.CreatePlayer($"Robot {i + 1}", 'A')));

		game = new Game(playerAgents, availableTableaus, allCards, humanCity);
	}

	private void InitializePlayerAreas(int playerCount)
	{
		var playerAreaScene = ResourceLoader.Load<PackedScene>("res://Scenes/PlayerArea.tscn");

		playerAreas = Enumerable.Range(0, playerCount)
								.Select(i => 
										{
											var instance = playerAreaScene.Instance<PlayerArea>();
											instance.Player = game.GetPlayer(i);
											instance.LeftNeighbour = game.GetPlayer((i + 1) % playerCount);
											instance.RightNeighbour = game.GetPlayer((i - 1 + playerCount) % playerCount);
											return instance;
										})
								.ToArray();

		// Position the player areas.
		GetNode("ActivePlayer").AddChild(playerAreas[0]);
		if (playerCount >= 2)
		{
			GetNode("LeftNeighbour").AddChild(playerAreas[1]);
		}
		if (playerCount >= 3)
		{
			GetNode("RightNeighbour").AddChild(playerAreas[playerCount - 1]);
		}
		switch (playerCount)
		{
			case 4:
				GetNode("OppositePlayers/Opposite3").AddChild(playerAreas[2]);
				break;

			case 5:
				GetNode("OppositePlayers/Opposite2").AddChild(playerAreas[2]);
				GetNode("OppositePlayers/Opposite4").AddChild(playerAreas[3]);
				break;

			case 6:
				GetNode("OppositePlayers/Opposite1").AddChild(playerAreas[2]);
				GetNode("OppositePlayers/Opposite3").AddChild(playerAreas[3]);
				GetNode("OppositePlayers/Opposite5").AddChild(playerAreas[4]);
				break;

			case 7:
				GetNode("OppositePlayers/Opposite0").AddChild(playerAreas[2]);
				GetNode("OppositePlayers/Opposite2").AddChild(playerAreas[3]);
				GetNode("OppositePlayers/Opposite4").AddChild(playerAreas[4]);
				GetNode("OppositePlayers/Opposite6").AddChild(playerAreas[5]);
				break;

			default:
				// No more players to add.
				break;	
		}
	}

	private void InitializeWindows()
	{
		var handAreaScene = ResourceLoader.Load<PackedScene>("res://Scenes/HandArea.tscn");
		var discardsAreaScene = ResourceLoader.Load<PackedScene>("res://Scenes/DiscardsArea.tscn");
		var leaderboardScene = ResourceLoader.Load<PackedScene>("res://Scenes/Leaderboard.tscn");

		var handArea = handAreaScene.Instance<HandArea>();
		handArea.ActionChosen = godotHumanPlayer.OnActionChosen;
		godotHumanPlayer.NewHand = handArea.ShowNewHand;
		AddChild(handArea);

		var discardsArea = discardsAreaScene.Instance<DiscardsArea>();
		discardsArea.CardChosen = godotHumanPlayer.OnDiscardBuildChosen;
		godotHumanPlayer.DiscardsBuild = discardsArea.OnDiscardsBuild;
		AddChild(discardsArea);

		leaderboard = leaderboardScene.Instance<Leaderboard>();
		AddChild(leaderboard);
	}

	private void AdvanceGame()
	{
		new System.Threading.Thread(() =>
		{
			var gameTurn = game.PlayTurn();
			
			HandleActions(gameTurn.playerActions);
			HandleActions(gameTurn.additionalPlayerActions);

			if (gameTurn.militaryResults != null)
			{
				HandleMilitaryResults(gameTurn.militaryResults);
			}

			if (game.IsGameOver)
			{
				CallDeferred("ShowLeaderboard");
			}
			else
			{
				AdvanceGame();
			}
		}).Start();
	}

	private void HandleActions(IReadOnlyCollection<IAction> playerActions)
	{
		var playerActionsArray = playerActions.ToArray();
		for (int i = 0; i < playerActionsArray.Length; ++i)
		{
			if (playerActionsArray[i] != null)
			{
				playerAreas[i].HandleAction(playerActionsArray[i]);
			}
		}
	}

	private void HandleMilitaryResults(IReadOnlyCollection<MilitaryResult> militaryResults)
	{
		foreach (var militaryResult in militaryResults)
		{
			int age = (militaryResult.winningVictoryPoints + 1) / 2;
			GetPlayerArea(militaryResult.winningPlayer).AddMilitaryResult(age, victory: true);
			GetPlayerArea(militaryResult.losingPlayer).AddMilitaryResult(age, victory: false);
		}
	}

	private PlayerArea GetPlayerArea(Player player)
	{
		for (int i = 0; i < playerAreas.Length; ++i)
		{
			if (game.GetPlayer(i).Name == player.Name)
			{
				return playerAreas[i];
			}
		}
		throw new System.Exception($"Failed to find player area for player named '{player.Name}'");
	}

	private void ShowLeaderboard()
	{
		leaderboard.ShowLeaderboard(game.Leaderboard);
	}

	private PlayerArea[] playerAreas;
	private Leaderboard leaderboard;

	private Game game;
	private GodotHumanPlayer godotHumanPlayer;
}
