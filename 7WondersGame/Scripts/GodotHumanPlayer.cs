using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _7Wonders;

/// <summary>
/// Player agent that displays the player choices in a Godot UI and allows for a human player to make the choice.
/// </summary>
public class GodotHumanPlayer : PlayerAgent
{
	public delegate void NewHandEventHandler(IList<Card> hand, IReadOnlyCollection<IAction> actions);
	public NewHandEventHandler NewHand { get; set; }

	public delegate void DiscardsBuildEventHandler(IReadOnlyCollection<Card> allDiscards, IReadOnlyCollection<Card> buildableCards);
	public DiscardsBuildEventHandler DiscardsBuild {get; set; }

	public string Name { get; }

	public GodotHumanPlayer(string name)
	{
		Name = name;
	}

	public IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand)
	{
		var actingPlayer = playerStates[0];
		var leftNeighbour = playerStates[1];
		var rightNeighbour = playerStates[playerStates.Count - 1];

		var actions = actingPlayer.GetAllActions(hand, leftNeighbour, rightNeighbour).ToList();

		NewHand?.Invoke(hand, actions);

		actionWaitHandle.WaitOne();   // Blocks until OnActionChosen is called.

		return chosenAction;
	}

	public Card GetBuildFromDiscards(IList<PlayerState> playerStates, IList<Card> discards)
	{
		var buildableCards = playerStates[0].GetAllBuildableCards(discards).ToList();
		if (!buildableCards.Any())
		{
			return null;
		}

		DiscardsBuild?.Invoke(discards.ToList(), buildableCards);

		discardBuildWaitHandle.WaitOne();   // Blocks until OnDiscardBuildChosen is called.

		return chosenDiscardBuild;
	}

	public void OnActionChosen(IAction action)
	{
		this.chosenAction = action;
		actionWaitHandle.Set();
	}

	public void OnDiscardBuildChosen(Card card)
	{
		this.chosenDiscardBuild = card;
		discardBuildWaitHandle.Set();
	}

	private AutoResetEvent actionWaitHandle = new AutoResetEvent(false);
	private IAction chosenAction;

	private AutoResetEvent discardBuildWaitHandle = new AutoResetEvent(false);
	private Card chosenDiscardBuild;
}
