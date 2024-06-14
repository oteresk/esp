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
    [Export] public PackedScene[] scnStruct;

    [Export] public Label lblIron;
    [Export] public Label lblWood;

    [Export] public int[] costIron;
    [Export] public int[] costWood;

    [Export] public string[] structName;
    [Export] public string[] structDescription;
    [Export] public Label lblStructName;
    [Export] public Label lblStructDescription;

    [Export] public TextureRect redStroke;

    private float rotateSpeed = .2f;

    //[Export] Label lblCurStruct;
    private int curStruct = 0;
    private bool buttonsEnabled = true;

    static public ResourceDiscovery platform;
    private double buildDelay = 0;
    private bool golemFactoryExists = false;

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
        if (buttonsEnabled)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    if (btn0)
                    {
                        buttonsEnabled = false;
                        curStruct -= 1;
                        UpdateCurStruct();
                        GD.Print("Rotate Right");

                        // set texture for new structure button
                        int newStruct = curStruct - 1;
                        if (newStruct < 0)
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

                        tween.Parallel().TweenProperty(GetNode("btn2"), "scale", new Vector2(0, 0), rotateSpeed);
                        btnNode2.ZIndex = -1; // put node to back of sort

                        btnNode3.Visible = true;
                        btnNode3.Scale = new Vector2(0, 0);
                        tween.Parallel().TweenProperty(GetNode("btn3"), "scale", new Vector2(.5f, .5f), rotateSpeed);

                        // reset positions
                        tween.TweenCallback(Callable.From(this.ResetButtonsLeft));
                        redStroke.Visible = false;
                    }

                    if (btn2)
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
                        GD.Print("Build structure");

                        // check if you can afford it
                        if (ResourceDiscoveries.iron >= costIron[curStruct] && ResourceDiscoveries.wood >= costWood[curStruct])
                        {
                            if (platform != null && buildDelay > 1)
                            {
                                if (curStruct == 7 && golemFactoryExists)
                                {
                                    // only allow 1 golem factory
                                }
                                else
                                {
                                    buildDelay = 0;
                                    // subtract resources
                                    ResourceDiscoveries.iron -= costIron[curStruct];
                                    ResourceDiscoveries.wood -= costWood[curStruct];
                                    // update ResourceGUI
                                    ResourceDiscoveries.UpdateResourceGUI();

                                    // build structure
                                    CreateStructure(curStruct, platform);
                                }
                            }

                        }

                    }
                }

            }
        }
    }

    public void CreateStructure(int strucNum, Node2D plat)
    {
        Debug.Print("Createstruct: " + strucNum);
        // build structure
        if (IsInstanceValid(this))
        {
            Vector2 strPos = plat.Position;
            ResourceDiscovery platformRD = (ResourceDiscovery)plat;
            int sX = platformRD.gridXPos;
            int sY = platformRD.gridYPos + 2;

            // load structure scene
            Node2D structure;
            structure = (Node2D)scnStruct[strucNum].Instantiate();

            Node2D nodRD = (Node2D)GetNode(Globals.NodeStructures);
            nodRD.AddChild(structure);

            structure.Position = new Vector2(sX * ResourceDiscoveries.pixelSizeX, sY * ResourceDiscoveries.pixelSizeY - 700);

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
            // disable buttons
            btnNode0.MouseFilter = Control.MouseFilterEnum.Ignore;
            btnNode1.MouseFilter = Control.MouseFilterEnum.Ignore;
            btnNode2.MouseFilter = Control.MouseFilterEnum.Ignore;
            btnNode3.MouseFilter = Control.MouseFilterEnum.Ignore;
            btn0 = false;
            btn1 = false;
            btn2 = false;

            // destroy platform ********call last
            //Debug.Print("Remove child: " + Globals.NodeResourceDiscoveries+"/"+plat.Name);
            Node rdNode = (Node)GetNode(Globals.NodeResourceDiscoveries);
            rdNode.RemoveChild(plat);

            if (IsInstanceValid(this))
                plat.QueueFree();

            // refresh Minimap
            Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
            miniMap.Call("DisplayMap");

            if (strucNum == 7)
                golemFactoryExists = true;

            AdjustStats(strucNum);
            Globals.UpdateStatsGUI();
        }
    }

    private void AdjustStats(int strucNum) // adjust player stats for structures that increase stats
    {
        if (strucNum == 0) // alchemy lab - inc research
        {
            ResourceDiscoveries.research++;
            ResourceDiscoveries.UpdateResourceGUI();
        }

        if (strucNum == 1) // armory - inc armor
        {
            Globals.armorLevel++;
        }

        if (strucNum == 2) // Herbalist - inc max HP
        {
            Globals.HPLevel++;
            Globals.SetMaxHP();
        }

        if (strucNum == 3) // lode stone - inc magnetism
        {
            Globals.magenetismLevel++;
            Globals.SetMagnetism();
        }
        
        if (strucNum == 6) // Training Center - inc player speed
        {
            Globals.ps.speedLevel++;
        }

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

        lblStructName.Text = structName[curStruct];
        lblStructDescription.Text = structDescription[curStruct];
    }

    private void PopulateTextures()
    {
        btnNode0.Texture = txStructBut[txStructBut.Length-1];
        btnNode1.Texture = txStructBut[0];
        btnNode2.Texture = txStructBut[1];
    }

    // update iron and wood cost
    public void UpdateCost()
    {
        lblIron.Text = costIron[curStruct].ToString();
        lblWood.Text = costWood[curStruct].ToString();

        if (costIron[curStruct] > ResourceDiscoveries.iron || costWood[curStruct] > ResourceDiscoveries.wood || (curStruct ==7 && golemFactoryExists))
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

        if (btnNode1.Visible)
        {
            // enable buttons
            btnNode0.MouseFilter = Control.MouseFilterEnum.Stop;
            btnNode1.MouseFilter = Control.MouseFilterEnum.Stop;
            btnNode2.MouseFilter = Control.MouseFilterEnum.Stop;
            btnNode3.MouseFilter = Control.MouseFilterEnum.Stop;
        }
        else
        {
            // disable buttons
            btnNode0.MouseFilter = Control.MouseFilterEnum.Ignore;
            btnNode1.MouseFilter = Control.MouseFilterEnum.Ignore;
            btnNode2.MouseFilter = Control.MouseFilterEnum.Ignore;
            btnNode3.MouseFilter = Control.MouseFilterEnum.Ignore;
            btn0 = false;
            btn1 = false;
            btn2 = false;
        }

    }

}
