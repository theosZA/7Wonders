using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;

public class HandArea : WindowHolder
{
	public delegate void ActionChosenEventHandler(IAction action);
	public ActionChosenEventHandler ActionChosen { get; set; }

	public bool AwaitingChoice => handCardAreas != null;

	public void ShowNewHand(IList<Card> hand, IReadOnlyCollection<IAction> actions)
	{
		// Update title.
		int age = hand[0].Age;
		int turn = 8 - hand.Count;
		GetWindowDialog().WindowTitle = $"Age {age} Turn {turn}";

		// Add new cards.
		handCardAreas = CreateHandCardAreas(hand, actions).ToList();
		foreach (var handCardArea in handCardAreas)
		{
			GetWindowDialog().AddChild(handCardArea);
		}

		ShowWindow();
	}

	protected override WindowDialog GetWindowDialog() => GetNode<WindowDialog>("HandDialog");
	protected override CanvasItem GetReshowButtonHolder() => GetNode<CanvasItem>("ReshowHandButtonHolder");
	protected override BaseButton GetReshowButton() => GetNode<BaseButton>("ReshowHandButtonHolder/ReshowHandButton");

	protected override float GetRequiredWidth() => handCardAreas?.Sum(handCardArea => handCardArea.RectSize.x) ?? 0;
	protected override float GetRequiredHeight() => handCardAreas?.Max(handCardArea => handCardArea.RectSize.y) ?? 0;

	protected override bool ShouldSuppressShow() => !AwaitingChoice;

	private void OnActionChosen(IAction action)
	{
		// Remove all existing cards.
		foreach (var handCardArea in handCardAreas)
		{
			GetWindowDialog().RemoveChild(handCardArea);
		}
		handCardAreas = null;

		HideWindow();
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

	private IReadOnlyCollection<HandCardArea> handCardAreas;
}
