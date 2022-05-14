using Godot;

public class TradeButton : Panel
{
	public int LeftAmount { get; set; }
	public int RightAmount { get; set; }

	public override void _Ready()
	{
		if (LeftAmount == 0)
		{
			GetNode<CanvasItem>("TradeLeft").Visible = false;
		}
		else
		{
			GetNode<Label>("TradeLeft/CoinValue").Text = LeftAmount.ToString();
		}

		if (RightAmount == 0)
		{
			GetNode<CanvasItem>("TradeRight").Visible = false;
		}
		else
		{
			GetNode<Label>("TradeRight/CoinValue").Text = RightAmount.ToString();
		}
	}
}
