using Godot;
using _7Wonders;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

public class HandCardArea : VBoxContainer
{
	// Editor-specific constructor. Don't use.
	private HandCardArea()
	{}

	public HandCardArea(Card card, IReadOnlyCollection<IAction> actions, Action<IAction> onActionChosen)
    {
        SizeFlagsHorizontal = (int)SizeFlags.ExpandFill;        

        this.card = card;
        this.onActionChosen = onActionChosen;

        buildActions = actions.Where(action => action is Build buildAction && buildAction.Card.Name == card.Name)
                              .Cast<Build>()
                              .ToList();
        wonderActions = actions.Where(action => action is BuildWonderStage wonderAction && wonderAction.CardToSpend.Name == card.Name)
                              .Cast<BuildWonderStage>()
                              .ToList();
        sellAction = (Sell)actions.First(action => action is Sell sellAction && sellAction.Card.Name == card.Name);

        var buildButton = Assets.CreateCardButton(card);
        buildButton.Connect("pressed", this, "OnBuild");
        buildButton.Disabled = !buildActions.Any();
        AddChild(buildButton);

        if (buildActions.Any())
        {
            var buildAction = buildActions[0];
            if (buildAction.Card.Cost.Coins > 0)
            {
                var costButton = new Button()
                {
                    Text = $"{buildAction.Card.Cost.Coins}",
                    Disabled = true
                };
                AddChild(costButton);
            }
            else if (buildAction.CoinsToLeftNeighbour > 0 || buildAction.CoinsToRightNeighbour > 0)
            {
                buildTradeSelection = new TradeSelection<Build>(buildActions);
                var costButton = new Button()
                {
                    Text = GetCostText(buildTradeSelection.SelectedTrade.CoinsToLeftNeighbour, buildTradeSelection.SelectedTrade.CoinsToRightNeighbour, buildActions.Count > 1),
                    Disabled = (buildActions.Count == 1)
                };
                AddChild(costButton);
            }
        }

        var sellButton = new Button()
        {
            Text = "+3"
        };
        sellButton.Connect("pressed", this, "OnSell");
        AddChild(sellButton);

        var wonderButton = new Button()
        {
            Text = "Wonder"
        };
        wonderButton.Connect("pressed", this, "OnWonder");
        wonderButton.Disabled = !wonderActions.Any();
        AddChild(wonderButton);

        if (wonderActions.Any())
        {
            var wonderAction = wonderActions[0];
            if (wonderAction.CoinsToLeftNeighbour > 0 || wonderAction.CoinsToRightNeighbour > 0)
            {
                wonderTradeSelection = new TradeSelection<BuildWonderStage>(wonderActions);
                var wonderCostButton = new Button()
                {
                    Text = GetCostText(wonderTradeSelection.SelectedTrade.CoinsToLeftNeighbour, wonderTradeSelection.SelectedTrade.CoinsToRightNeighbour, wonderActions.Count > 1),
                    Disabled = (wonderActions.Count == 1)
                };
                AddChild(wonderCostButton);
            }
        }        
    }

    public void OnSell()
    {
        onActionChosen(sellAction);
    }

    public void OnBuild()
    {
        onActionChosen(buildActions.First());
    }

    public void OnWonder()
    {
        onActionChosen(wonderActions.First());
    }

    private static string GetCostText(int coinsToLeftNeighbour, int coinsToRightNeighbour, bool moreOptionsAvailable)
    {
        StringBuilder text = new StringBuilder();
        if (coinsToLeftNeighbour > 0)
        {
            text.Append("< ");
            text.Append(coinsToLeftNeighbour);
            text.Append(" ");
        }
        if (moreOptionsAvailable)
        {
            text.Append("v");
        }
        if (coinsToRightNeighbour > 0)
        {
            text.Append(" ");
            text.Append(coinsToRightNeighbour);
            text.Append(" >");
        }
        return text.ToString();
    }
    private Card card;

    private List<Build> buildActions;
    private List<BuildWonderStage> wonderActions;
    Sell sellAction;

    private TradeSelection<Build> buildTradeSelection;
    private TradeSelection<BuildWonderStage> wonderTradeSelection;

    private Action<IAction> onActionChosen;

}
