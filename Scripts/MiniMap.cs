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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Player = (Area2D)GetNode(Globals.NodePlayer);

        lastPos = new Vector2(-999999, -999999); // make sure last pos doesn't match cur player pos

        CalculateZoom();
        DisplayMap();
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
//        Debug.Print("************ Display Map **********");

        // delete child nodes if they exist
        Node rdNode = GetNode(Globals.NodeMiniMapContainer);
        if (rdNode.GetChildCount() >0)
        {
            for (int iter= rdNode.GetChildCount()-1; iter>=0; iter--)
            {
                rdNode.GetChild(iter).QueueFree();
            }
        }

        DisplayResourceDiscoveries();
        DisplayStructures();
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
                            Debug.Print("time - found: " + rdp.RDResource.resourceType.ToString());
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
                Debug.Print("Structure found: " + childNode.Name);
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
                        Debug.Print("time - found: " + rdp.RDResource.resourceType.ToString());
                        mapIcon.Modulate = new Color(curColor.R, curColor.G, curColor.B, .5f);
                    }
                }

            }
        }
    }

    public void ShowMiniMap()
    {
        if (isVisible == true)
        {
            isVisible = false;
            Tween tween = GetTree().CreateTween();
            tween.Parallel().TweenProperty(this, "modulate:a", 0f, 1.0f);
            TextureRect playerIcon = (TextureRect)GetNode(Globals.NodeMiniMapPlayer);
            tween.Parallel().TweenProperty(playerIcon, "modulate:a", 0f, 1.0f);
        }
        else
        {
            isVisible = true;
            Tween tween = GetTree().CreateTween();
            tween.Parallel().TweenProperty(this, "modulate:a", 1.0f, 1.0f);
            TextureRect playerIcon = (TextureRect)GetNode(Globals.NodeMiniMapPlayer);
            tween.Parallel().TweenProperty(playerIcon, "modulate:a", 1.0f, 1.0f);
        }
    }

}
