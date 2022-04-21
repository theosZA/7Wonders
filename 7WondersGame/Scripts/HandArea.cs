using Godot;
using _7Wonders;
using System.Collections.Generic;
using System;

public class HandArea : PanelContainer
{
	// Editor-specific constructor. Don't use.
	private HandArea()
	{}

	public HandArea(IList<Card> hand, IReadOnlyCollection<IAction> actions, Action<IAction> onActionChosen)
    {
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

        foreach (var card in hand)
        {
            var cardArea = new HandCardArea(card, actions, onActionChosen);
			perCardContainer.AddChild(cardArea);
        }
    }
}