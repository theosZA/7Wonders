using System.Globalization;
using _7Wonders;
using Godot;
using static Godot.Control;

public static class Assets
{
	public static TextureButton CreateCardButton(Card card)
	{
		return new TextureButton()
		{
			TextureNormal = LoadCardTexture(card),
            TextureDisabled = LoadCardTexture(card, enabled: false),
            Expand = true,
            StretchMode = TextureButton.StretchModeEnum.KeepAspect,
            SizeFlagsHorizontal = (int)SizeFlags.ExpandFill
		};
	}

	public static Sprite CreateCardSprite(Card card)
	{
		return new Sprite()
		{
			Texture = LoadCardTexture(card),
			Scale = new Vector2(cardScale, cardScale)
		};
	}

    public static Sprite CreateCardBackSprite(int age)
    {
		return new Sprite()
		{
			Texture = LoadCardBackTexture(age),
			Scale = new Vector2(cardScale, cardScale)
		};
    }

    private static Texture LoadCardTexture(Card card, bool enabled = true)
    {
        string filename = $"Age{card.Age}_{ToTitleCase(card.Colour.ToString())}_{ToTitleCase(card.Name)}";
        if (!enabled)
        {
            filename += "_Disabled";
        }
        return GD.Load<Texture>($"res://Art/{filename}.png");
    }

    private static Texture LoadCardBackTexture(int age)
    {
        return GD.Load<Texture>($"res://Art/Age{age}_Back.png");
    }

	private static string ToTitleCase(string text)
	{
		return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text).Replace(" ", string.Empty);
	}

    private const float cardScale = 0.8f;
}