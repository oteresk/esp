using Godot;
using System;
using System.Diagnostics;

public partial class Pause : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("pause"))
		{
			if (Globals.canUnPause)
			{
				if (Globals.rootNode.GetTree().Paused == false)
				{
					// show pause menu

					PauseGame();
				}
				else
					UnPauseGame();
			}
		}
	}

	public void PauseGame()
	{
		// pause game
		this.ButtonPressed = true;
		Globals.rootNode.GetTree().Paused = true;
	}

	public void UnPauseGame()
	{
		// unpause game
		this.ButtonPressed = false;
		Globals.rootNode.GetTree().Paused = false;
	}

	public void PressPause()
	{
		if (Globals.canUnPause)
		{
			if (Globals.rootNode.GetTree().Paused == false)
			{
				PauseGame();
			}
			else
				UnPauseGame();
		}
	}
}
