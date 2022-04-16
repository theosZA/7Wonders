using Godot;
using System.Collections;
using System.Linq;
using _7Wonders;

public class Gameplay : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		const int playerCount = 7;
		InitializeGame(playerCount);
		InitializePlayerAreas(playerCount);
	}

	public override void _Process(float delta)
	{
		// Nothing to do on every frame.		
	}

	private void InitializeGame(int playerCount)
	{
		var availableTableaus = new StartingTableauCollection("..\\Cities.xml");
		var allCards = new CardCollection("..\\Cards.xml");
		var playerAgents = Enumerable.Range(0, playerCount)
									 .Select(i => new DefaultPlayer($"Robot {i + 1}"))
									 .Cast<PlayerAgent>()
									 .ToList();
		game = new Game(playerAgents, availableTableaus, allCards);
	}

	private void InitializePlayerAreas(int playerCount)
	{
		playerAreas = Enumerable.Range(0, playerCount)
								.Select(i => CreatePlayerArea(i))
								.ToArray();

		// Position the player areas.

		const float primaryAreaScale = 0.45f;
		const float neighbourScale = 0.25f;
		const float otherScale = 0.2f;
		
		// The primary player area (currently area 0) should be front and center.
		playerAreas[0].ApplyScale(new Vector2(primaryAreaScale, primaryAreaScale));
		playerAreas[0].Translate(PositionFromViewportFraction(0.5f, 0.7f));

		// Left-hand neighbour.
		if (playerCount >= 2)
		{
			playerAreas[1].ApplyScale(new Vector2(neighbourScale, neighbourScale));
			playerAreas[1].Translate(PositionFromViewportFraction(0.13f, 0.5f));
		}

		// Right-hand neighbour.
		if (playerCount >= 3)
		{
			playerAreas[playerCount - 1].ApplyScale(new Vector2(neighbourScale, neighbourScale));
			playerAreas[playerCount - 1].Translate(PositionFromViewportFraction(0.87f, 0.5f));
		}

		// Other players (not neighbours).
		float widthFractionPerPlayer = 1.0f / (playerCount - 2.5f);
		for (int i = 2; i < playerCount - 1; ++i)
		{
			playerAreas[i].ApplyScale(new Vector2(otherScale, otherScale));
			playerAreas[i].Translate(PositionFromViewportFraction((i - 1.25f) * widthFractionPerPlayer, 0.2f));
		}
	}

	private Node2D CreatePlayerArea(int index)
	{
		var playerArea = new Node2D();

		playerArea.AddChild(CreateBoard(index));

		AddChild(playerArea);
		return playerArea;
	}

	private Sprite CreateBoard(int index)
	{
		var board = new Sprite();

		var cityName = game.GetPlayer(index).CityName;
		board.Texture = GD.Load<Texture>($"res://Art/PlayerBoard_{cityName}_A.jpg");
		float boardScale = GetViewportRect().Size.x / board.Texture.GetWidth();
		float boardToPlayerAreaScale = 0.9f * boardScale;	// The board should take up 90% of the horizontal width player area.
		board.ApplyScale(new Vector2(boardToPlayerAreaScale, boardToPlayerAreaScale));

		return board;
	}

	private Vector2 PositionFromViewportFraction(float fractionX, float fractionY)
	{
		var viewport = GetViewportRect();
		return new Vector2(viewport.Position.x + viewport.Size.x * fractionX,
						   viewport.Position.y + viewport.Size.y * fractionY);
	}

	private Node2D[] playerAreas;

	private Game game;
}
