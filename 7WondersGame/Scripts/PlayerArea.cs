using System.Collections.Generic;
using Godot;
using _7Wonders;

public class PlayerArea : Node2D
{
	public Player Player;
	public Player LeftNeighbour;
	public Player RightNeighbour;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Player != null)
		{
			var playerBoard = GetNode<TextureRect>("PlayerBoard");
			playerBoard.Texture = GD.Load<Texture>($"res://Art/PlayerBoard_{Player.CityName}_A.jpg");
		}
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

	private void AddCard(Card card)
	{
		cards.Add(card);

		var terminalNode = (Node2D)GetTerminalNode(GetColumnRoot(card));
		terminalNode.AddChild(Assets.CreateCardFront(card));
		
		var newTerminalNode = new Node2D()
		{
			Name = "next",
			ZIndex = terminalNode.ZIndex - 1,	// next card goes behind this one...
			Position = new Vector2(0, -125)		// ...and a bit up
		};
		terminalNode.AddChild(newTerminalNode);
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

	private Node GetColumnRoot(Card card)
	{
		switch (card.Colour)
		{
			case Colour.Brown:
			case Colour.Gray:
				return FindNode("ProductionRoot");

			case Colour.Yellow:
				return FindNode("YellowRoot");

			case Colour.Red:
				return FindNode("RedRoot");

			case Colour.Green:
				return FindNode("GreenRoot");

			case Colour.Blue:
				return FindNode("BlueRoot");

			case Colour.Purple:
				return FindNode("PurpleRoot");

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
		}
	}

	private List<Card> cards = new List<Card>();
	int wonderStagesBuilt = 0;
}
