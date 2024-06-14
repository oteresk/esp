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
	[Export] public AudioStreamPlayer2D sndTurnOn;
	[Export] public AudioStreamPlayer2D sndTurnOff;
    [Export] public AudioStreamPlayer2D sndDisabled;
    public bool nearResource = false;
	private ShaderMaterial mat;
	private Node2D capTimer;
	private CaptureTimer captureTimer;
	[Export] public float resourceCaptureSpeed=.2f;

	[Export] public bool captured = false;
	[Export] public bool discovered = false;
	PackedScene scnStructureSelectGUI;
	[Export] public CanvasLayer structureSelect;
	//public bool structureIsBuilt = false;   No longer needed because platform gets deleted when structure is built
	public int gridXPos;
	public int gridYPos;
	[Export] public GpuParticles2D smoke;
	public bool factoryisOn = true;
	[Export] public ProgressBar progressBar;
	[Export] public Light2D light1;
	[Export] public Light2D light2;
	[Export] public Light2D light3;
	private bool factoryPaused = false;
	public bool switchEnabled = true;


    public override void _Ready()
	{
		if (Name== "Golem Factory")
		{
			if (progressBar != null)
				progressBar.Value = 0;

			if (!factoryisOn)
			{
				smoke.Visible = false;
				light1.Visible = false;
				light2.Visible = false;
				light3.Visible = false;
			}
		}
		
		if (RDResource != null)
		{
			Texture2D tx = (Texture2D)RDResource.sprImage;
			this.Texture = tx;

			mat = (ShaderMaterial)this.Material;
			nearResource = false;

			if (mat != null)
			{
				if (RDResource.resourceType.ToString() != "None") // if not a platform
				{
					//Debug.Print("RType:" + RDResource.resourceType.ToString());
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
		if (Globals.useOcclusion)
			Visible = false;
	}

	public void MakeSaturated()
	{
		mat.SetShaderParameter("saturation", 1);
	}

	public override void _Process(double delta)
	{
		if (mat != null)
		{
			if (nearResource && captured==false)
			{
				if (RDResource != null)
				{

					if (RDResource.resourceType.ToString() != "None")
					{
						//capTimer.Position = Position;
						capTimer.Visible = true;

						if (captureTimer.timer >= 1)
						{
							// if frequency =0, then give one-time resource right away and not every minute
							if (RDResource.freq > 0)
							{ // not wood or platform
								mat.SetShaderParameter("saturation", 1);
								capTimer.Visible = false;
								captured = true;
								Debug.Print("Add RD: " + RDResource.resourceType);
								ResourceDiscoveries.AddRD(RDResource.resourceType.ToString(), 1);
								// update minimap
								Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
								miniMap.Call("DisplayMap");

							}
							else
							{ // wood or platform
								if (RDResource.resourceType.ToString()=="Wood") // if tree
								{
									ResourceDiscoveries.AddResource(RDResource.resourceType.ToString(), RDResource.amount, RDResource.amountMax);
									capTimer.Visible = false;
									captured = true;
									StartCoroutine(MakeTreeFall());
									// update minimap
									Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
									miniMap.Call("DisplayMap");
								}
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
	}

	// change sprite from black & white to color
	public void _on_area_2d_area_exited(Area2D area)
	{
		if (RDResource != null)
		{
			// exit platform
			if (RDResource.resourceType.ToString() == "None" && nearResource == true && area.IsInGroup("Players"))
			{
				// hide structure select canvas
				CanvasLayer nodStruct = (CanvasLayer)GetNode(Globals.NodeStructureGUI);
				nodStruct.Visible = false;

				nearResource = false;
				Debug.Print("Hide Structure Select");
			}
		}
		// not near resource
		if (area.IsInGroup("Players") && captured == false && nearResource == true)
		{
			nearResource = false;
		}
	}

	public void _on_area_2d_area_entered(Area2D area)
	{
		//Debug.Print("Structure - nearresource:" + nearResource+ " resourceType:"+ RDResource.resourceType.ToString());
		// platform
		if (RDResource != null)
		{
			if (RDResource.resourceType.ToString() == "None" && nearResource == false && area.IsInGroup("Players"))
			{
				CanvasLayer nodStruct = (CanvasLayer)GetNode(Globals.NodeStructureGUI);
				nodStruct.Visible = true;
				nearResource = true;
				Debug.Print("Show Structure Select");

				// let Structure Select know which platform you are on
				SettlementSelect.platform = this;

			}
		}
		// player near resource
		if (area.IsInGroup("Players") && captured == false && nearResource == false)
		{
			nearResource = true;
			// set capture speed
			if (captureTimer!=null)
				captureTimer.captureSpeed = resourceCaptureSpeed;
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

	public void _on_occlusion_area_collider_area_entered(Area2D area)
	{
		if (area.IsInGroup("Players") && Globals.useOcclusion)
		{
			Visible = true;
			//Debug.Print("occlusion: enter:" + RDResource.ToString());
			
		}
	}

	public void _on_occlusion_area_collider_area_exited(Area2D area)
	{
		if (area.IsInGroup("Players") && Globals.useOcclusion)
		{
			Visible = false;
			//Debug.Print("occlusion: exit" + RDResource.ToString());
		}
	}

	public void OnDiscoveryColliderEnter(Area2D area)
	{
		if (area.IsInGroup("Players") && discovered == false)
		{
			discovered = true;
			// update minimap
			Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
			miniMap.Call("DisplayMap");
		}
	}

	public void ToggleSwitch(bool button_pressed)
	{
		if (switchEnabled)
		{
            if (button_pressed)
            {
                //smoke.Emitting = true;
                light1.Visible = true;
                light2.Visible = true;
                light3.Visible = true;
                factoryisOn = true;
                sndTurnOn.Play();
            }
            else
            {
                //smoke.Emitting = false;
                light1.Visible = false;
                light2.Visible = false;
                light3.Visible = false;
                factoryisOn = false;
                sndTurnOff.Play();
            }
        }
		else
		{
			sndDisabled.Play();
		}
	}

	// shut off smoke and flash lights when you don't have any mana to operate factory
	public void PauseFactory()
	{
		factoryPaused = true;
        smoke.Emitting = false;
		FlashLight();
    }

    public void CompleteFactory()
    {
        factoryPaused = false;
        smoke.Emitting = false;
        light1.Visible = false;
        light2.Visible = false;
        light3.Visible = false;
    }

    private async void FlashLight()
	{
        await Task.Delay(TimeSpan.FromMilliseconds(2 * 1000));
		if (IsInstanceValid(light1) && IsInstanceValid(light2) && IsInstanceValid(light3))
		{
            if (factoryPaused && factoryisOn)
            {
                light1.Visible = false;
                light2.Visible = false;
                light3.Visible = false;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(2 * 1000));
            if (factoryisOn)
            {
                light1.Visible = true;
                light2.Visible = true;
                light3.Visible = true;
                if (factoryPaused)
                    FlashLight();
            }
        }
    }

    public void StartFactory()
    {
		factoryPaused = false;
        smoke.Emitting = true;
        light1.Visible = true;
        light2.Visible = true;
        light3.Visible = true;
    }

}
