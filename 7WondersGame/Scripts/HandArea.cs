using Godot;
using _7Wonders;
using System.Collections.Generic;
using System.Linq;
using System;

public class HandArea : PanelContainer
{
	// Editor-specific constructor. Don't use.
	private HandArea()
	{}

	public HandArea(IList<Card> hand, IReadOnlyCollection<IAction> actions, Action<IAction> onActionChosen)
    {
        this.hand = hand;
        this.actions = actions;
        this.onActionChosen = onActionChosen;

        RectSize = new Vector2(1200, 400);
		var panelStyle = new StyleBoxFlat()
		{
			BgColor = new Color(0.1f, 0.1f, 0.5f, 0.75f),
			BorderColor = new Color(0.1f, 0.1f, 0.5f, 0.75f),
			BorderWidthLeft = 15,
			BorderWidthTop = 15,
			BorderWidthRight = 15,
			BorderWidthBottom = 15,
			CornerRadiusTopLeft = 7,
			CornerRadiusTopRight = 7,
			CornerRadiusBottomRight = 7,
			CornerRadiusBottomLeft = 7
		};
		AddStyleboxOverride("panel", panelStyle);

        var perCardContainer = new HBoxContainer();
        perCardContainer.AddConstantOverride("separation", 5);
        AddChild(perCardContainer);

        for (int i = 0; i < hand.Count; ++i)
        {
            var controlsContainer = new VBoxContainer()
            {
                SizeFlagsHorizontal = (int)SizeFlags.ExpandFill
            };
            perCardContainer.AddChild(controlsContainer);

            var card = hand[i];
            var buildButton = Assets.CreateCardButton(card);
            buildButton.Connect("pressed", this, "OnBuild", new Godot.Collections.Array(i));
            buildButton.Disabled = !actions.Any(action => action is Build build && build.Card.Name == card.Name);
            controlsContainer.AddChild(buildButton);

            var sellButton = new Button()
            {
                Text = "+3"
            };
            sellButton.Connect("pressed", this, "OnSell", new Godot.Collections.Array(i));
            controlsContainer.AddChild(sellButton);

            var wonderButton = new Button()
            {
                Text = "Wonder"
            };
            wonderButton.Connect("pressed", this, "OnWonder", new Godot.Collections.Array(i));
            wonderButton.Disabled = !actions.Any(action => action is BuildWonderStage buildWonderStage && buildWonderStage.CardToSpend.Name == card.Name);
            controlsContainer.AddChild(wonderButton);
        }
    }

    public void OnBuild(int index)
    {
        var card = hand[index];
        // TODO: Choose between multiple build options for same card but different trades.
        var chosenAction = actions.First(action => action is Build build && build.Card.Name == card.Name);
        onActionChosen(chosenAction);
    }

    public void OnSell(int index)
    {
        var card = hand[index];
        var chosenAction = actions.First(action => action is Sell sell && sell.Card.Name == card.Name);
        onActionChosen(chosenAction);
    }

    public void OnWonder(int index)
    {
        var card = hand[index];
        // TODO: Choose between multiple trade options for building the wonder stage.
        var chosenAction = actions.First(action => action is BuildWonderStage buildWonderStage && buildWonderStage.CardToSpend.Name == card.Name);
        onActionChosen(chosenAction);
    }

    private IList<Card> hand;
    private IReadOnlyCollection<IAction> actions;
    private Action<IAction> onActionChosen;
}