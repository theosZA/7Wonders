using Godot;
using System.Collections.Generic;

public class ChangeTradePopup : PopupDialog
{
	public delegate void TradeChosenEventHandler(int leftAmount, int rightAmount);
	public TradeChosenEventHandler TradeChosen { get; set; }

	public IReadOnlyCollection<(int leftAmount, int rightAmount)> TradeValues { get; set; }

	public override void _Ready()
	{
		var tradeButtonScene = ResourceLoader.Load<PackedScene>("res://Scenes/TradeButton.tscn");

		var currentNode = new Node2D()
		{
			Position = new Vector2(5, 5)
		};
		foreach (var tradeValue in TradeValues)
		{
			var tradeButton = tradeButtonScene.Instance<TradeButton>();
			tradeButton.LeftAmount = tradeValue.leftAmount;
			tradeButton.RightAmount = tradeValue.rightAmount;
			tradeButton.Connect("gui_input", this, "OnTradeButtonInput", new Godot.Collections.Array(tradeValue.leftAmount, tradeValue.rightAmount));
			currentNode.AddChild(tradeButton);
			AddChild(currentNode);

			currentNode = new Node2D()
			{
				Position = currentNode.Position + new Vector2(0, tradeButton.RectSize.y + 5)
			};
		}
	}

	public void OnTradeButtonInput(InputEvent inputEvent, int leftAmount, int rightAmount)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			TradeChosen?.Invoke(leftAmount, rightAmount);
			Hide();
		}
	}
}
