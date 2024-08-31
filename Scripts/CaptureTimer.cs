using Godot;
using System;
using System.Diagnostics;

public partial class CaptureTimer : Sprite2D
{
	public CollisionObject2D col; 

	[Export] public double timer=0;
    [Export] public float captureSpeed =.2f;
	
	private bool inCapture=false;
//[Export] public ResourceDiscovery rd;

	public override void _Ready()
	{
		(Material as ShaderMaterial).SetShaderParameter("fill_ratio", 0);
//this.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (inCapture==true && Visible==true)
		{
			timer+=captureSpeed*delta;
			(Material as ShaderMaterial).SetShaderParameter("fill_ratio", timer);
		}
		else
		{
			timer -= captureSpeed * delta*2; 
			if (timer < 0)
				timer = 0;
			else
                (Material as ShaderMaterial).SetShaderParameter("fill_ratio", timer);
        }


	}

public void _on_area_2d_area_exited(Area2D area)
	{
		if (area.IsInGroup("Player"))
		{
			inCapture=false;
			Debug.Print("Player out of CaptureTimer");
			(Material as ShaderMaterial).SetShaderParameter("fill_ratio", timer);
		}
	}


public void _on_area_2d_area_entered(Area2D area)
	{
		if (area.IsInGroup("Player"))
		{
			inCapture =true;
			// Player in CaptureTimer
			Debug.Print("Player in CaptureTimer");

		}
	}


}
