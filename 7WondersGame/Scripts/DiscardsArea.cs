using Godot;
using _7Wonders;
using System;
using System.Collections.Generic;
using System.Linq;

public class DiscardsArea : Node2D
{
	public delegate void CardChosenEventHandler(Card card);
	public CardChosenEventHandler CardChosen { get; set; }

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
		float height = actualRows * rowHeight;
		float width = actualColumns * columnWidth;
		float left = (GetViewportRect().Size.x - width) / 2;
		dialog.Popup_(new Rect2(new Vector2(left, 10), new Vector2(width, height)));
	}

	public void OnCardSelected(CardObject card)
	{
		GetNode<WindowDialog>("DiscardsDialog").Hide();
		CardChosen?.Invoke(card.Card);
	}
}
