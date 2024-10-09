using Godot;
using System;
using System.Diagnostics;

public partial class BackButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


    public void PressButton() // used for back button on death
    {
        Debug.Print("load upgrade scene");
        Globals.rootNode.GetTree().Paused = false;
        //().ChangeSceneToFile("res://Scenes/StatUpgrades.tscn");
        GetTree().ChangeSceneToPacked(Globals.StatUpgradesScene);
        //Globals.instance.ResetGame();
        //Globals.instance.DelayedStart();
    }


}
