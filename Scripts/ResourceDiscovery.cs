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
                    Debug.Print("Add RD: " + RDResource.resourceType);
                    ResourceDiscoveries.AddRD(RDResource.resourceType.ToString(), 1);
                }
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
		if (area.IsInGroup("Players") && discovered==false)
		{
			nearResource = false;
		}
	}

	public void _on_area_2d_area_entered(Area2D area)
	{
		if (area.IsInGroup("Players") && discovered==false)
		{
			nearResource = true;
		}

	}

}
