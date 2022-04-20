using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _7Wonders;
using Godot;

/// <summary>
/// Player agent that displays the player choices in a Godot UI and allows for a human player to make the choice.
/// </summary>
public class GodotHumanPlayer : PlayerAgent
{
    public string Name { get; }

    public GodotHumanPlayer(string name, Node2D godotParent)
    {
        Name = name;
        this.godotParent = godotParent;
    }

    public IAction GetAction(IList<PlayerState> playerStates, IList<Card> hand)
    {
        var actingPlayer = playerStates[0];
        var leftNeighbour = playerStates[1];
        var rightNeighbour = playerStates[playerStates.Count - 1];

        var actions = playerStates[0].GetAllActions(hand, leftNeighbour, rightNeighbour).ToList();

        var handArea = new HandArea(hand, actions, OnActionChosen);
        godotParent.AddChild(handArea);
        handArea.RectPosition = new Vector2((godotParent.GetViewportRect().Size.x - handArea.RectSize.x) / 2, 40);

        waitHandle.WaitOne();   // Blocks until OnActionChosen is called.

        handArea.QueueFree();

        return chosenAction;
    }

    public void OnActionChosen(IAction action)
    {
        this.chosenAction = action;
        waitHandle.Set();
    }

    private Node2D godotParent;
    private AutoResetEvent waitHandle = new AutoResetEvent(false);
    private IAction chosenAction;
}