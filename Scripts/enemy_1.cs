using Godot;
using System;

public partial class enemy_1 : Area2D
{
	private const float Speed = 45;
	Vector2 velocity;
	AnimatedSprite2D enemySprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemySprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		enemySprite.Animation = "default";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Area2D pl = (Area2D)GetTree().GetFirstNodeInGroup("players");
		Vector2 playerPosition = pl.GlobalPosition;
		
		// Vector2 direction = (playerPosition - GlobalPosition).Normalized();
		Vector2 direction=GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
		velocity = direction * Speed;

		Position += velocity * (float)delta;
	}
}
