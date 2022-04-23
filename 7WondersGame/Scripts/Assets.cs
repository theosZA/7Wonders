using System.Globalization;
using _7Wonders;
using Godot;

public static class Assets
{
	public static TextureRect CreateCardFront(Card card, bool enabled = true)
	{
		return new TextureRect()
		{
			Texture = LoadCardTexture(card, enabled)
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