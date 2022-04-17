using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;
using _7Wonders;

public class PlayerArea
{
    public PlayerArea(Node parent, Rect2 viewport, Player player, Player leftNeighbour, Player rightNeighbour)
    {
        this.player = player;
        this.viewport = viewport;

        rootNode = new Node2D();
		rootNode.AddChild(CreateBoard(viewport.Size.x));

        playerInfo = new PlayerInfo(player, leftNeighbour, rightNeighbour);
        rootNode.AddChild(playerInfo);

        parent.AddChild(rootNode);
    }

    public void ApplyScale(float relativeScale)
    {
        rootNode.ApplyScale(new Vector2(relativeScale, relativeScale));
    }

    public void SetPosition(float fractionX, float fractionY)
    {
        rootNode.Position = new Vector2(viewport.Position.x + viewport.Size.x * fractionX,
                                        viewport.Position.y + viewport.Size.y * fractionY);
    }

    public void HandleAction(IAction action)
    {
        switch (action)
        {
            case Build build:
                AddCard(build.Card);
                break;

            case BuildWonderStage buildWonderStage:
                AddWonderStage(buildWonderStage.CardToSpend.Age);
                break;
        }

        playerInfo.OnGameUpdate();
    }

    private void AddCard(Card card)
    {
        var cardSprite = CreateCardSprite(card);
        rootNode.AddChild(cardSprite);
        var cardPosition = CalculateCardPosition(card.Colour);
        cardSprite.Position = cardPosition.position;
        cardSprite.ZIndex = cardPosition.zIndex;

        cards.Add(card);
    }

    private void AddWonderStage(int age)
    {
        var cardBackSprite = CreateCardBackSprite(age);
        rootNode.AddChild(cardBackSprite);
        
        // TODO: Handle wonders that have 2 or 4 stages. For now we assume 3 stages.
        cardBackSprite.Position = CalculatePosition(0.2f + 0.3f * wonderStagesBuilt, 0.9f);
        cardBackSprite.ZIndex = -1;

        ++wonderStagesBuilt;        
    }

	private Sprite CreateBoard(float viewportWidth)
	{
		var board = new Sprite()
		{
			Texture = GD.Load<Texture>($"res://Art/PlayerBoard_{player.CityName}_A.jpg")
		};

        boardWidth = board.Texture.GetWidth();
        boardHeight = board.Texture.GetHeight();

		float playAreaScale = viewportWidth / boardWidth;
		float boardScale = 0.80645f * playAreaScale;	// The board should take up about 80% of the horizontal width of the player area.
		board.ApplyScale(new Vector2(boardScale, boardScale));

        boardWidth *= boardScale;
        boardHeight *= boardScale;

		return board;
	}    

	private static Sprite CreateCardSprite(Card card)
	{
		return new Sprite()
		{
			Texture = GD.Load<Texture>($"res://Art/Age{card.Age}_{ToTitleCase(card.Colour.ToString())}_{ToTitleCase(card.Name)}.png"),
			Scale = new Vector2(cardScale, cardScale)
		};
	}

    private static Sprite CreateCardBackSprite(int age)
    {
		return new Sprite()
		{
			Texture = GD.Load<Texture>($"res://Art/Age{age}_Back.png"),
			Scale = new Vector2(cardScale, cardScale)
		};
    }

    private (Vector2 position, int zIndex) CalculateCardPosition(Colour colour)
    {
        int cardsOfTheSameType = CountCardsOfTheSameType(colour);
        int column = GetColumnForCard(colour);
        
        float fractionX = 0.105f + 0.2f * column;
        float fractionY = 0.16f - 0.175f * cardsOfTheSameType;
        if (column > 0) // The above calculation is to line up the resource cards neatly. We want to offset the other card types a bit.
        {
            fractionX += 0.04f;
            fractionY -= 0.15f;
        }

        Vector2 position = CalculatePosition(fractionX, fractionY);
        int zIndex = -1 - cardsOfTheSameType;
        return (position, zIndex);
        
    }

    private Vector2 CalculatePosition(float fractionX, float fractionY)
    {
        float leftEdge = -boardWidth / 2;
        float topEdge = -boardHeight / 2;

        // A fraction of 0 is the left edge (/top edge) and a fraction of 1 is the right edge (/bottom edge).
        return new Vector2(leftEdge + boardWidth * fractionX, topEdge + boardHeight * fractionY);
    }

    private int CountCardsOfTheSameType(Colour colour)
    {
        if (colour == Colour.Brown || colour == Colour.Gray)
        {
            return cards.Count(card => card.Colour == Colour.Brown || card.Colour == Colour.Gray);
        }
        return cards.Count(card => card.Colour == colour);
    }

    private static int GetColumnForCard(Colour colour)
    {
        switch (colour)
        {
            case Colour.Brown:
            case Colour.Gray:
                return 0;

            case Colour.Yellow:
                return 1;

            case Colour.Red:
                return 2;

            case Colour.Green:
                return 3;

            case Colour.Blue:
                return 4;

            case Colour.Purple:
                return 5;

            default:
                throw new System.Exception($"Unexpected card colour: {colour}");
        }
    }

	private static string ToTitleCase(string text)
	{
		return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text).Replace(" ", string.Empty);
	}

    private const float cardScale = 0.8f;
		
    private Player player;
    private Rect2 viewport;
    private Node2D rootNode;
    private PlayerInfo playerInfo;
    private List<Card> cards = new List<Card>();
    float boardWidth;
    float boardHeight;
    int wonderStagesBuilt = 0;
}
 