using System.Linq;
using Godot;
using _7Wonders;
using System.Collections.Generic;

public class Gameplay : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		const int playerCount = 7;

		godotHumanPlayer = new GodotHumanPlayer("Human");
		var handArea = GetNode<HandArea>("HandArea");
		handArea.ActionChosen = godotHumanPlayer.OnActionChosen;
		godotHumanPlayer.NewHand = handArea.OnNewHand;

		InitializeGame(playerCount);
		InitializePlayerAreas(playerCount);

		AdvanceGame();
	}

	private void InitializeGame(int playerCount)
	{
		var availableTableaus = new StartingTableauCollection("..\\Cities.xml");
		var allCards = new CardCollection("..\\Cards.xml");
		var playerFactory = new RobotPlayerFactory("..\\Robots.xml");

		var playerAgents = new List<PlayerAgent>();
		playerAgents.Add(godotHumanPlayer);
		playerAgents.AddRange(Enumerable.Range(0, playerCount - 1)
									 	.Select(i => playerFactory.CreatePlayer($"Robot {i + 1}", 'A')));

		game = new Game(playerAgents, availableTableaus, allCards);
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

	private void AdvanceGame()
	{
		new System.Threading.Thread(() =>
		{
			var gameTurn = game.PlayTurn();
			
			var playerActions = gameTurn.playerActions.ToArray();
			for (int i = 0; i < playerActions.Length; ++i)
			{
				playerAreas[i].HandleAction(playerActions[i]);
			}

			// TODO: Handle military results in the militaryResults property.

			// TODO: Display the leaderboard in a nice way. For now it goes to debug output.
			var leaderboard = game.Leaderboard.ToArray();
			for (int i = 0; i < leaderboard.Length; ++i)
			{
				GD.Print($"{i + 1}. {leaderboard[i].player.CityName} ({leaderboard[i].player.Name}): {leaderboard[i].victoryPoints}");
			}
			GD.Print();

			if (!game.IsGameOver)
			{
				AdvanceGame();
			}
		}).Start();
	}

	private PlayerArea[] playerAreas;
	private Game game;
	private GodotHumanPlayer godotHumanPlayer;
}
