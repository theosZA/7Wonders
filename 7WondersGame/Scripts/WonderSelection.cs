using Godot;
using Godot.Collections;
using System.Linq;

public class WonderSelection : PopupDialog
{
	public delegate void NewWonderSelectedEventHandler(string wonder);
	public NewWonderSelectedEventHandler NewWonderSelected { get; set; }

	public override void _Ready()
	{
		foreach (var node in new Array<Node>(FindNode("WonderSelectionNode").GetChildren()))
		{
			node.Connect("gui_input", this, "OnWonderGuiInput", new Array(node));
		}
	}

	private void OnWonderGuiInput(InputEvent inputEvent, Node triggeringWonderNode)
	{
		if (inputEvent is InputEventMouseButton mouseButtonEvent && mouseButtonEvent.Pressed && mouseButtonEvent.ButtonIndex == (int)ButtonList.Left)
		{
			NewWonderSelected?.Invoke(triggeringWonderNode.Name);
			Hide();
		}
	}
}
