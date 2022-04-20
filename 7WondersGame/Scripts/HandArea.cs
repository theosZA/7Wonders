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

        var container = new HBoxContainer();
        container.AddConstantOverride("separation", 5);
        AddChild(container);

        for (int i = 0; i < hand.Count; ++i)
        {
            var card = hand[i];
            var button = Assets.CreateCardButton(card);
            button.Connect("pressed", this, "OnCardChosen", new Godot.Collections.Array(i));
            container.AddChild(button);
            button.Disabled = !actions.Any(action => action is Build build && build.Card.Name == card.Name);
        }
    }

    public void OnCardChosen(int index)
    {
        var card = hand[index];
        // TODO: Choose between multiple build options for same card but different trades.
        var chosenAction = actions.First(action => action is Build build && build.Card.Name == card.Name);
        onActionChosen(chosenAction);
    }

    private IList<Card> hand;
    private IReadOnlyCollection<IAction> actions;
    private Action<IAction> onActionChosen;
}