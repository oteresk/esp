using Godot;
using System;
using System.Diagnostics;

public partial class ResourceDiscoveries : Node2D
{

	[Export] private PackedScene rTemplateIronMine;
	[Export] private PackedScene rTemplateGoldMine;
	[Export] private PackedScene rTemplateManaWell;
	[Export] private PackedScene rTemplateWood;
    [Export] private PackedScene rTemplatePlatform;
<<<<<<< Updated upstream
    static private int gridSizeX=10;
	static private int gridSizeY=10;
	const int subGridSizeX = 10;
    const int subGridSizeY = 10;
=======

>>>>>>> Stashed changes
    [Export] public static int pixelSizeX=192*3;
	[Export] public static int pixelSizeY=108*3;
	[Export] public int resourcesPerCell;

<<<<<<< Updated upstream
	static private int[,] worldArray; // two-dimensional array
	// 1 = Iron Mine
	// 2 = Gold Mine
	// 3 = Mana Well
	// 4 = Tree
	// 5 = Platform
=======
	private bool mapNotPressed = true; // prevents spamming map key
>>>>>>> Stashed changes

// resource timer
	[Export] public int resourceUpdateFreq;
	private int curResourceTimer=0;

	private Node2D resourceDiscovery;
	private Node2D capTimer;

// resource tracking
	private static float gold; // how much gold you have
	static private int goldResourceCount; // how many gold mine resources you have discovered
	public static float iron;
	private static int ironResourceCount;
	public static float mana;
	private static int manaResourceCount;
	public static float wood;

    private int seconds = 0;
	private int minutes = 0;

// GUI node
	private Node rGUI;
<<<<<<< Updated upstream
	static private resourceGUI rG2;
=======

>>>>>>> Stashed changes
	public override void _Ready()
	{
	    worldArray = new int[gridSizeX*subGridSizeX, gridSizeY*subGridSizeY];
		GD.Randomize();
		//Place resource discoveries
		PlaceResourceDiscoveries();	

		// get GUI nodes
<<<<<<< Updated upstream
		rGUI = GetNode("../GUI");
        rG2 = (resourceGUI)GetNode(rGUI.GetPath());
        UpdateResourceGUI();

		// offset resourcediscoveries position to put player in middle of array
		Position=new Vector2(-(gridSizeX*subGridSizeX)/2*pixelSizeX,-(gridSizeY*subGridSizeY)/2*pixelSizeY);

        // hide structure select canvas
        CanvasLayer nodStruct = (CanvasLayer)GetNode("/root/World/StructureGUI");
        nodStruct.Visible = false;
		Debug.Print("********************* " + nodStruct.GetPath());
=======
		rGUI = GetNodeOrNull("../GUI");
		if (rGUI != null)
		{
			Globals.rG2 = (resourceGUI)GetNodeOrNull(rGUI.GetPath());
			UpdateResourceGUI();
		}

        // offset resourcediscoveries position to put player in middle of array
        Position = new Vector2(-(Globals.gridSizeX* Globals.subGridSizeX)/2*pixelSizeX,-(Globals.gridSizeY * Globals.subGridSizeY)/2*pixelSizeY);
        Node2D nodStruct = (Node2D)GetNode(Globals.NodeStructures);
		nodStruct.Position = Position;

        // hide structure select canvas
        CanvasLayer nodStructGUI = (CanvasLayer)GetNodeOrNull(Globals.NodeStructureGUI);
		if (nodStructGUI != null)
		{
            nodStructGUI.Visible = false;
			//Debug.Print("********************* " + nodStruct.GetPath());
		}
>>>>>>> Stashed changes
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
<<<<<<< Updated upstream
	}
=======
        if (Input.IsActionPressed("map") && mapNotPressed)
		{
			mapNotPressed = false;
            Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
            miniMap.Call("ShowMiniMap");
        }

		if (Input.IsActionJustReleased("map"))
			mapNotPressed = true;

    }
>>>>>>> Stashed changes

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



		// update resources
		curResourceTimer++;
		if (curResourceTimer>=resourceUpdateFreq)
		{
			curResourceTimer = 0;
            gold += goldResourceCount;
            iron += ironResourceCount;
            mana += manaResourceCount;

            UpdateResourceGUI(); 
		}

        //Debug.Print("Timer: " + minutes+":"+seconds);
