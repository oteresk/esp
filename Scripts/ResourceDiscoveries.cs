using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Godot.HttpRequest;


public partial class ResourceDiscoveries : Node2D
{
// Structures and resources
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
    // 13 = Golem Factory

    [Export] private PackedScene rTemplateIronMine;
	[Export] private PackedScene rTemplateGoldMine;
	[Export] private PackedScene rTemplateManaWell;
	[Export] private PackedScene rTemplateWood;
	[Export] private PackedScene rTemplatePlatform;

	[Export] public static int pixelSizeX=192*3;
	[Export] public static int pixelSizeY=108*3;
	[Export] public int resourcesPerCell;

	static public bool mapNotPressed = true; // prevents spamming map key

// resource timer
	[Export] public int resourceUpdateFreq;
	private int curResourceTimer=0;

	private Node2D resourceDiscovery;
	private Node2D capTimer;

	// resource tracking
	static public float gold; // how much gold you have
	static public int goldResourceCount; // how many gold mine resources you have discovered
	static public float iron;
	static public int ironResourceCount;
	static public float mana;
	static public int manaResourceCount;
	static public float wood;
    static public int research;

    static public int seconds = 0;
	static public int minutes = 0;

	// GUI node
	private Node rGUI;

    [Export] public PackedScene relicScene;

    public override void _Ready()
	{
        GD.Randomize();
        //Place resource discoveries
        PlaceResourceDiscoveries();

		DelayedStart();

        // get GUI nodes
        rGUI = GetNodeOrNull("../GUI");
        if (rGUI != null)
        {
            Globals.rG2 = (resourceGUI)GetNodeOrNull(rGUI.GetPath());
            UpdateResourceGUI();
        }

        // offset resourcediscoveries position to put player in middle of array
        Position = new Vector2(-(Globals.gridSizeX * Globals.subGridSizeX) / 2 * pixelSizeX, -(Globals.gridSizeY * Globals.subGridSizeY) / 2 * pixelSizeY);
        Node2D nodStruct = (Node2D)GetNodeOrNull(Globals.NodeStructures);
		if (nodStruct!=null)
	        nodStruct.Position = Position;

        // hide structure select canvas
        CanvasLayer nodStructGUI = (CanvasLayer)GetNodeOrNull(Globals.NodeStructureGUI);
        if (nodStructGUI != null)
        {
            nodStructGUI.Visible = false;
            //Debug.Print("********************* " + nodStruct.GetPath());
        }

    }

	private async void DelayedStart()
	{
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        // place relics
        if (Globals.xpBar != null) // dont place if on Stat Upgrade menu
            PlaceRelics();
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("map") && mapNotPressed)
		{
			Debug.Print("Pressed 'm'");
			mapNotPressed = false;
			Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
			miniMap.Call("ShowMiniMap");
		}

		if (Input.IsActionJustReleased("map"))
			mapNotPressed = true;

	}

