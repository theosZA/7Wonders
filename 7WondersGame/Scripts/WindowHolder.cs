using Godot;

public abstract class WindowHolder : Node2D
{
	public override void _Ready()
	{
		GetWindowDialog().Connect("popup_hide", this, "OnPopupHide");
		GetReshowButton().Connect("pressed", this, "ShowWindow");
	}

	public void ShowWindow()
	{
		if (ShouldSuppressShow())
		{
			return;
		}

		float width = GetRequiredWidth();
		float height = GetRequiredHeight();
		float left = (GetViewportRect().Size.x - width) / 2;
		GetWindowDialog().Popup_(new Rect2(new Vector2(left, 10), new Vector2(width, height)));

		GetReshowButtonHolder().Visible = false;
	}

	protected abstract WindowDialog GetWindowDialog();
	protected abstract CanvasItem GetReshowButtonHolder();
	protected abstract BaseButton GetReshowButton();

	protected abstract float GetRequiredWidth();
	protected abstract float GetRequiredHeight();

	protected virtual bool ShouldSuppressShow()
	{
		return false;
	}

	private void OnPopupHide()
	{
		GetReshowButtonHolder().Visible = !ShouldSuppressShow();
	}
}