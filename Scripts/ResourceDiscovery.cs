using Godot;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class ResourceDiscovery : Sprite2D
{
	[Export] public ResourceDiscoveryResource RDResource;
	public bool nearResource = false;
	private ShaderMaterial mat;
	private Node2D capTimer;
	private CaptureTimer captureTimer;
	private bool discovered = false;
    PackedScene scnStructureSelectGUI;
	[Export] public CanvasLayer structureSelect;
	public bool structureIsBuilt = false;
    public override void _Ready()
	{
		Texture2D tx=(Texture2D)RDResource.sprImage;
		this.Texture=tx;

		mat = (ShaderMaterial)this.Material;
		nearResource = false;

        if (mat!=null)
		{
			if (RDResource.resourceType.ToString() != "None") // if not a platform
			{
				Debug.Print("RType:" + RDResource.resourceType.ToString());
				capTimer = (Node2D)GetNode("CaptureTimer");
				captureTimer = (CaptureTimer)GetNode("CaptureTimer/Progress");

				if (RDResource.resourceType.ToString() != "Wood")
					mat.SetShaderParameter("saturation", .1);
			}
        }
        if (structureSelect != null)
        {
            structureSelect.Visible = false;
            Debug.Print("Hide Structure Select ready");
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
				if (RDResource.resourceType.ToString() != "None")
				{ 
				//capTimer.Position = Position;
				capTimer.Visible = true;

					if (captureTimer.timer >= 1)
					{
						// if frequency =0, then give one-time resource right away and not every minute
						if (RDResource.freq > 0)
						{ // not wood
							mat.SetShaderParameter("saturation", 1);
							capTimer.Visible = false;
							discovered = true;
							Debug.Print("Add RD: " + RDResource.resourceType);
							ResourceDiscoveries.AddRD(RDResource.resourceType.ToString(), 1);
						}
						else
						{ // wood
							ResourceDiscoveries.AddResource(RDResource.resourceType.ToString(), RDResource.amount, RDResource.amountMax);
							capTimer.Visible = false;
							discovered = true;
							StartCoroutine(MakeTreeFall());
						}
					}
                }
                else
                {
                    if (RDResource.resourceType.ToString() != "None")
                    {
                        capTimer.Visible = false;
                    }
                }
            }
		}
	}

	// change sprite from black & white to color
	public void _on_area_2d_area_exited(Area2D area)
	{
        if (RDResource.resourceType.ToString() == "None" && nearResource == true && area.IsInGroup("Players"))
        {
            // hide structure select canvas
            CanvasLayer nodStruct = (CanvasLayer)GetNode("/root/World/StructureGUI");
            nodStruct.Visible = false;

			nearResource = false;
			Debug.Print("Hide Structure Select");
        }
        if (area.IsInGroup("Players") && discovered == false && nearResource == true)
        {
            nearResource = false;
        }
    }

	public void _on_area_2d_area_entered(Area2D area)
	{
		//Debug.Print("Structure - nearresource:" + nearResource+ " resourceType:"+ RDResource.resourceType.ToString());
        if (RDResource.resourceType.ToString() == "None" && nearResource == false && area.IsInGroup("Players") && structureIsBuilt==false)
        {
            CanvasLayer nodStruct = (CanvasLayer)GetNode("/root/World/StructureGUI");
            nodStruct.Visible = true;
			nearResource = true;
			Debug.Print("Show Structure Select");

			// let Structure Select know which platform you are on
			SettlementSelect.platform = this;

        }
        if (area.IsInGroup("Players") && discovered == false && nearResource == false)
        {
            nearResource = true;
        }
    }

    IEnumerable MakeTreeFall()
    {
		for (int iter = 1; iter <= 90; iter++)
		{
			RotationDegrees = RotationDegrees + 1;
			yield return null;
		}
    }

    public static async void StartCoroutine(IEnumerable objects)
    {
        var mainLoopTree = Engine.GetMainLoop();
        foreach (var _ in objects)
        {
            await mainLoopTree.ToSignal(mainLoopTree, SceneTree.SignalName.ProcessFrame);
        }
    }

}
