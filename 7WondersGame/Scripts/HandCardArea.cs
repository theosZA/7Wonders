using Godot;
using _7Wonders;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

public class HandCardArea : ColorRect
{
	public delegate void ActionChosenEventHandler(IAction action);
	public ActionChosenEventHandler ActionChosen { get; set; }

	public Card Card { get; set; }

	public IReadOnlyCollection<IAction> HandActions { get; set; }   // actions not limited to just this card
	public IEnumerable<Build> BuildActions => HandActions.Where(action => action is Build buildAction && buildAction.Card.Name == Card.Name)
														 .Cast<Build>();
	public IEnumerable<BuildWonderStage> WonderActions => HandActions.Where(action => action is BuildWonderStage wonderAction && wonderAction.CardToSpend.Name == Card.Name)
																	 .Cast<BuildWonderStage>();
	public Sell SellAction => (Sell)HandActions.FirstOrDefault(action => action is Sell sellAction && sellAction.Card.Name == Card.Name);

	public override void _Ready()
	{
		InitializeCard(GetNode<Node2D>("CardNode"));
		InitializeFixedCost(GetNode<Node2D>("CostNode/FixedCostNode"));
		InitializeSell(GetNode<Node2D>("SellNode"));
		InitializeWonder(GetNode<Node2D>("WonderNode"));

		InitializeBuildTradeSelection();
		UpdateTrade(buildTradeSelection, GetNode<Node2D>("CostNode/TradeNode"));

		InitializeWonderTradeSelection();
		UpdateTrade(wonderTradeSelection, GetNode<Node2D>("WonderNode/TradeNode"));
	}

	private void InitializeCard(Node cardNode)
	{
		if (!BuildActions.Any())
		{
			cardNode.AddChild(Assets.CreateCardFront(Card, enabled: false));
			return;
		}

		var cardFront = Assets.CreateCardFront(Card);
		cardFront.Connect("gui_input", this, "OnBuildGuiInput");
		cardNode.AddChild(cardFront);
	}

	private void InitializeFixedCost(CanvasItem fixedCostNode)
	{
		fixedCostNode.Visible = BuildActions.Any() && Card.Cost.Coins > 0;
		fixedCostNode.GetNode<Label>("CoinValue").Text = Card.Cost.Coins.ToString();
	}

	private void InitializeSell(CanvasItem sellNode)
	{
		if (SellAction == null)
		{
			sellNode.Visible = false;
			return;
		}

		sellNode.GetNode("SellPanel").Connect("gui_input", this, "OnSellGuiInput");
	}

	private void InitializeWonder(CanvasItem wonderNode)
	{
		if (!WonderActions.Any())
		{
			wonderNode.Visible = false;
			return;
		}

		wonderNode.GetNode("WonderPanel").Connect("gui_input", this, "OnWonderGuiInput");
	}

	private void InitializeBuildTradeSelection()
	{
		var buildAction = BuildActions.FirstOrDefault();
		if (buildAction?.CoinsToLeftNeighbour > 0 || buildAction?.CoinsToRightNeighbour > 0)
		{
			buildTradeSelection = new TradeSelection<Build>(BuildActions);
		}
	}

	private void InitializeWonderTradeSelection()
	{
		var wonderAction = WonderActions.FirstOrDefault();
		if (wonderAction?.CoinsToLeftNeighbour > 0 || wonderAction?.CoinsToRightNeighbour > 0)
		{
			wonderTradeSelection = new TradeSelection<BuildWonderStage>(WonderActions);
		}
	}

	private static void UpdateTrade<ActionType>(TradeSelection<ActionType> tradeSelection, Node2D tradeNode) where ActionType : IAction
	{
		if (tradeSelection == null)
		{
			tradeNode.Visible = false;
			return;
		}

		tradeNode.Visible = true;

		UpdateDirectionalTrade(tradeNode.GetNode<Node2D>("TradeLeft"), tradeSelection.SelectedCoinsToLeftNeighbour);
		UpdateDirectionalTrade(tradeNode.GetNode<Node2D>("TradeRight"), tradeSelection.SelectedCoinsToRightNeighbour);
	}

	private static void UpdateDirectionalTrade(Node2D directionalTradeNode, int value)
	{
		directionalTradeNode.Visible = value > 0;
		directionalTradeNode.GetNode<Label>("CoinValue").Text = value.ToString();
	}

	private void OnBuildGuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			ActionChosen?.Invoke(buildTradeSelection?.SelectedTrade ?? BuildActions.First());
		}
	}

	private void OnWonderGuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			ActionChosen?.Invoke(wonderTradeSelection?.SelectedTrade ?? WonderActions.First());
		}
	}

	private void OnSellGuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			ActionChosen?.Invoke(SellAction);
		}
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

	private TradeSelection<Build> buildTradeSelection;
	private TradeSelection<BuildWonderStage> wonderTradeSelection;
}
