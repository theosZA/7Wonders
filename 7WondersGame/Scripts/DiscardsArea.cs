using Godot;
using _7Wonders;
using System;
using System.Collections.Generic;
using System.Linq;

public class DiscardsArea : Node2D
{
	public delegate void CardChosenEventHandler(Card card);
	public CardChosenEventHandler CardChosen { get; set; }

	[Signal]
	public delegate void DiscardsShown();
	[Signal]
	public delegate void DiscardsClosed();

	public bool AwaitingChoice { get; private set; }

	public void OnDiscardsBuild(IReadOnlyCollection<Card> allDiscards, IReadOnlyCollection<Card> buildableCards)
	{
		var dialog = GetNode<WindowDialog>("DiscardsDialog");

		// Remove all existing cards.
		var oldCardButtons = dialog.GetChildren().Cast<Node>()
												 .Where(node => node is Node2D);
		foreach (var cardButton in oldCardButtons)
		{
			dialog.RemoveChild(cardButton);
		}

		// Add new cards.
		const int columns = 8;
		const int columnWidth = 228;
		const int rowHeight = 352;
		int count = 0;
		foreach (var card in allDiscards)
		{
			int column = count % columns;
			int row = count / columns;

			var cardNode = new Node2D()
			{
				Scale = new Vector2(0.6f, 0.6f),
				Position = new Vector2(column * columnWidth, row * rowHeight)
			};

			var cardButton = Assets.CreateCardButton(card);
			cardButton.Disabled = !buildableCards.Contains(card);
			cardButton.Connect("pressed", this, "OnCardSelected", new Godot.Collections.Array(new CardObject() { Card = card }));

			cardNode.AddChild(cardButton);
			dialog.AddChild(cardNode);

			++count;
		};

		int actualRows = (count - 1) / columns + 1;
		int actualColumns = Math.Min(columns, count);
		requiredHeight = actualRows * rowHeight;
		requiredWidth = actualColumns * columnWidth;
		startingLeft = (GetViewportRect().Size.x - requiredWidth) / 2;
		
		AwaitingChoice = true;
		ShowDiscards();
	}

	private void ShowDiscards()
	{
		if (AwaitingChoice)
		{
			GetNode<WindowDialog>("DiscardsDialog").Popup_(new Rect2(new Vector2(startingLeft, 10), new Vector2(requiredWidth, requiredHeight)));
			EmitSignal("DiscardsShown");
		}
	}

	private void OnDiscardsDialogClosed()
	{
		EmitSignal("DiscardsClosed");
	}

	private void OnCardSelected(CardObject card)
	{
		AwaitingChoice = false;
		GetNode<WindowDialog>("DiscardsDialog").Hide();
		CardChosen?.Invoke(card.Card);
	}

	private float requiredWidth;
	private float requiredHeight;
	private float startingLeft;
}
