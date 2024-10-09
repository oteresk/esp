using Godot;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class Status : TextureRect
{
	[Export] public float startX;
	[Export] public float endX;
	private bool overButton = false;
	private bool panelOut = false;
	private bool panelMoving = false;

	public override void _Ready()
	{
		Position = new Vector2(startX, Position.Y);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _Input(InputEvent @event)
	{

		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				if (overButton == true)
				{
					if (panelOut==false && panelMoving==false)
						PullOutPanel();

					if (panelOut == true && panelMoving == false)
						PushInPanel();
				}
			}
		}
	}
        
	async public void PullOutPanel()
	{
		panelOut = true;
        panelMoving = true;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "position", new Vector2(endX, Position.Y), 1.0f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
        await ToSignal(tween, Tween.SignalName.Finished);
        panelMoving = false;
        // pause game
        //Globals.PauseGame();
    }

    async public void PushInPanel()
    {
        panelOut = false;
        panelMoving = true;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "position", new Vector2(startX, Position.Y), 1.0f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
        await ToSignal(tween, Tween.SignalName.Finished);
        panelMoving = false;
        // unpause game
        //Globals.UnPauseGame();
    }

	public void MouseEntered()
	{
		overButton = true;
	}
	public void MouseExited()
	{
		overButton = false;
	}


}
