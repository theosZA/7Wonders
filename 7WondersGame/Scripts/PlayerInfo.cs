using Godot;
using _7Wonders;

public class PlayerInfo : PanelContainer
{
	// Editor-specific constructor. Don't use.
	private PlayerInfo()
	{}

	public PlayerInfo(Player player, Player leftNeighbour, Player rightNeighbour)
	{
		this.player = player;
		this.leftNeighbour = leftNeighbour;
		this.rightNeighbour = rightNeighbour;

		RectPosition = new Vector2(-600, -150);
		RectSize = new Vector2(600, 200);

		var panelStyle = new StyleBoxFlat()
		{
			BgColor = new Color(0, 0, 0, 0.5f),
			BorderColor = new Color(0, 0, 0, 0.7f),
			BorderWidthLeft = 5,
			BorderWidthTop = 5,
			BorderWidthRight = 5,
			BorderWidthBottom = 5,
			CornerRadiusTopLeft = 3,
			CornerRadiusTopRight = 3,
			CornerRadiusBottomRight = 3,
			CornerRadiusBottomLeft = 3
		};
		AddStyleboxOverride("panel", panelStyle);

		var centerContainer = new CenterContainer();
		AddChild(centerContainer);

		var font = new DynamicFont();
        font.FontData = ResourceLoader.Load<DynamicFontData>("res://Fonts/Mont-HeavyDEMO.otf");
        font.Size = 72;

		victoryPointsLabel = new Label();
		victoryPointsLabel.AddColorOverride("font_color", new Color(1.0f, 1.0f, 1.0f));
		victoryPointsLabel.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
		victoryPointsLabel.AddFontOverride("font", font);
		coinsLabel = new Label();
		coinsLabel.AddColorOverride("font_color", new Color(1.0f, 1.0f, 1.0f));
		coinsLabel.SizeFlagsVertical = (int)SizeFlags.ExpandFill;
		coinsLabel.AddFontOverride("font", font);

		var container = new VSplitContainer();
		container.AddChild(victoryPointsLabel);
		container.AddChild(coinsLabel);

		centerContainer.AddChild(container);

		OnGameUpdate();
	}

	public void OnGameUpdate()
	{
		victoryPointsLabel.Text = $"VP: {player.CalculateVictoryPoints(leftNeighbour, rightNeighbour)}";
		// victoryPointsLabel.Text += $" (RED: {player.MilitaryVictoryPoints}, COIN: {player.TreasuryVictoryPoints}, WOND: {player.WonderVictoryPoints}, "
		// 					     + $"BLUE: {player.CalculateCivilianVictoryPoints(leftNeighbour, rightNeighbour)}, YLW: {player.CalculateCommercialVictoryPoints(leftNeighbour, rightNeighbour)}, "
		// 						 + $"PURP: {player.CalculateGuildVictoryPoints(leftNeighbour, rightNeighbour)}, GRN: {player.ScienceVictoryPoints})";
		coinsLabel.Text = $"Coins: {player.Coins}";
	}
	
	private Player player;
	private Player leftNeighbour;
	private Player rightNeighbour;
	private Label victoryPointsLabel;
	private Label coinsLabel;
}
