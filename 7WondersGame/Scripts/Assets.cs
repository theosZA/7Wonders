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

    public static TextureButton CreateCardButton(Card card)
    {
        return new TextureButton()
        {
            TextureNormal = Assets.LoadCardTexture(card, enabled: true),
            TextureDisabled = Assets.LoadCardTexture(card, enabled: false),
        };
    }

    public static Texture LoadCardTexture(Card card, bool enabled = true)
    {
        string filename = $"Age{card.Age}_{ToTitleCase(card.Colour.ToString())}_{ToTitleCase(card.Name)}";
        if (!enabled)
        {
            filename += "_Disabled";
        }
        return GD.Load<Texture>($"res://Art/{filename}.png");
    }

    public static Texture LoadCardBackTexture(int age)
    {
        return GD.Load<Texture>($"res://Art/Age{age}_Back.png");
    }

    public static Texture LoadCityBoard(string cityName, BoardSide boardSide)
    {
        return GD.Load<Texture>($"res://Art/PlayerBoard_{cityName}_{boardSide}.jpg");
    }

    public static Texture LoadCityIcon(string cityName)
    {
        return GD.Load<Texture>($"res://Art/Icon_City_{cityName}.png");
    }

    public static Texture LoadMilitaryToken(int age, bool victory)
    {
        string filename = "res://Art/Token_Military";
        if (victory)
        {
            filename += $"Victory_Age{age}.png";
        }
        else
        {
            filename += "Defeat.png";
        }

        return GD.Load<Texture>(filename);
    }

	private static string ToTitleCase(string text)
	{
		return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text).Replace(" ", string.Empty);
	}
}