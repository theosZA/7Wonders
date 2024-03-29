using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;

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
		InitializeFreeBuild(GetNode<Node2D>("FreeNode"));

		var buildTradeNode = GetNode<Node2D>("CostNode/TradeNode");
		buildTradeSelection = CreateTradeSelection(BuildActions.Where(build => !build.UsesFreeBuild), buildTradeNode);
		UpdateTrade(buildTradeSelection, buildTradeNode);
		InitializeChangeTradeButton(buildTradeSelection, buildTradeNode);

		var wonderTradeNode = GetNode<Node2D>("WonderNode/TradeNode");
		wonderTradeSelection = CreateTradeSelection(WonderActions, wonderTradeNode);
		UpdateTrade(wonderTradeSelection, wonderTradeNode);
		InitializeChangeTradeButton(wonderTradeSelection, wonderTradeNode);
	}

	private void InitializeCard(Node cardNode)
	{
		// If there are no possible builds for this card, or the only possible build uses up a free build,
		// you can't build this directly by clicking it and it will show as disabled.
		bool enabled = BuildActions.Any(build => !build.UsesFreeBuild);

		cardTexture = Assets.CreateCardFront(Card, enabled);
		if (enabled)
		{	
			cardTexture.Connect("gui_input", this, "OnBuildGuiInput");
		}
		cardNode.AddChild(cardTexture);
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

	private void InitializeFreeBuild(CanvasItem freeNode)
	{
		if (!BuildActions.Any(build => build.UsesFreeBuild))
		{
			freeNode.Visible = false;
			return;
		}

		freeNode.Visible = true;
		var freeButton = freeNode.GetNode("FreePanel");
		freeButton.Connect("gui_input", this, "OnFreeGuiInput");

		if (!BuildActions.Any(build => !build.UsesFreeBuild))
		{	// The displayed card will show as disabled. We need to show it enabled only when hovering over the button.
			freeButton.Connect("mouse_entered", this, "OnFreeMouseIn");
			freeButton.Connect("mouse_exited", this, "OnFreeMouseOut");

		}
	}

	private TradeSelection<ActionType> CreateTradeSelection<ActionType>(IEnumerable<ActionType> actions, Node2D tradeNode) where ActionType : IAction
	{
		var action = actions.FirstOrDefault();
		if (action == null)
		{
			return null;
		}

		var tradeSelection = new TradeSelection<ActionType>(actions);
		if (tradeSelection.OptionCount == 0)
		{
			return null;
		}

		tradeSelection.NewTradeSelected = () => UpdateTrade(tradeSelection, tradeNode);

		return tradeSelection;
	}

	private void UpdateTrade<ActionType>(TradeSelection<ActionType> tradeSelection, Node2D tradeNode) where ActionType : IAction
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

	private void InitializeChangeTradeButton<ActionType>(TradeSelection<ActionType> tradeSelection, Node2D tradeNode) where ActionType : IAction
	{
		var changeTradeButton = tradeNode.GetNode<TextureRect>("ChangeTrade");
		if (tradeSelection?.OptionCount >= 2)
		{
			var tradePopupDialog = tradeSelection.CreatePopupDialog();
			changeTradeButton.AddChild(tradePopupDialog);
			changeTradeButton.Connect("gui_input", this, "OnChangeTradeGuiInput", new Godot.Collections.Array(changeTradeButton, tradePopupDialog));
			changeTradeButton.Connect("mouse_entered", this, "OnChangeTradeMouseIn", new Godot.Collections.Array(changeTradeButton));
			changeTradeButton.Connect("mouse_exited", this, "OnChangeTradeMouseOut", new Godot.Collections.Array(changeTradeButton));
		}
		else
		{
			changeTradeButton.Visible = false;
		}
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

	private void OnFreeGuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			ActionChosen?.Invoke(BuildActions.First(build => build.UsesFreeBuild));
		}
	}

	private void OnChangeTradeGuiInput(InputEvent inputEvent, TextureRect changeTradeButton, PopupDialog changeTradeDialog)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			var position = changeTradeButton.RectGlobalPosition + new Vector2(0, 20);
			changeTradeDialog.Popup_(new Rect2(position, changeTradeDialog.RectSize));
		}
	}

	private void OnChangeTradeMouseIn(TextureRect changeTradeButton)
	{
		changeTradeButton.Texture = ResourceLoader.Load<Texture>("res://Art/Icon_Popup_Hover.png");
	}

	private void OnChangeTradeMouseOut(TextureRect changeTradeButton)
	{
		changeTradeButton.Texture = ResourceLoader.Load<Texture>("res://Art/Icon_Popup.png");
	}

	private void OnFreeMouseIn()
	{
		cardTexture.Texture = Assets.LoadCardTexture(Card, enabled: true);
	}

	private void OnFreeMouseOut()
	{
		cardTexture.Texture = Assets.LoadCardTexture(Card, enabled: false);
	}

	private TradeSelection<Build> buildTradeSelection;
	private TradeSelection<BuildWonderStage> wonderTradeSelection;

	private TextureRect cardTexture;
}