<<<<<<< Updated upstream
	}

	static public void UpdateResourceGUI()
	{
		rG2.lblGold.Text = gold.ToString();
        rG2.lblIron.Text = iron.ToString();
        rG2.lblMana.Text = mana.ToString();
        rG2.lblWood.Text = wood.ToString();
=======
    }

	static public void UpdateResourceGUI()
	{
        Globals.rG2.lblGold.Text = gold.ToString();
        Globals.rG2.lblIron.Text = iron.ToString();
        Globals.rG2.lblMana.Text = mana.ToString();
        Globals.rG2.lblWood.Text = wood.ToString();
>>>>>>> Stashed changes
    }
private void PlaceResourceDiscoveries()
{
	uint rdType;

<<<<<<< Updated upstream
	for (int y=0;y<gridSizeY;y++)
		for (int x=0;x<gridSizeX;x++)
=======
	for (int y=0;y< Globals.gridSizeY;y++)
		for (int x=0;x< Globals.gridSizeX;x++)
>>>>>>> Stashed changes
			for (int i=1;i<=resourcesPerCell;i++)
			{
				//Debug.Print("RD Len: " + GetChildCount());
				Vector2I pos;
				pos= GetRandomPos(x,y);
                rdType = GD.Randi() % 5;
                PlaceDiscovery(x,y,pos.X,pos.Y,rdType);
			}
	// place some discoveries
		PlaceDiscovery(5, 5, 1, 1, 4);
        //PlaceDiscovery(5, 5, 1, 0, 1);
        //PlaceDiscovery(5, 5, 0, 1, 1);
        //PlaceDiscovery(5, 5, 1, 1, 1);
    }

	private void PlaceDiscovery(int x, int y, int px, int py, uint rdType)
<<<<<<< Updated upstream
	{ 
=======
	{
		ResourceDiscovery rdp;

>>>>>>> Stashed changes
        switch (rdType)
        {
            case 0:
                resourceDiscovery = (Node2D)rTemplateIronMine.Instantiate();
<<<<<<< Updated upstream
                worldArray[x * subGridSizeX + px, y * subGridSizeY +py] = 1;
                break;
            case 1:
                resourceDiscovery = (Node2D)rTemplateGoldMine.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 2;
                break;
            case 2:
                resourceDiscovery = (Node2D)rTemplateManaWell.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 3;
                break;
            case 3:
                resourceDiscovery = (Node2D)rTemplateWood.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 4;
                break;
            case 4:
                resourceDiscovery = (Node2D)rTemplatePlatform.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 5;
=======
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
            case 4:
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
            case 11: // tower
                resourceDiscovery = (Node2D)rTemplatePlatform.Instantiate();
                Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 5;
                SettlementSelect settle = (SettlementSelect)GetNode(Globals.NodeStructureGUICanvas);
				settle.CreateStructure(5, resourceDiscovery);
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
>>>>>>> Stashed changes
                break;
        }

        AddChild(resourceDiscovery);


        resourceDiscovery.Position = new Vector2(x*subGridSizeX * pixelSizeX + px * pixelSizeX, y*subGridSizeY * pixelSizeY + py * pixelSizeY);
        ResourceDiscovery rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());

        string myType = rdp.RDResource.resourceType.ToString();

		//Debug.Print("x:" + x + " y:" + y + " px:" + px + " py:" + py);

        Debug.Print("pos:" + resourceDiscovery.Position + " type:" + myType);
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
<<<<<<< Updated upstream
			xPos = GD.RandRange(0, subGridSizeX-1);
            yPos = GD.RandRange(0, subGridSizeY-1);
=======
			xPos = GD.RandRange(0, Globals.subGridSizeX -1);
            yPos = GD.RandRange(0, Globals.subGridSizeY -1);
>>>>>>> Stashed changes

            //Debug.Print("xRand=" + xPos + " yRand=" + yPos+" curX:"+curX+" curY:"+curY);
			//Debug.Print("worldArray[" + (curX * gridSizeX + xPos) + ", " + (curY * gridSizeY + yPos) +"]");
		}
<<<<<<< Updated upstream
		while (worldArray[curX*gridSizeX+xPos, curY*gridSizeY + yPos] != 0);
=======
		while (Globals.worldArray[curX* Globals.gridSizeX +xPos, curY* Globals.gridSizeY + yPos] != 0);
>>>>>>> Stashed changes

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
    }
	// Add one-time resource
	public static void AddResource(string resourceType,float amount,float amountMax)
	{
		if (resourceType == "Wood")
		{
			Debug.Print("*********" + amountMax);
            int rAmount = (int)(GD.Randi() % (amountMax+1));
			wood += (int)(rAmount+amount);
		}


        UpdateResourceGUI();
    }

}
