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

        PositionMapIcons();

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		

        if (Player.Position != lastPos)
        {
            CalculateZoom();
            PositionMapIcons();
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

    public void PositionMapIcons()
    {
        foreach (Node2D childNode in GetNode(Globals.NodeMiniMapContainer).GetChildren())
        {
            Vector2 pos= (Vector2)childNode.Get("worldPos");
            childNode.Position = new Vector2(pos.X * posRatioX, pos.Y * posRatioY);
            //Debug.Print("Map Icon Pos:" + pos);
        }
    }

    // worldArray; 
    // 1 = Iron Mine
    // 2 = Gold Mine
    // 3 = Mana Well
    // 4 = Tree
    // 5 = Platform
    // 6 = Alchemy Lab
    // 7 = Blacksmith
    // 8 = Herbalist
    // 9 = Lodestone
    // 10 = Settlement
    // 11 = Tower
    // 12 = Training Center
    public void UpdateMapIcon(int mX, int mY, int stru)
    {
        //Debug.Print("mx:"+mX+" my:"+mY+"Structure: " + stru);
        foreach (Node2D childNode in GetNode(Globals.NodeMiniMapContainer).GetChildren())
        {
            Vector2 pos = (Vector2)childNode.Get("worldPos");
            if (pos.X==mX && pos.Y==mY)
            {
                Vector2 oldPos = childNode.Position;
                childNode.Free();
                PackedScene pS;
                pS = icon[stru + 5];
                mapIcon = (Node2D)pS.Instantiate();
                GetNode(Globals.NodeMiniMapContainer).AddChild((Node2D)mapIcon);
                mapIcon.Scale = new Vector2(iconScale, iconScale);
                mapIcon.Position = new Vector2(mX * posRatioX, mY * posRatioY);
                mapIcon.Set("worldPos", new Vector2(mX, mY));
            }
        }
    }


    public void DisplayMap()
    {
        // worldArray; // two-dimensional array
        // 1 = Iron Mine
        // 2 = Gold Mine
        // 3 = Mana Well

        // 5 = Platform

        // Display Resource Discoveries on map
        foreach (Node childNode in GetNode(Globals.NodeResourceDiscoveries).GetChildren())
        {
            if (childNode.GetType() == typeof(ResourceDiscovery))
            {
                if ((bool)childNode.Get("discovered") == true || (bool)childNode.Get("treeDiscovered") == true)
                {
                    PackedScene pS;
                    pS = icon[0];

                    ResourceDiscovery rdp = (ResourceDiscovery)GetNode(childNode.GetPath());
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
                    mapIcon.Position = new Vector2(rdp.gridXPos * posRatioX, rdp.gridYPos * posRatioY);
                    mapIcon.Set("worldPos", new Vector2(rdp.gridXPos, rdp.gridYPos));
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
