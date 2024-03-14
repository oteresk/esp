using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public partial class SettlementSelect : CanvasGroup
{
	private bool btn0;
	private bool btn1;
	private bool btn2;
    [Export] public TextureRect btnNode0;
    [Export] public TextureRect btnNode1;
    [Export] public TextureRect btnNode2;
    [Export] public TextureRect btnNode3;

    [Export] public Texture2D[] txStructBut;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    [Export] public Texture2D[] txStruct;
=======
    [Export] public PackedScene[] scnStruct;
>>>>>>> Stashed changes
=======
    [Export] public PackedScene[] scnStruct;
>>>>>>> Stashed changes
=======
    [Export] public PackedScene[] scnStruct;
>>>>>>> Stashed changes

    [Export] public Label lblIron;
    [Export] public Label lblWood;

    [Export] public int[] costIron;
    [Export] public int[] costWood;
    [Export] public TextureRect redStroke;

    private float rotateSpeed = .2f;

    [Export] Label lblCurStruct;
    private int curStruct = 0;
    private bool buttonsEnabled = true;

    static public ResourceDiscovery platform;
    private double buildDelay = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        btnNode3.Visible = false;
        UpdateCurStruct();

        PopulateTextures();
        UpdateCost();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        buildDelay += delta;
    }

    public void _on_btn_0_mouse_entered()
	{
        if (buttonsEnabled)
        {
            Debug.Print("btn0 entered");
            btn0 = true;
        }
	}
    public void _on_btn_0_mouse_exited()
    {
        if (buttonsEnabled)
        {
            Debug.Print("btn0 exited");
            btn0 = false;
        }
    }
    public void _on_btn_1_mouse_entered()
    {
        if (buttonsEnabled)
        {
            Debug.Print("btn1 entered");
            btn1 = true;
        }
    }
    public void _on_btn_1_mouse_exited()
    {
        if (buttonsEnabled)
        {
            Debug.Print("btn1 exited");
            btn1 = false;
        }
    }
    public void _on_btn_2_mouse_entered()
    {
        if (buttonsEnabled)
        {
            Debug.Print("btn2 entered");
            btn2 = true;
        }
    }
    public void _on_btn_2_mouse_exited()
    {
        if (buttonsEnabled)
        {
            Debug.Print("btn2 exited");
            btn2 = false;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
			if (mouseEvent.ButtonIndex== MouseButton.Left)
			{
                Debug.Print("Button");
                if (btn0 && buttonsEnabled)
                {
                    buttonsEnabled = false;
                    curStruct -= 1;
                    UpdateCurStruct();
                    GD.Print("Rotate Right");

                    // set texture for new structure button
                    int newStruct = curStruct - 1;
                    if (newStruct < 0 )
                        newStruct = txStructBut.Length - 1;
                    btnNode3.Texture = txStructBut[newStruct];

                    btnNode2.ZIndex = 0; // reset sort index

                    btnNode3.Position = btnNode0.Position;
                    btnNode3.ZIndex = -1; // put node to back of sort
                    Tween tween = GetTree().CreateTween();

                    tween.TweenProperty(GetNode("btn0"), "position", btnNode1.Position, rotateSpeed);
                    tween.Parallel().TweenProperty(GetNode("btn0"), "scale", btnNode1.Scale, rotateSpeed);

                    tween.Parallel().TweenProperty(GetNode("btn1"), "position", btnNode2.Position, rotateSpeed);
                    tween.Parallel().TweenProperty(GetNode("btn1"), "scale", btnNode2.Scale, rotateSpeed);

                    tween.Parallel().TweenProperty(GetNode("btn2"), "scale", new Vector2(0,0), rotateSpeed);
                    btnNode2.ZIndex = -1; // put node to back of sort

                    btnNode3.Visible = true;
                    btnNode3.Scale=new Vector2(0,0);
                    tween.Parallel().TweenProperty(GetNode("btn3"), "scale", new Vector2(.5f, .5f), rotateSpeed);

                    // reset positions
                    tween.TweenCallback(Callable.From(this.ResetButtonsLeft));
                    redStroke.Visible = false;
                }

                if (btn2 && buttonsEnabled)
                {
                    buttonsEnabled = false;
                    curStruct += 1;
                    UpdateCurStruct();
                    GD.Print("Rotate Left");

                    // set texture for new structure button
                    int newStruct = curStruct + 1;
                    if (newStruct > txStructBut.Length - 1)
                        newStruct = 0;
                    btnNode3.Texture = txStructBut[newStruct];

                    btnNode0.ZIndex = 0; // reset sort index
                    btnNode3.ZIndex = -1; // put node to back of sort
                    btnNode3.Position = btnNode2.Position;
                    Tween tween = GetTree().CreateTween();

                    tween.TweenProperty(GetNode("btn2"), "position", btnNode1.Position, rotateSpeed);
                    tween.Parallel().TweenProperty(GetNode("btn2"), "scale", btnNode1.Scale, rotateSpeed);

                    tween.Parallel().TweenProperty(GetNode("btn1"), "position", btnNode0.Position, rotateSpeed);
                    tween.Parallel().TweenProperty(GetNode("btn1"), "scale", btnNode0.Scale, rotateSpeed);

                    tween.Parallel().TweenProperty(GetNode("btn0"), "scale", new Vector2(0, 0), rotateSpeed);
                    btnNode0.ZIndex = -1; // put node to back of sort

                    btnNode3.Visible = true;
                    btnNode3.Scale = new Vector2(0, 0);
                    tween.Parallel().TweenProperty(GetNode("btn3"), "scale", new Vector2(.5f, .5f), rotateSpeed);

                    // reset positions
                    tween.TweenCallback(Callable.From(this.ResetButtonsRight));
                    redStroke.Visible = false;
                }

                if (btn1) // build new structure
                {
                    GD.Print("Select");
                    GD.Print("Build structure");

                    // check if you can afford it
                    if (ResourceDiscoveries.iron >= costIron[curStruct] && ResourceDiscoveries.wood >= costWood[curStruct])
                    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                        platform.Texture = txStruct[curStruct];
                        platform.Offset = new Vector2(0, 0);
                        // disable GUI for this platform
                        platform.structureIsBuilt = true;
                        // hide structure select canvas
                        CanvasLayer nodStruct = (CanvasLayer)GetNode("/root/World/StructureGUI");
                        nodStruct.Visible = false;

                        // subtract resources
                        ResourceDiscoveries.iron -= costIron[curStruct];
                        ResourceDiscoveries.wood -= costWood[curStruct];
                        // update ResourceGUI
                        ResourceDiscoveries.UpdateResourceGUI();
=======
                        if (platform != null && buildDelay>1)
                        {
                            buildDelay = 0;
                            // subtract resources
                            ResourceDiscoveries.iron -= costIron[curStruct];
                            ResourceDiscoveries.wood -= costWood[curStruct];
                            // update ResourceGUI
                            ResourceDiscoveries.UpdateResourceGUI();

=======
                        if (platform != null && buildDelay>1)
                        {
                            buildDelay = 0;
                            // subtract resources
                            ResourceDiscoveries.iron -= costIron[curStruct];
                            ResourceDiscoveries.wood -= costWood[curStruct];
                            // update ResourceGUI
                            ResourceDiscoveries.UpdateResourceGUI();

>>>>>>> Stashed changes
                            // build structure
                            CreateStructure(curStruct, platform);

                            /*
                            Vector2 strPos = platform.Position;
                            ResourceDiscovery platformRD = (ResourceDiscovery)platform;
                            int sX = platformRD.gridXPos;
                            int sY = platformRD.gridYPos+2;

                            // load structure scene
                            Node2D structure;
                            structure = (Node2D)scnStruct[curStruct].Instantiate();

                            Node2D nodRD = (Node2D)GetNode(Globals.NodeStructures);
                            nodRD.AddChild(structure);

                            structure.Position = new Vector2(sX * ResourceDiscoveries.pixelSizeX, sY * ResourceDiscoveries.pixelSizeY);

                            ResourceDiscovery rdp = (ResourceDiscovery)GetNode(structure.GetPath()); // get resourceDiscovery of structure
                            rdp.gridXPos = sX;
                            rdp.gridYPos = sY;
                            structure.Name = scnStruct[curStruct].ResourceName+" "+sX.ToString()+" "+sY.ToString();
                            rdp.discovered = true;

                            structure.Visible = true;

                            Globals.worldArray[sX, sY] = curStruct + 5;

                            // hide structure select canvas
                            CanvasLayer nodStruct = (CanvasLayer)GetNode(Globals.NodeStructureGUI);
                            nodStruct.Visible = false;

                            // destroy platform ********call last
                            Node rdNode=(Node)GetNode(Globals.NodeResourceDiscoveries);
                            rdNode.RemoveChild(platform);

                            platform.QueueFree();

                            // refresh Minimap
                            Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
                            miniMap.Call("DisplayMap");

                            */

                        }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

                    }

                }
            }
				
        }
    }

    public void CreateStructure(int strucNum, Node2D plat)
    {
        Debug.Print("Createstruct: " + strucNum);
        // build structure
        Vector2 strPos = plat.Position;
        ResourceDiscovery platformRD = (ResourceDiscovery)plat;
        int sX = platformRD.gridXPos;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        int sY = platformRD.gridYPos;
=======
        int sY = platformRD.gridYPos+2;
>>>>>>> Stashed changes
=======
        int sY = platformRD.gridYPos+2;
>>>>>>> Stashed changes

        // load structure scene
        Node2D structure;
        structure = (Node2D)scnStruct[strucNum].Instantiate();

        Node2D nodRD = (Node2D)GetNode(Globals.NodeStructures);
        nodRD.AddChild(structure);

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        structure.Position = new Vector2(sX * ResourceDiscoveries.pixelSizeX, sY * ResourceDiscoveries.pixelSizeY);
=======
        structure.Position = new Vector2(sX * ResourceDiscoveries.pixelSizeX, sY * ResourceDiscoveries.pixelSizeY-700);
>>>>>>> Stashed changes
=======
        structure.Position = new Vector2(sX * ResourceDiscoveries.pixelSizeX, sY * ResourceDiscoveries.pixelSizeY-700);
>>>>>>> Stashed changes

        ResourceDiscovery rdp = (ResourceDiscovery)GetNode(structure.GetPath()); // get resourceDiscovery of structure
        rdp.gridXPos = sX;
        rdp.gridYPos = sY;
        structure.Name = scnStruct[strucNum].ResourceName + " " + sX.ToString() + " " + sY.ToString();
        rdp.discovered = true;

        structure.Visible = true;

        Globals.worldArray[sX, sY] = strucNum + 5;

        // hide structure select canvas
        CanvasLayer nodStruct = (CanvasLayer)GetNode(Globals.NodeStructureGUI);
        nodStruct.Visible = false;

        // destroy platform ********call last
        //Debug.Print("Remove child: " + Globals.NodeResourceDiscoveries+"/"+plat.Name);
        Node rdNode = (Node)GetNode(Globals.NodeResourceDiscoveries);
        //rdNode.RemoveChild(plat);

        plat.QueueFree();

        // refresh Minimap
        Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
        miniMap.Call("DisplayMap");
    }

    private void ResetButtonsLeft()
    {
        Debug.Print("Reset Buttons");
        btnNode2.Position = btnNode1.Position;
        btnNode2.Scale = btnNode1.Scale;
        btnNode2.Texture = btnNode1.Texture;

        btnNode1.Position = btnNode0.Position;
        btnNode1.Scale = btnNode0.Scale;
        btnNode1.Texture = btnNode0.Texture;

        btnNode0.Position = btnNode3.Position;
        btnNode0.Scale = btnNode3.Scale;
        btnNode0.Texture=btnNode3.Texture;

        btnNode3.Visible = false;
        buttonsEnabled = true;
        UpdateCost();
    }

    private void ResetButtonsRight()
    {
        Debug.Print("Reset Buttons");
        btnNode0.Position = btnNode1.Position;
        btnNode0.Scale = btnNode1.Scale;
        btnNode0.Texture = btnNode1.Texture;

        btnNode1.Position = btnNode2.Position;
        btnNode1.Scale = btnNode2.Scale;
        btnNode1.Texture = btnNode2.Texture;

        btnNode2.Position = btnNode3.Position;
        btnNode2.Scale = btnNode3.Scale;
        btnNode2.Texture = btnNode3.Texture;

        btnNode3.Visible = false;

        buttonsEnabled = true;
        UpdateCost();
    }

    private void UpdateCurStruct()
    {
        if (curStruct < 0)
            curStruct = txStructBut.Length-1;
        if (curStruct > txStructBut.Length-1)
            curStruct = 0;
        lblCurStruct.Text="Cur: "+curStruct.ToString();
    }

    private void PopulateTextures()
    {
        btnNode0.Texture = txStructBut[txStructBut.Length-1];
        btnNode1.Texture = txStructBut[0];
        btnNode2.Texture = txStructBut[1];
    }

    // update iron and wood cost
    private void UpdateCost()
    public void UpdateCost()
    {
        lblIron.Text = costIron[curStruct].ToString();
        lblWood.Text = costWood[curStruct].ToString();

        if (costIron[curStruct] > ResourceDiscoveries.iron || costWood[curStruct] > ResourceDiscoveries.wood)
        {
            // can't afford it
            redStroke.Visible = true;
        }
        else
        {
            redStroke.Visible = false;
        }

    }

    public void _on_visibility_changed()
    {
        curStruct = 0;
        PopulateTextures();
        UpdateCurStruct();
        UpdateCost();
    }

}
