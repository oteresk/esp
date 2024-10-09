using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public partial class MiniMap : Node2D
{
	[Export] public PackedScene[] icon;
	private Node2D mapIcon;
	[Export] public float iconScale = .15f;
	private float iconSpacingX = 18;
	private float iconSpacingY = 18;
	private float posRatioX = 32;
	private float posRatioY = 18;

	[Export] public float zoom=18;
	[Export] public Vector2 offset;
	//[Export] public float ratio; // divide player pos by this to get map pos

	private Area2D Player;
	private Vector2 lastPos;
	private bool isVisible = false;

	private PackedScene golemIcon;
	private Node2D golemMapIcon;
	private PackedScene agroGolemIcon;
	private Node2D agroGolemMapIcon;

    ShaderMaterial minMapMat;
    Node miniMapNode;
    public Node borderNode;
    int pixelSizeX;
    int pixelSizeY;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pixelSizeX = 192 * 3; // used for golems
		pixelSizeY = 108 * 3;
		
		Player = (Area2D)GetNode(Globals.NodePlayer);

        miniMapNode = GetNode("../..");

		lastPos = new Vector2(-999999, -999999); // make sure last pos doesn't match cur player pos

		CalculateZoom();
		DisplayMap();

        CreateGolemIcons();

        DisplayGolem();
        DisplayAgroGolem();
    }


    public void CreateGolemIcons()
    {
        Debug.Print("instantiate minimap golems");
        golemIcon = icon[13];
        golemMapIcon = (Node2D)golemIcon.Instantiate();
        GetNode(Globals.NodeMiniMapContainer).AddChild((Node2D)golemMapIcon);
        golemMapIcon.Scale = new Vector2(iconScale, iconScale);
        golemMapIcon.Name = "Golem";

		// agro golem
		agroGolemIcon = icon[14];
		agroGolemMapIcon = (Node2D)agroGolemIcon.Instantiate();
		GetNode(Globals.NodeMiniMapContainer).AddChild((Node2D)agroGolemMapIcon);
		agroGolemMapIcon.Scale = new Vector2(iconScale, iconScale);
		agroGolemMapIcon.Name = "AgroGolem";
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Player.Position != lastPos) // only calculate zoom hen player moves
		{
			CalculateZoom();
			// position map reletive to player
			Position = new Vector2(-Player.Position.X / iconSpacingX, -Player.Position.Y / iconSpacingY) + offset;
		}
		lastPos= Player.Position;

	}

	public void CalculateZoom()
	{
		iconSpacingX = zoom;
		iconSpacingY = zoom;
		posRatioX = zoom * ResourceDiscoveries.pixelSizeX / ResourceDiscoveries.pixelSizeY;
		posRatioY = zoom;
	}

	public void DisplayMap()
	{
		//Debug.Print("************ Display Map **********");

		// delete child nodes if they exist
		Node rdNode = GetNode(Globals.NodeMiniMapContainer);
		if (rdNode.GetChildCount() >0)
		{
			for (int iter= rdNode.GetChildCount()-1; iter>=0; iter--)
			{
				if (rdNode.GetChild(iter).Name!= "Golem" && rdNode.GetChild(iter).Name != "AgroGolem")
					rdNode.GetChild(iter).QueueFree();
			}
		}

		DisplayResourceDiscoveries();
		DisplayStructures();
		DisplayGolem();
		DisplayAgroGolem();
	}

	private void DisplayResourceDiscoveries()
	{
		// Display Resource Discoveries on map
		foreach (Node childNode in GetNode(Globals.NodeResourceDiscoveries).GetChildren())
		{
			if (childNode.GetType() == typeof(ResourceDiscovery))
			{
				PackedScene pS;
				pS = icon[0];

				ResourceDiscovery rdp = (ResourceDiscovery)GetNode(childNode.GetPath());
				if (rdp != null)
				{
					if ((bool)childNode.Get("discovered") == true)
					{
						string myType = rdp.RDResource.resourceType.ToString();

						switch (myType)
						{
							case "Iron": // iron mine
								pS = icon[0];
								break;

							case "Gold": // gold mine
								pS = icon[1];
								break;
							case "Mana": // mana well
								pS = icon[2];
								break;
							case "Wood": // tree
								pS = icon[3];
								break;
							case "None": // Platform
								pS = icon[4];
								break;
						}

						mapIcon = (Node2D)pS.Instantiate();
						GetNode(Globals.NodeMiniMapContainer).AddChild((Node2D)mapIcon);

						mapIcon.Scale = new Vector2(iconScale, iconScale);
						mapIcon.Position = new Vector2(rdp.gridXPos * posRatioX, rdp.gridYPos * posRatioY + (Globals.headerOffset / 2));
						mapIcon.Name = myType + " " + rdp.gridXPos + ", " + rdp.gridYPos;

						// make monochrome if already discovered
						if ((bool)childNode.Get("captured") == true)
						{
							Color curColor = mapIcon.Modulate;
							//Debug.Print("time - found: " + rdp.RDResource.resourceType.ToString());
							mapIcon.Modulate = new Color(curColor.R, curColor.G, curColor.B, .5f);
						}
					}
				}

			}
		}
	}

	private void DisplayStructures()
	{
		// Display Resource Discoveries on map
		foreach (Node childNode in GetNode(Globals.NodeStructures).GetChildren())
		{
			if (childNode.GetType() == typeof(ResourceDiscovery))
			{
				//Debug.Print("Structure found: " + childNode.Name);
				PackedScene pS;
				pS = icon[0];

				ResourceDiscovery rdp = (ResourceDiscovery)GetNode(childNode.GetPath());
				if (rdp != null)
				{
					int myType = Globals.worldArray[rdp.gridXPos, rdp.gridYPos];

					pS = icon[myType];

					mapIcon = (Node2D)pS.Instantiate();
					GetNode(Globals.NodeMiniMapContainer).AddChild((Node2D)mapIcon);
					mapIcon.Scale = new Vector2(iconScale, iconScale);
					mapIcon.Position = new Vector2(rdp.gridXPos * posRatioX, rdp.gridYPos * posRatioY);
					mapIcon.Name = myType + " " + rdp.gridXPos + ", " + rdp.gridYPos;

					// make monochrome if already discovered
					if ((bool)childNode.Get("captured") == true)
					{
						Color curColor = mapIcon.Modulate;
						//Debug.Print("time - found: " + rdp.RDResource.resourceType.ToString());
						mapIcon.Modulate = new Color(curColor.R, curColor.G, curColor.B, .5f);
					}
				}

			}
		}
	}

	async public void DisplayGolem()
	{
		//Debug.Print("Display golem");
		await Task.Delay(TimeSpan.FromMilliseconds(200)); // use delay to speed up function

		if (Globals.golem!=null)
		{
			if (IsInstanceValid(Globals.golem))
			{
				if (golemMapIcon != null)
				{
					if (IsInstanceValid(golemMapIcon))
					{
						golemMapIcon.Position = new Vector2(Globals.golem.Position.X / pixelSizeX * posRatioX + 1600, Globals.golem.Position.Y / pixelSizeY * posRatioY + 931);
					}
				}
			}
		}

		if (Globals.golemAlive)
		{
			DisplayGolem();
		}
		else
			if (IsInstanceValid(golemMapIcon))
			{
				Debug.Print("kill minimmap golem");
				golemMapIcon.QueueFree();
			}     
	}

	async public void DisplayAgroGolem()
	{
		await Task.Delay(TimeSpan.FromMilliseconds(200)); // use delay to speed up function
		//Debug.Print("DisplayAgroGolem: Globals.agroGolem:"+ Globals.agroGolem);
		//Debug.Print("IsInstanceValid(Globals.agroGolem):" + IsInstanceValid(Globals.agroGolem));
		//Debug.Print("agroGolemMapIcon:" + IsInstanceValid(agroGolemMapIcon));

		if (Globals.agroGolem != null)
		{
			if (IsInstanceValid(Globals.agroGolem))
			{
				if (agroGolemMapIcon != null)
				{
					if (IsInstanceValid(agroGolemMapIcon))
					{
						agroGolemMapIcon.Position = new Vector2(Globals.agroGolem.Position.X / pixelSizeX * posRatioX + 1600, Globals.agroGolem.Position.Y / pixelSizeY * posRatioY + 931);
						//Debug.Print("DisplayAgroGolem: " + agroGolemMapIcon.Position);
					}
				}
			}
		}

		if (Globals.agroGolemAlive)
		{
			DisplayAgroGolem();
		}
		else
			if (IsInstanceValid(agroGolemMapIcon))
			{
				Debug.Print("kill minimmap agro golem");
				agroGolemMapIcon.QueueFree();
			}
	}


	public void ShowMiniMap()
	{
		if (isVisible == true) // fade out
		{
			Debug.Print("Hide map");
			isVisible = false;
			Tween tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "modulate:a", 0f, 1.0f);

			// fade shader mask
			tween.Parallel().TweenMethod(Callable.From<float>(SetShaderValue), .7f, 0f, 1.0f);

			TextureRect playerIcon = (TextureRect)GetNode(Globals.NodeMiniMapPlayer);
			tween.Parallel().TweenProperty(playerIcon, "modulate:a", 0f, 1.0f);

			// border
			// border
			tween.Parallel().TweenProperty(borderNode, "modulate:a", 0f, 1.00f);
		}
		else // fade in
		{
			Debug.Print("Show map");
			isVisible = true;
			Tween tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "modulate:a", .7f, 1.0f);

			// fade shader mask
			tween.Parallel().TweenMethod(Callable.From<float>(SetShaderValue), 0f, .7f, 1.0f);

			TextureRect playerIcon = (TextureRect)GetNode(Globals.NodeMiniMapPlayer);
			tween.Parallel().TweenProperty(playerIcon, "modulate:a", .7f, 1.0f);

			// border
			tween.Parallel().TweenProperty(borderNode, "modulate:a", .7f, 1.0f);
			
		}
	}

	private void SetShaderValue(float value)
	{

		
		//Debug.Print("mmnaode:" + miniMapNode + " val:" + value);
		SubViewportContainer svc = (SubViewportContainer)miniMapNode;

		minMapMat = (ShaderMaterial)svc.Material;

		minMapMat.SetShaderParameter("alpha", value);

	}

}
