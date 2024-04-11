using Godot;
using System;
using System.Diagnostics;

public partial class GemScript : Area2D
{
    [Export] public GemType gemType;
    [Export] public float XP;

    public bool collected = false;

    private Node2D Player;
    private double speed = -1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Player = (Node2D)GetNode(Globals.NodePlayer);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (collected) // gets called when gem is in player magnetic area
        {
            Vector2 pos = Player.GlobalPosition + new Vector2(0, -50);
            GlobalPosition = GlobalPosition.MoveToward(pos, (float)speed);
            speed += 3 * delta;
        }
    }

    public void OnAreaEntered(Area2D area)
	{
        if (area.IsInGroup("Players")) // picked up by player
        {
            Globals.AddXP(XP);
            Visible = false;
            // play sound for pickup gem
            //Debug.Print("Pickup collide");
            
            //QueueFree();
        }
	}

    public enum GemType
    {
        Blue, Yellow, Green, Red
    }

    public void OnVisibilityChanged()
    {
        if(!IsVisibleInTree())
        {
            QueueFree();
        }
    }

    
}
