using Godot;
using System;

public partial class MapIcon : Node2D
{
	[Export] public Vector2 worldPos;
	public bool discovered = false;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
