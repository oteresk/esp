using Godot;
using System;
using System.Diagnostics;

public partial class ResourceDiscovery : Sprite2D
{
    [Export] public ResourceDiscoveryResource RDResource;
	public bool nearResource = false;
    private ShaderMaterial mat;
	private Node2D capTimer;
	private CaptureTimer captureTimer;
	private bool discovered = false;

    public override void _Ready()
	{
		Texture2D tx=(Texture2D)RDResource.sprImage;
		this.Texture=tx;

		mat = (ShaderMaterial)this.Material;

        if (mat!=null)
        {
			mat.SetShaderParameter("saturation", .1);
            capTimer = (Node2D)GetNode("CaptureTimer");
			captureTimer = (CaptureTimer)GetNode("CaptureTimer/Progress");
        }

    }

	public void MakeSaturated()
	{
        mat.SetShaderParameter("saturation", 1);
    }

	public override void _Process(double delta)
	{
		if (mat != null)
		{
			if (nearResource && discovered==false)
			{
				//capTimer.Position = Position;
				capTimer.Visible = true;

				if (captureTimer.timer >= 1)
				{
					mat.SetShaderParameter("saturation", 1);
					capTimer.Visible = false;
					discovered = true;
				}


				//Debug.Print("timer: "+ captureTimer.timer);
			}
			else
			{
				capTimer.Visible = false;
			}
		}
	}

    // change sprite from black & white to color
    public void _on_area_2d_area_exited(Area2D area)
    {
		if (area.IsInGroup("Player") && discovered==false)
		{
			nearResource = false;
		}
	}

    public void _on_area_2d_area_entered(Area2D area)
    {
		if (area.IsInGroup("Player") && discovered==false)
		{
			nearResource = true;
		}

		// if overlapping another resource discovery, get new position
        if (area.IsInGroup("ResourceDiscovery"))
        {
			Vector2 pos = ResourceDiscoveries.GetRandomPos();
			float y = Position.Y / ResourceDiscoveries.cellSizeY;
            float x = Position.X / ResourceDiscoveries.cellSizeX;

            Position = new Vector2(x * ResourceDiscoveries.cellSizeX + pos.X, y * ResourceDiscoveries.cellSizeY + pos.Y);
			Debug.Print("Repo: "+Name+" x:" + x + " cellsizeX:" + ResourceDiscoveries.cellSizeX + " pos.x:" + pos.X+" Position: "+Position);
        }
    }

}
