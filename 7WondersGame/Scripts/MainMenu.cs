using Godot;
using _7Wonders;

public class MainMenu : Node2D
{
	public int PlayerCount
	{
		get => (int)((Range)FindNode("PlayerCountValue")).Value;

		set
		{
			((Range)FindNode("PlayerCountValue")).Value = value;
		}
	}

	public string WonderChoice
	{
		get => wonderChoice;

		set
		{
			wonderChoice = value;
			((TextureRect)FindNode("WonderChoiceImage")).Texture = Assets.LoadCityIcon(wonderChoice);
		}
	}

	public BoardSide BoardSide
	{
		get => ((BaseButton)FindNode("BoardSideA")).Pressed ? BoardSide.A : BoardSide.B;

		set
		{
			((BaseButton)FindNode("BoardSideA")).Pressed = (value == BoardSide.A);
			((BaseButton)FindNode("BoardSideB")).Pressed = (value == BoardSide.B);
		}
	}

	public override void _Ready()
	{
		ReadSettings();

		((WonderSelection)FindNode("WonderSelection")).NewWonderSelected = OnWonderSelected;
	}

	private void OnStartButtonPressed()
	{
		WriteSettings();
		GetTree().ChangeScene("res://Scenes/Gameplay.tscn");
	}

	private void OnWonderChoiceGuiInput(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			var position = ((Node2D)FindNode("WonderSelectionNode")).GlobalPosition;
			var wonderSelection = (WonderSelection)FindNode("WonderSelection");
			wonderSelection.Popup_(new Rect2(position, wonderSelection.RectSize));
		}
	}

	private void OnWonderSelected(string wonder)
	{
		WonderChoice = wonder;
	}

	private void ReadSettings()
	{
		PlayerCount = GameFactory.PlayerCount;
		WonderChoice = GameFactory.WonderChoice ?? "Random";
		BoardSide = GameFactory.BoardSide;	
	}

	private void WriteSettings()
	{
		GameFactory.PlayerCount = PlayerCount;
		GameFactory.WonderChoice = (WonderChoice == "Random" ? null : WonderChoice);
		GameFactory.BoardSide = BoardSide;
	}

	private string wonderChoice;
}
