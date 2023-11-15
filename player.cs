using Godot;
using System;

public partial class player : Area2D
{
	[Export]
	public int Speed {get; set; } = 100;
	public const float DashSpeed = 400;
	private const float DashDuration = 0.2f;
	private const float DashCooldown = 1.0f;
	private float dashTimer = 0;
	private float dashCooldownTimer = 0;
	private bool isDashing = false;
	public Vector2 ScreenSize;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		// Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero; // The player's movement vector.

		// Basic movement handlers
		if (Input.IsActionPressed("move_right"))
			velocity.X += 0.1f;
		if (Input.IsActionPressed("move_left"))
			velocity.X -= 0.1f;
		if (Input.IsActionPressed("move_down"))
			velocity.Y += 0.1f;
		if (Input.IsActionPressed("move_up"))
			velocity.Y -= 0.1f;
		
		velocity = velocity.Normalized();
		
		if(dashCooldownTimer > 0)
			dashCooldownTimer -= (float)delta;
		
		// Dash handler
		if (Input.IsActionPressed("dash") && !isDashing && dashCooldownTimer <= 0)
		{
			isDashing = true;
			dashTimer = DashDuration;
			dashCooldownTimer = DashCooldown;
		}
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			if(isDashing)
			{
				velocity *= DashSpeed;
				dashTimer -= (float)delta;
				
				if(dashTimer <= 0)
				{
					isDashing = false;
				}
			}
			else
			{
				velocity *= Speed;
			}	
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Animation = "idle";
		}
	
		Position += velocity * (float)delta;
		Position = new Vector2(
		x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
		y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y));
	
		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
   			animatedSprite2D.FlipV = false;
			// See the note below about boolean assignment.
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
   			animatedSprite2D.Animation = "idle";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
		if (velocity.X < 0)
		{
			animatedSprite2D.FlipH = true;
		}
		else
		{
			animatedSprite2D.FlipH = false;
		}
	}
	
}
