using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;

public class HandArea : Node2D
{
	public delegate void ActionChosenEventHandler(IAction action);
	public ActionChosenEventHandler ActionChosen { get; set; }

	[Signal]
	public delegate void HandShown();
	[Signal]
	public delegate void HandClosed();

	public bool AwaitingChoice { get; private set; }

	public void OnNewHand(IList<Card> hand, IReadOnlyCollection<IAction> actions)
	{
		var dialog = GetNode<WindowDialog>("HandDialog");

		// Update title.
		int age = hand[0].Age;
		int turn = 8 - hand.Count;
		dialog.WindowTitle = $"Age {age} Turn {turn}";

		// Remove all existing cards.
		var oldHandCardAreas = dialog.GetChildren().Cast<Node>()
												   .Where(node => node is HandCardArea);
		foreach (var handCardArea in oldHandCardAreas)
		{
			dialog.RemoveChild(handCardArea);
		}

		// Add new cards.
		var handCardAreas = CreateHandCardAreas(hand, actions).ToList();
		foreach (var handCardArea in handCardAreas)
		{
			dialog.AddChild(handCardArea);
		}

		requiredWidth = handCardAreas.Sum(handCardArea => handCardArea.RectSize.x);
		requiredHeight = handCardAreas.Max(handCardArea => handCardArea.RectSize.y);
		startingLeft = (GetViewportRect().Size.x - requiredWidth) / 2;
		
		AwaitingChoice = true;
		ShowHand();
	}

	private void ShowHand()
	{
		if (AwaitingChoice)
		{
			GetNode<WindowDialog>("HandDialog").Popup_(new Rect2(new Vector2(startingLeft, 10), new Vector2(requiredWidth, requiredHeight)));
			EmitSignal("HandShown");
		}
	}

	private void OnHandDialogClosed()
	{
		EmitSignal("HandClosed");
	}

	private void OnActionChosen(IAction action)
	{
		AwaitingChoice = false;
		GetNode<WindowDialog>("HandDialog").Hide();
		ActionChosen?.Invoke(action);
	}

	private IEnumerable<HandCardArea> CreateHandCardAreas(IList<Card> hand, IReadOnlyCollection<IAction> actions)
	{
		float xOffset = 0;
		foreach (var card in hand)
		{
			var handCardArea = CreateHandCardArea(card, actions);
			handCardArea.RectPosition = new Vector2(xOffset, 0);
			xOffset += handCardArea.RectSize.x;
			yield return handCardArea;
		}
	}

	private HandCardArea CreateHandCardArea(Card card, IReadOnlyCollection<IAction> handActions)
	{
		var handCardArea = handCardAreaScene.Instance<HandCardArea>();
		handCardArea.ActionChosen = OnActionChosen;
		handCardArea.Card = card;
		handCardArea.HandActions = handActions;

		return handCardArea;
	}

	private PackedScene handCardAreaScene = ResourceLoader.Load<PackedScene>("res://Scenes/HandCardArea.tscn");

	float requiredWidth;
	float requiredHeight;
	float startingLeft;
}