// Game Timer - also updates resources every minute
	public void OnTimer()
	{
		seconds++;
		if (seconds>59)
		{
			seconds = 0;
			minutes++;

		}

		string strSeconds = seconds.ToString();
		if (seconds < 10)
			strSeconds = "0" + strSeconds;

		string strMinutes = minutes.ToString();

		if (minutes < 10)
			strMinutes = "0" + strMinutes;



		Globals.rG2.lblTimer.Text = strMinutes+":"+strSeconds;

		// update resources
		curResourceTimer++;
		if (curResourceTimer>=resourceUpdateFreq)
		{
			curResourceTimer = 0;
			gold += goldResourceCount;
			iron += ironResourceCount;
			mana += manaResourceCount;

			UpdateResourceGUI(); 
			SaveLoad.SaveGame(); // save gold
		}

		// refresh structure gui if visible
		Node2D nodStructCanvas = (Node2D)GetNode(Globals.NodeStructureGUICanvas);
		if (nodStructCanvas.Visible == true)
			nodStructCanvas.Call("UpdateCost");

		//Debug.Print("Timer: " + minutes+":"+seconds);
	}

	static public void UpdateResourceGUI()
	{
		if (Globals.rG2!=null)
		{
            Globals.rG2.lblGold.Text = gold.ToString();
            Globals.rG2.lblIron.Text = iron.ToString();
            Globals.rG2.lblMana.Text = mana.ToString();
            Globals.rG2.lblWood.Text = wood.ToString();
            Globals.rG2.lblResearch.Text = research.ToString();

            // update Tooltips
            MarginContainer toolTip = (MarginContainer)Globals.rG2.lblGold.GetParent().GetParent();
            toolTip.TooltipText = "Gold Mines: " + ResourceDiscoveries.goldResourceCount;

            toolTip = (MarginContainer)Globals.rG2.lblIron.GetParent().GetParent();
            toolTip.TooltipText = "Iron Mines: " + ResourceDiscoveries.ironResourceCount;

            toolTip = (MarginContainer)Globals.rG2.lblMana.GetParent().GetParent();
            toolTip.TooltipText = "Mana Wells: " + ResourceDiscoveries.manaResourceCount;

			// platform button
			// TODO
			/*
			if (mana>=Globals.platformManaCost)
			{
				Globals.rG2.platformAvailable = true;
                Globals.rG2.FlashPlatform();
            }
			else
				Globals.rG2.platformAvailable = false;

			*/

        }       
    }

	private void PlaceRelics()
	{
		float relicX, relicY;
		int numRelics = 5;
        bool[] pickedRelic=new bool[Globals.MAXRELICS];

        int needed = Globals.GetRelicsNeeded();
        Debug.Print("Globals.GetRelicsNeeded() "+ needed);

        if (numRelics > needed)
			numRelics = needed;

		if (numRelics > 0)
		{
			for (int i = 0; i < numRelics; i++)
			{
				PickRandomRelic:
				int rType = GD.RandRange(0, Globals.MAXRELICS-1);
				//Debug.Print("RandRelic: " + rType);
				if (Globals.hasRelic[rType] || pickedRelic[rType])
					goto PickRandomRelic;

				pickedRelic[rType] = true;
                Area2D rel = (Area2D)relicScene.Instantiate();
				Relic relic = (Relic)rel;
				relic.SetRelic(rType);


GetNewPos:
				relicX = GD.RandRange(0, (Globals.gridSizeX - 2) * Globals.subGridSizeX * pixelSizeX + (Globals.subGridSizeX - 2) * pixelSizeX);
				relicX -= ((Globals.gridSizeX - 2) * Globals.subGridSizeX * pixelSizeX + (Globals.subGridSizeX - 2) * pixelSizeX) / 2;
				relicY = GD.RandRange(0, (Globals.gridSizeY - 2) * Globals.subGridSizeY * pixelSizeY + (Globals.subGridSizeY - 2) * pixelSizeY);
				relicY -= ((Globals.gridSizeY - 2) * Globals.subGridSizeY * pixelSizeY + (Globals.subGridSizeY - 2) * pixelSizeY) / 2;

				relic.Position = new Vector2(relicX, relicY);

				float dist = relic.GlobalPosition.DistanceTo(new Vector2(0, 0));
				if (dist < 11000) // make sure distance to relic is >11000
					goto GetNewPos;
				//Debug.Print("relic dist:" + dist);

				Node2D nodItems = (Node2D)GetNode(Globals.NodeItems);
				nodItems.AddChild(relic);

                Debug.Print("Relic: " + relic.GetRelicName(i) + relicX + " , " + relicY);

            }
        }
	}
