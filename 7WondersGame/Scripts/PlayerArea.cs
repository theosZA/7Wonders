using System.Collections.Generic;
using Godot;
using _7Wonders;

public class PlayerArea : Node2D
{
	public Player Player { get; set; }
	public Player LeftNeighbour { get; set; }
	public Player RightNeighbour { get; set; }

	public override void _Ready()
	{
		if (Player != null)
		{
			var playerBoard = GetNode<TextureRect>("PlayerBoard");
			playerBoard.Texture = Assets.LoadCityBoard(Player.CityName, Player.BoardSide);
		}

		GetNode<CardStack>("PlayerBoard/Cards/ProductionRoot/CardStack").LatestToFront = false;
		GetNode<CardStack>("PlayerBoard/Cards/PurpleRoot/CardStack").GrowUpwards = false;

		OnGameUpdate();
	}

	public void HandleAction(IAction action)
	{
		switch (action)
		{
			case Build build:
				AddCard(build.Card);
				break;

			case BuildWonderStage buildWonderStage:
				AddWonderStage(buildWonderStage.CardToSpend.Age);
				break;
		}

		OnGameUpdate();
	}

	public void AddMilitaryResult(int age, bool victory)
	{
		const int maxColumns = 3;
		int row = militaryTokens.Count / maxColumns;
		int column = militaryTokens.Count % maxColumns;

		var texture = Assets.LoadMilitaryToken(age, victory);
		var militaryToken = new TextureRect()
		{
			Texture = texture,
			RectPosition = new Vector2(column * texture.GetWidth(), row * texture.GetHeight())
		};
		
		militaryTokens.Add(militaryToken);
		GetNode("PlayerBoard/MilitaryResults").AddChild(militaryToken);
	}

	private void AddCard(Card card)
	{
		cards.Add(card);

		GetCardStack(card).AddCard(card);
	}

	private void AddWonderStage(int age)
	{
		++wonderStagesBuilt;

		var positioningNode = FindNode($"WonderStage{wonderStagesBuilt}");
		if (positioningNode != null)	// TODO: Handle wonders that have 2 or 4 stages. For now we assume 3 stages.
		{
			positioningNode.AddChild(Assets.CreateCardBack(age));
		}
	}

	private CardStack GetCardStack(Card card)
	{
		switch (card.Colour)
		{
			case Colour.Brown:
			case Colour.Gray:
				return GetNode<CardStack>("PlayerBoard/Cards/ProductionRoot/CardStack");

			case Colour.Yellow:
				return GetNode<CardStack>("PlayerBoard/Cards/YellowRoot/CardStack");

			case Colour.Red:
				return GetNode<CardStack>("PlayerBoard/Cards/RedRoot/CardStack");

			case Colour.Green:
				return GetNode<CardStack>("PlayerBoard/Cards/GreenRoot/CardStack");

			case Colour.Blue:
				return GetNode<CardStack>("PlayerBoard/Cards/BlueRoot/CardStack");

			case Colour.Purple:
				return GetNode<CardStack>("PlayerBoard/Cards/PurpleRoot/CardStack");

			default:
				throw new System.Exception($"Unexpected card colour: {card.Colour}");
		}		
	}

	private static Node GetTerminalNode(Node root)
	{
		var currentNode = root;
		while (true)
		{
			var nextNode = currentNode.GetNodeOrNull("next");
			if (nextNode == null)
			{
				return currentNode;
			}
			currentNode = nextNode;
		}
	}

	public void OnGameUpdate()
	{
		if (Player != null && LeftNeighbour != null && RightNeighbour != null)
		{
			var vpsValue = (Label)FindNode("VPsValue");
			vpsValue.Text = Player.CalculateVictoryPoints(LeftNeighbour, RightNeighbour).ToString();
		}
		if (Player != null)
		{
			var coinsValue = (Label)FindNode("CoinsValue");
			coinsValue.Text = Player.Coins.ToString();

			var freeBuildAvailable = (CanvasItem)FindNode("FreeBuildAvailable");
			freeBuildAvailable.Visible = (Player.FreeBuildsLeft > 0);
		}
	}

	private List<Card> cards = new List<Card>();
	private List<TextureRect> militaryTokens = new List<TextureRect>();
	int wonderStagesBuilt = 0;
}
