using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class StructureSelect : Control
{
	private int curStruct = 0;
	[Export] public TextureRect[] structures;
	[Export] public TextureRect selStructure;

    [Export] public PackedScene[] scnStruct;

    [Export] public Label lblIron;
    [Export] public Label lblWood;

    [Export] public string[] structName;
    [Export] public string[] structDescription;
    [Export] public Label lblStructName;
    [Export] public Label lblStructDescription;

    [Export] public TextureButton btnBuild;

    [Export] public TextureRect redStroke;

    static public ResourceDiscovery platform;
    private bool canBuild = true;
    private bool golemFactoryExists = false;

    public override void _Ready()
	{
        UpdateStructure();
        UpdateCost();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Left()
	{
        Debug.Print("Left");
        curStruct--;
		if (curStruct < 0)
		{
            curStruct = structures.Length-1;
		}
		UpdateStructure();
        UpdateCost();

    }
    public void Right()
    {
        curStruct++;
        if (curStruct > structures.Length - 1)
        {
            curStruct = 0;
        }
        UpdateStructure();
        UpdateCost();
    }

    public void UpdateStructure()
	{
        if (structures.Length>0)
        {
            selStructure.Texture = structures[curStruct].Texture;
            lblStructName.Text = structName[curStruct];
            lblStructDescription.Text = structDescription[curStruct];
        }
    }

    public void _OnClose()
    {
		Visible = false;
        Globals.UnPauseGame();

        if (Globals.wonGame == true) // if win game
        {
            GetTree().ChangeSceneToPacked(Globals.StatUpgradesScene);
        }

    }

    // update iron and wood cost
    public void UpdateCost()
    {
        if (structures.Length > 0)
        {
            Debug.Print("Update Cost:" + curStruct);
            if (Globals.costIron != null)
            {
                lblIron.Text = Globals.costIron[curStruct].ToString();
                lblWood.Text = Globals.costWood[curStruct].ToString();

                if (Globals.costIron[curStruct] > ResourceDiscoveries.iron || Globals.costWood[curStruct] > ResourceDiscoveries.wood || (curStruct == 2 && golemFactoryExists))
                {
                    // can't afford it
                    redStroke.Visible = true;
                }
                else
                {
                    redStroke.Visible = false;
                }
            }
        }
    }

    public void BuildStructure()
    {
        GD.Print("Build structure:"+ curStruct+" - " + structName[curStruct]);

        // check if you can afford it
        if (ResourceDiscoveries.iron >= Globals.costIron[curStruct] && ResourceDiscoveries.wood >= Globals.costWood[curStruct])
        {
            if (platform != null && canBuild==true )
            {
                if (curStruct == 2 && golemFactoryExists)
                {
                    Debug.Print("Can'r build 2 golem factories");
                    // only allow 1 golem factory
                }
                else
                {
                    canBuild = false;
                    // subtract resources
                    ResourceDiscoveries.iron -= Globals.costIron[curStruct];
                    ResourceDiscoveries.wood -= Globals.costWood[curStruct];
                    // update ResourceGUI
                    ResourceDiscoveries.UpdateResourceGUI();

                    // build structure
                    CreateStructure(curStruct, platform);
                    Visible = false;
                    Globals.UnPauseGame();
                    BuildDelay();
                }
            }

        }
    }

    private async void BuildDelay()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        canBuild = true;
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
            {
                golemFactoryExists = true;
            }

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
            Stats.UpdateStats();
        }

        if (strucNum == 1) // armory - inc armor
        {
            Globals.armorLevel++;
        }

        if (strucNum == 3) // Herbalist - inc max HP
        {
            Globals.HPLevel++;
            Globals.SetMaxHP();
        }

        if (strucNum == 4) // lode stone - inc magnetism
        {
            Globals.magenetismLevel++;
            Globals.SetMagnetism();
        }

        if (strucNum == 7) // Training Center - inc player speed
        {
            Globals.speedLevel++;
        }

    }

    public void Hover()
    {
        btnBuild.Modulate = new Color(1, 1, .2f, 1);
    }

    public void UnHover()
    {
        btnBuild.Modulate = new Color(1, 1, 1, 1);
    }


}