private void PlaceResourceDiscoveries()
{
	uint rdType;

	for (int y=0;y< Globals.gridSizeY;y++)
		for (int x=0;x< Globals.gridSizeX;x++)
			for (int i=1;i<=resourcesPerCell;i++)
			{
				//Debug.Print("RD Len: " + GetChildCount());
				Vector2I pos;
				pos= GetRandomPos(x,y);
            PickResource:
                rdType = GD.Randi() % 5;

				if (rdType == 1 && GD.RandRange(0, 1) == 1) // make fewer gold mines by randomly picking a different resource if gold
					goto PickResource;

				PlaceDiscovery(x,y,pos.X,pos.Y,rdType);
			}

		PlaceStartingStructures();	
	}

	public void PlaceStartingStructures()
	{
        Debug.Print("PlaceStartingStructures");

        // place some discoveries
        if (Globals.StartingPlatform)
        {
            PlaceDiscovery(5, 5, 0, -2, 4);
            Debug.Print("starting platform");
        }
        if (Globals.StartingTower)
        {
            PlaceDiscovery(5, 5, 0, 0, 11);
            Debug.Print("starting tower");
        }

        //		PlaceDiscovery(5, 5, 8, 0, 4);
        //PlaceDiscovery(5, 5, 0, 1, 1);
        //PlaceDiscovery(5, 5, 1, 1, 1);
    }

    private void PlaceDiscovery(int x, int y, int px, int py, uint rdType)
	{
		ResourceDiscovery rdp;

		switch (rdType)
		{
			case 0:
				resourceDiscovery = (Node2D)rTemplateIronMine.Instantiate();
				Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY +py] = 1;
				break;
			case 1:
				resourceDiscovery = (Node2D)rTemplateGoldMine.Instantiate();
				Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 2;
				break;
			case 2:
				resourceDiscovery = (Node2D)rTemplateManaWell.Instantiate();
				Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 3;
				break;
			case 3:
				resourceDiscovery = (Node2D)rTemplateWood.Instantiate();
				Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 4;
				break;
			case 4: // platform
				resourceDiscovery = (Node2D)rTemplatePlatform.Instantiate();
				Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 5;
				break;
			case 11: // tower
				resourceDiscovery = (Node2D)rTemplatePlatform.Instantiate();
				Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 5;

				AddChild(resourceDiscovery);
				resourceDiscovery.Position = new Vector2(x * Globals.subGridSizeX * pixelSizeX + px * pixelSizeX, y * Globals.subGridSizeY * pixelSizeY + py * pixelSizeY);
				rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());
				rdp.gridXPos = x * Globals.subGridSizeX + px;
				rdp.gridYPos = y * Globals.subGridSizeY + py;
				resourceDiscovery.Name = (rdp.gridXPos.ToString()) + ", " + (rdp.gridYPos.ToString());
				// convert to structure
				SettlementSelect settle = (SettlementSelect)GetNode(Globals.NodeStructureGUICanvas);
				settle.CreateStructure(5, resourceDiscovery);
				break;
		}

		if (rdType != 11) // skip this if creating structure directly
		{
			AddChild(resourceDiscovery);

			//Debug.Print("rd px:" + px + " py:" + py);
			resourceDiscovery.Position = new Vector2(x * Globals.subGridSizeX * pixelSizeX + px * pixelSizeX, y * Globals.subGridSizeY * pixelSizeY + py * pixelSizeY);

			rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());
			rdp.gridXPos = x * Globals.subGridSizeX + px;
			rdp.gridYPos = y * Globals.subGridSizeY + py;
			resourceDiscovery.Name = (rdp.gridXPos.ToString()) + ", " + (rdp.gridYPos.ToString());

			string myType = rdp.RDResource.resourceType.ToString();

			//Debug.Print("x:" + x + " y:" + y + " px:" + px + " py:" + py);

			// Debug.Print("rd pos:" + resourceDiscovery.Position + " type:" + myType);
		}
		
	}

	// returns a random x and y pos between 0-9
	// it repeats the check until the worldArray location has 0 value
	public static Vector2I GetRandomPos(int curX, int curY)
	{
		int xPos;
		int yPos;

		do
		{
			// get random x and y pos 
			xPos = GD.RandRange(0, Globals.subGridSizeX -1);
			yPos = GD.RandRange(0, Globals.subGridSizeY -1);

			//Debug.Print("xRand=" + xPos + " yRand=" + yPos+" curX:"+curX+" curY:"+curY);
			//Debug.Print("worldArray[" + (curX * gridSizeX + xPos) + ", " + (curY * gridSizeY + yPos) +"]");
		}
		while (Globals.worldArray[curX* Globals.gridSizeX +xPos, curY* Globals.gridSizeY + yPos] != 0);

		return new Vector2I(xPos,yPos);
	}

	// this gets called from the apropriate ResourceDiscovery.cs
		public static void AddRD(string resourceType,int amount) 
	{
		if (resourceType=="Gold")
		{
			goldResourceCount += amount;
		}
		if (resourceType == "Iron")
		{
			ironResourceCount += amount;
		}
		if (resourceType == "Mana")
		{
			manaResourceCount += amount;
		}

		UpdateResourceGUI();

    }
	// Add one-time resource
	public static void AddResource(string resourceType,float amount,float amountMax)
	{
		if (resourceType == "Wood")
		{
			//Debug.Print("*********" + amountMax);
			int rAmount = (int)(GD.Randi() % (amountMax+1));
			wood += (int)(rAmount+amount);
		}
		UpdateResourceGUI();
	}

	public static int GetMinutes() // gets the number of minutes passed
	{
		return minutes;
	}

}
