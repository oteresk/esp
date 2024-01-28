using Godot;
using System;

public partial class player : Area2D
{
	[Export] public int Speed {get; set; } = 200;
    [Export] public float DashSpeed = 600;
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 1.0f;
	public float dashTimer = 0;
	
	private float dashCooldownTimer = 0;
	private bool isDashing = false;
	
	public Vector2 ScreenSize;
	Vector2 velocity; 
	AnimatedSprite2D animatedSprite2D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "idle";
		animatedSprite2D.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocity = Vector2.Zero;
		
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
		
		// If the dash cooldown is currently up, then subtract delta time from it
		if(dashCooldownTimer > 0)
			dashCooldownTimer -= (float)delta;
		
		// Check if dash needs to be set
		if (Input.IsActionPressed("dash") && !isDashing && dashCooldownTimer <= 0 && (velocity.X != 0 || velocity.Y != 0))
		{
			isDashing = true;
			dashTimer = DashDuration;
			dashCooldownTimer = DashCooldown;
		}

		// If player is dashing, multiply velocity by dash speed and check dash cooldown
		if(isDashing)
		{
			velocity *= DashSpeed;
			dashTimer -= (float)delta;
				
			if(dashTimer <= 0)
			{
				isDashing = false;
			}
		}
		// If the player is moving then multiply the velocity by speed variable
		else if (velocity.Length() > 0)
		{
			velocity *= Speed;
		}	
	
		// Moving the character around the screen
		Position += velocity * (float)delta;
		// Position = new Vector2( x: Position.X, y: Position.Y);
		
	
		// Setting the animations for the character
		if (velocity.X != 0)
		{
			if (isDashing)
				animatedSprite2D.Animation = "dash";
			else
			{
				animatedSprite2D.Animation = "walk";
				animatedSprite2D.FlipV = false;
				animatedSprite2D.FlipH = velocity.X < 0;
			}
		}
		else if (velocity.Y != 0)
		{
			if (isDashing)
				animatedSprite2D.Animation = "dash";
			else
			{
				animatedSprite2D.Animation = "walk";
			}
		}
		else 
		{
			animatedSprite2D.Animation = "idle";
		}

		// set order to y pos
//		ZIndex=(int)Position.Y;
	
	}
	
}
