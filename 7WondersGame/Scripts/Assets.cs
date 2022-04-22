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
            SizeFlagsHorizontal = (int)SizeFlags.ExpandFill,
            SizeFlagsVertical = (int)SizeFlags.ExpandFill
		};
	}

	public static TextureRect CreateCardFront(Card card)
	{
		return new TextureRect()
		{
			Texture = LoadCardTexture(card)
		};
	}

    public static TextureRect CreateCardBack(int age)
    {
		return new TextureRect()
		{
			Texture = LoadCardBackTexture(age)
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
}