using Godot;
using _7Wonders;
using System;
using System.Collections.Generic;
using System.Linq;

public class DiscardsArea : WindowHolder
{
	public delegate void CardChosenEventHandler(Card card);
	public CardChosenEventHandler CardChosen { get; set; }

	public bool AwaitingChoice => cardButtons != null;

	public void OnDiscardsBuild(IReadOnlyCollection<Card> allDiscards, IReadOnlyCollection<Card> buildableCards)
	{
		var dialog = GetNode<WindowDialog>("DiscardsDialog");

		// Add new cards.
		cardButtons = new List<Node2D>();
		foreach (var card in allDiscards)
		{
			int column = cardButtons.Count % maxColumns;
			int row = cardButtons.Count / maxColumns;

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

			cardButtons.Add(cardNode);
		};

		ShowWindow();
	}

	protected override WindowDialog GetWindowDialog() => GetNode<WindowDialog>("DiscardsDialog");
	protected override CanvasItem GetReshowButtonHolder() => GetNode<CanvasItem>("ReshowDiscardsButtonHolder");
	protected override BaseButton GetReshowButton() => GetNode<BaseButton>("ReshowDiscardsButtonHolder/ReshowDiscardsButton");

	protected override float GetRequiredWidth() => Math.Min(cardButtons.Count, maxColumns) * columnWidth;
	protected override float GetRequiredHeight() => ((cardButtons.Count - 1) / maxColumns + 1) * rowHeight;

	protected override bool ShouldSuppressShow() => !AwaitingChoice;

	private void OnCardSelected(CardObject card)
	{
		// Remove all existing cards.
		foreach (var cardButton in cardButtons)
		{
			GetWindowDialog().RemoveChild(cardButton);
		}
		cardButtons = null;

		HideWindow();
		CardChosen?.Invoke(card.Card);
	}
	
	private const int maxColumns = 8;
	private const int columnWidth = 228;
	private const int rowHeight = 352;

	private IList<Node2D> cardButtons;
}
