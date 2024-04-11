using Godot;
using System;
using System.Diagnostics;

public partial class world : Node2D
{
	[Export] public PathFollow2D EnemySpawnPath;

    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SpawnMob()
	{
		//Debug.Print("preloading mob");
        PackedScene newMobScene = (PackedScene)ResourceLoader.Load("res://Scenes/enemy_1.tscn");
        Node2D newMob = (Node2D)newMobScene.Instantiate();

        EnemySpawnPath.ProgressRatio = (float)GD.Randf();
        newMob.GlobalPosition = ((PathFollow2D)GetNode("Player/Path2D/EnemySpawnPath")).GlobalPosition;
        AddChild(newMob);
    }

	// this is just a rudimentary way of spawning enemies; a more detailed wave-like spawning system will replace this
	public void _on_enemy_timer_timeout()
	{
		//Debug.Print("spawning mob");
		SpawnMob();
	}

}
