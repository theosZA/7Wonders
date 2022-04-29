using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;

public class Leaderboard : WindowHolder
{
	public void ShowLeaderboard(IReadOnlyCollection<(Player player, int victoryPoints)> newLeaderboard)
	{
		leaderboard = newLeaderboard.ToList();

		for (int i = 0; i < leaderboard.Count; ++i)
		{
			var node = GetNode<Node2D>($"LeaderboardDialog/Position{i + 1}");
			var (player, victoryPoints) = leaderboard[i];

			node.GetNode<TextureRect>("CityIcon").Texture = Assets.LoadCityIcon(player.CityName);
			node.GetNode<Label>("PlayerName").Text = player.Name;
			node.GetNode<Label>("VPs").Text = victoryPoints.ToString();

			// Show the coins if the player has the same score as the previous or next player in the leaderboard.
			bool showCoins = (i > 0 && victoryPoints == leaderboard[i - 1].victoryPoints)
						  || (i < leaderboard.Count - 1 && victoryPoints == leaderboard[i + 1].victoryPoints);
			var coinsNode = node.GetNode<Node2D>("CoinsNode");
			coinsNode.Visible = showCoins;
			if (showCoins)
			{
				coinsNode.GetNode<Label>("CoinsValue").Text = player.Coins.ToString();
			}

			// Hide the position number if the player has the same score and coins as the previous player in the leaderboard.
			bool hidePosition = (i > 0 && victoryPoints == leaderboard[i - 1].victoryPoints
									   && player.Coins == leaderboard[i - 1].player.Coins);
			node.GetNode<CanvasItem>("Number").Visible = !hidePosition;
		}

		// Only show the leadboard rows for the right player count.
		for (int i = 0; i < 7; ++i)
		{
			GetNode<CanvasItem>($"LeaderboardDialog/Position{i + 1}").Visible = (i < leaderboard.Count);
		}

		ShowWindow();
	}

	protected override WindowDialog GetWindowDialog() => GetNode<WindowDialog>("LeaderboardDialog");
	protected override CanvasItem GetReshowButtonHolder() => GetNode<CanvasItem>("ReshowLeaderboardButtonHolder");
	protected override BaseButton GetReshowButton() => GetNode<BaseButton>("ReshowLeaderboardButtonHolder/ReshowLeaderboardButton");

	protected override float GetRequiredWidth() => 1127;
	protected override float GetRequiredHeight() => 108 + 134 * leaderboard.Count;

	protected override bool ShouldSuppressShow() => (leaderboard == null);

	private IList<(Player player, int victoryPoints)> leaderboard;
}
