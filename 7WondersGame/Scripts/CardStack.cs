using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;

public class CardStack : Node2D
{
	// If true, the stack will grow towards the top of the screen.
	// If false, the stack will grow towards the bottom of the screen.
	public bool GrowUpwards { get; set; } = true;
	// If true, the last-added card will appear at the front of the stack, with earlier cards pushed backwards.
	// If false, the first-added card will appear at the front of the stack, with later cards added to the back.
	public bool LatestToFront { get; set; } = true;

	public void AddCard(Card newCard)
	{
		Node2D newNode = CreateNewNode();
		cardNodes.Add(newNode);
		AddChild(newNode);

		newNode.AddChild(Assets.CreateCardFront(newCard));
	}

	private Node2D CreateNewNode()
	{
		var adjustment = new Vector2(0, GrowUpwards ? -125 : +125);

		if (MustShiftCards)
		{	// Move all existing nodes up or down.
			foreach (var cardNode in cardNodes)
			{
				cardNode.Position += adjustment;
			}
		}

		if (LatestToFront)
		{	// Move all existing cards back one.
			foreach (var cardNode in cardNodes)
			{
				--cardNode.ZIndex;
			}
		}

		return new Node2D()
		{
			Position = (MustShiftCards ? 0 : cardNodes.Count) * adjustment,
			ZIndex = LatestToFront ? this.ZIndex : (cardNodes.LastOrDefault()?.ZIndex ?? this.ZIndex) - 1
		};
	}

	private bool MustShiftCards => (GrowUpwards && LatestToFront)
								|| (!GrowUpwards && !LatestToFront);

	private IList<Node2D> cardNodes = new List<Node2D>();
}
