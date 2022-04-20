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
		playerAgents.Add(new GodotHumanPlayer("Human", this));
		playerAgents.AddRange(Enumerable.Range(0, playerCount - 1)
									 	.Select(i => playerFactory.CreatePlayer($"Robot {i + 1}", 'A')));

		game = new Game(playerAgents, availableTableaus, allCards);
	}

	private void InitializePlayerAreas(int playerCount)
	{
		playerAreas = Enumerable.Range(0, playerCount)
								.Select(i => new PlayerArea(this,
															GetViewportRect(),
															game.GetPlayer(i),
															game.GetPlayer((i + 1) % playerCount),
															game.GetPlayer((i - 1 + playerCount) % playerCount)))
								.ToArray();

		// Position the player areas.

		const float primaryAreaScale = 0.45f;
		const float neighbourScale = 0.25f;
		const float otherScale = 0.2f;
		
		// The primary player area (currently area 0) should be front and center.
		playerAreas[0].ApplyScale(primaryAreaScale);
		playerAreas[0].SetPosition(0.456f, 0.78f);

		// Left-hand neighbour.
		if (playerCount >= 2)
		{
			playerAreas[1].ApplyScale(neighbourScale);
			playerAreas[1].SetPosition(0.106f, 0.6f);
		}

		// Right-hand neighbour.
		if (playerCount >= 3)
		{
			playerAreas[playerCount - 1].ApplyScale(neighbourScale);
			playerAreas[playerCount - 1].SetPosition(0.846f, 0.6f);
		}

		// Other players (not neighbours).
		float widthFractionPerPlayer = 1.0f / (playerCount - 2.5f);
		for (int i = 2; i < playerCount - 1; ++i)
		{
			playerAreas[i].ApplyScale(otherScale);
			playerAreas[i].SetPosition((i - 1.3f) * widthFractionPerPlayer, 0.23f);
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
}
