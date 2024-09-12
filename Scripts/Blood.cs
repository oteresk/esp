using Godot;
using System;

public partial class Blood : AnimatedSprite2D
{
    public override void _Ready()
    {
        // random scale and random playback speed
        Scale = new Vector2((float)GD.RandRange(0.6f, 1f), (float)GD.RandRange(0.6f, 1f));
        SpeedScale = (float)GD.RandRange(0.7f, 1.2f);
    }

    public void AnimationFinished()
	{
        Visible = false;
    }
}
