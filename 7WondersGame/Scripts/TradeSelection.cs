using System;
using System.Collections.Generic;
using System.Linq;
using _7Wonders;
using Godot;

public class TradeSelection<ActionType> : PanelContainer where ActionType : IAction
{
    public ActionType SelectedTrade => actions[selectedActionIndex];

	// Editor-specific constructor. Don't use.
	private TradeSelection()
	{}

    public TradeSelection(IEnumerable<ActionType> actions)
    {
        this.actions = actions.ToList();

        // By default select the cheapest trading option.
        selectedActionIndex = GetIndexOfCheapestTrade();        
    }

    private int GetIndexOfCheapestTrade()
    {
        var cheapestTradeIndex = 0;
        for (int i = 1; i < actions.Count; ++i)
        {
            if (IsTradeCheaperThan(actions[i], actions[cheapestTradeIndex]))
            {
                cheapestTradeIndex = i;
            }
        }

        return cheapestTradeIndex;
    }

    private static bool IsTradeCheaperThan(ActionType lhs, ActionType rhs)
    {
        int lhsCoinsToLeftNeighbour = CoinsToLeftNeighbour(lhs);
        int lhsCoinsToRightNeighbour = CoinsToRightNeighbour(lhs);
        int rhsCoinsToLeftNeighbour = CoinsToLeftNeighbour(rhs);
        int rhsCoinsToRightNeighbour = CoinsToRightNeighbour(rhs);

        int lhsTotal = lhsCoinsToLeftNeighbour + lhsCoinsToRightNeighbour;
        int rhsTotal = rhsCoinsToLeftNeighbour + rhsCoinsToRightNeighbour;
        if (lhsTotal < rhsTotal)
        {
            return true;
        }
        if (lhsTotal > rhsTotal)
        {
            return false;
        }

        // Break ties in favour of the trade option with the most equal distribution.
        int lhsDifference = Math.Abs(lhsCoinsToLeftNeighbour - lhsCoinsToRightNeighbour);
        int rhsDifference = Math.Abs(rhsCoinsToLeftNeighbour - rhsCoinsToRightNeighbour);
        return lhsDifference < rhsDifference;
    }

    private static int CoinsToLeftNeighbour(ActionType action)
    {
        switch (action)
        {
            case Build buildAction:
                return buildAction.CoinsToLeftNeighbour;

            case BuildWonderStage wonderAction:
                return wonderAction.CoinsToLeftNeighbour;

            default:
                return 0;
        }
    }

    private static int CoinsToRightNeighbour(ActionType action)
    {
        switch (action)
        {
            case Build buildAction:
                return buildAction.CoinsToRightNeighbour;

            case BuildWonderStage wonderAction:
                return wonderAction.CoinsToRightNeighbour;

            default:
                return 0;
        }
    }

    private List<ActionType> actions;
    private int selectedActionIndex;
}
