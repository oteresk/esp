using Godot;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public partial class ResourceDiscoveries : Node2D
{

	[Export] private PackedScene rTemplateIronMine;
	[Export] private PackedScene rTemplateGoldMine;
	[Export] private PackedScene rTemplateManaWell;
	[Export] private PackedScene rTemplateWood;
    [Export] private PackedScene rTemplatePlatform;
    static private int gridSizeX=10;
	static private int gridSizeY=10;
	const int subGridSizeX = 10;
    const int subGridSizeY = 10;

    [Export] public static int pixelSizeX=192*3;
	[Export] public static int pixelSizeY=108*3;
	[Export] public int resourcesPerCell;

	static private int[,] worldArray; // two-dimensional array
	// 1 = Iron Mine
	// 2 = Gold Mine
	// 3 = Mana Well
	// 4 = Tree
	// 5 = Platform
	private bool mapNotPressed = true; // prevents spamming map key

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
	static private resourceGUI rG2;

	public override void _Ready()
	{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
	    worldArray = new int[gridSizeX*subGridSizeX, gridSizeY*subGridSizeY];
		GD.Randomize();
=======
=======
>>>>>>> Stashed changes
        GD.Randomize();
>>>>>>> Stashed changes
=======
        GD.Randomize();
>>>>>>> Stashed changes
		//Place resource discoveries
		PlaceResourceDiscoveries();	

		// get GUI nodes
		rGUI = GetNode("../GUI");
        rG2 = (resourceGUI)GetNode(rGUI.GetPath());
        UpdateResourceGUI();
		rGUI = GetNodeOrNull("../GUI");
		if (rGUI != null)
		{
			Globals.rG2 = (resourceGUI)GetNodeOrNull(rGUI.GetPath());
			UpdateResourceGUI();
		}

		// offset resourcediscoveries position to put player in middle of array
		Position=new Vector2(-(gridSizeX*subGridSizeX)/2*pixelSizeX,-(gridSizeY*subGridSizeY)/2*pixelSizeY);
        // offset resourcediscoveries position to put player in middle of array
        Position = new Vector2(-(Globals.gridSizeX* Globals.subGridSizeX)/2*pixelSizeX,-(Globals.gridSizeY * Globals.subGridSizeY)/2*pixelSizeY);
        Node2D nodStruct = (Node2D)GetNode(Globals.NodeStructures);
		nodStruct.Position = Position;

        // hide structure select canvas
        CanvasLayer nodStruct = (CanvasLayer)GetNode("/root/World/StructureGUI");
        nodStruct.Visible = false;
		Debug.Print("********************* " + nodStruct.GetPath());
        CanvasLayer nodStructGUI = (CanvasLayer)GetNodeOrNull(Globals.NodeStructureGUI);
		if (nodStructGUI != null)
		{
            nodStructGUI.Visible = false;
			//Debug.Print("********************* " + nodStruct.GetPath());
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
        if (Input.IsActionPressed("map") && mapNotPressed)
		{
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

		

		rG2.lblTimer.Text = strMinutes+":"+strSeconds;

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
		}

        // refresh structure gui if visible
        Node2D nodStructCanvas = (Node2D)GetNode(Globals.NodeStructureGUICanvas);
		if (nodStructCanvas.Visible == true)
			nodStructCanvas.Call("UpdateCost");

        //Debug.Print("Timer: " + minutes+":"+seconds);
	}
    }

	static public void UpdateResourceGUI()
	{
		rG2.lblGold.Text = gold.ToString();
        rG2.lblIron.Text = iron.ToString();
        rG2.lblMana.Text = mana.ToString();
        rG2.lblWood.Text = wood.ToString();
        Globals.rG2.lblGold.Text = gold.ToString();
        Globals.rG2.lblIron.Text = iron.ToString();
        Globals.rG2.lblMana.Text = mana.ToString();
        Globals.rG2.lblWood.Text = wood.ToString();
    }
private void PlaceResourceDiscoveries()
{
	uint rdType;

	for (int y=0;y<gridSizeY;y++)
		for (int x=0;x<gridSizeX;x++)
	for (int y=0;y< Globals.gridSizeY;y++)
		for (int x=0;x< Globals.gridSizeX;x++)
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
        PlaceDiscovery(5, 5, 2, 2, 11);
>>>>>>> Stashed changes
=======
        PlaceDiscovery(5, 5, 0, 0, 11);
>>>>>>> Stashed changes
=======
        PlaceDiscovery(5, 5, 0, 0, 11);
>>>>>>> Stashed changes
        //PlaceDiscovery(5, 5, 1, 0, 1);
        //PlaceDiscovery(5, 5, 0, 1, 1);
        //PlaceDiscovery(5, 5, 1, 1, 1);
    }

	private void PlaceDiscovery(int x, int y, int px, int py, uint rdType)
	{ 
	{
		ResourceDiscovery rdp;

        switch (rdType)
        {
            case 0:
                resourceDiscovery = (Node2D)rTemplateIronMine.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY +py] = 1;
                Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY +py] = 1;
                break;
            case 1:
                resourceDiscovery = (Node2D)rTemplateGoldMine.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 2;
                Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 2;
                break;
            case 2:
                resourceDiscovery = (Node2D)rTemplateManaWell.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 3;
                Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 3;
                break;
            case 3:
                resourceDiscovery = (Node2D)rTemplateWood.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 4;
                Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 4;
                break;
            case 4:
                resourceDiscovery = (Node2D)rTemplatePlatform.Instantiate();
                worldArray[x * subGridSizeX + px, y * subGridSizeY + py] = 5;
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
                break;
        }

		if (rdType != 11) // skip this if creating structure directly
		{
			AddChild(resourceDiscovery);

<<<<<<< Updated upstream
			Debug.Print("rd px:" + px + " py:" + py);
			resourceDiscovery.Position = new Vector2(x * Globals.subGridSizeX * pixelSizeX + px * pixelSizeX, y * Globals.subGridSizeY * pixelSizeY + py * pixelSizeY);

<<<<<<< Updated upstream
        resourceDiscovery.Position = new Vector2(x*subGridSizeX * pixelSizeX + px * pixelSizeX, y*subGridSizeY * pixelSizeY + py * pixelSizeY);
        ResourceDiscovery rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());

        string myType = rdp.RDResource.resourceType.ToString();
=======
			rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());
			rdp.gridXPos = x * Globals.subGridSizeX + px;
			rdp.gridYPos = y * Globals.subGridSizeY + py;
			resourceDiscovery.Name = (rdp.gridXPos.ToString()) + ", " + (rdp.gridYPos.ToString());

			string myType = rdp.RDResource.resourceType.ToString();
>>>>>>> Stashed changes

			//Debug.Print("x:" + x + " y:" + y + " px:" + px + " py:" + py);

			Debug.Print("rd pos:" + resourceDiscovery.Position + " type:" + myType);
=======
			//Debug.Print("rd px:" + px + " py:" + py);
			resourceDiscovery.Position = new Vector2(x * Globals.subGridSizeX * pixelSizeX + px * pixelSizeX, y * Globals.subGridSizeY * pixelSizeY + py * pixelSizeY);

			rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());
			rdp.gridXPos = x * Globals.subGridSizeX + px;
			rdp.gridYPos = y * Globals.subGridSizeY + py;
			resourceDiscovery.Name = (rdp.gridXPos.ToString()) + ", " + (rdp.gridYPos.ToString());

			string myType = rdp.RDResource.resourceType.ToString();

			//Debug.Print("x:" + x + " y:" + y + " px:" + px + " py:" + py);

			// Debug.Print("rd pos:" + resourceDiscovery.Position + " type:" + myType);
>>>>>>> Stashed changes
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
			xPos = GD.RandRange(0, subGridSizeX-1);
            yPos = GD.RandRange(0, subGridSizeY-1);
			xPos = GD.RandRange(0, Globals.subGridSizeX -1);
            yPos = GD.RandRange(0, Globals.subGridSizeY -1);

            //Debug.Print("xRand=" + xPos + " yRand=" + yPos+" curX:"+curX+" curY:"+curY);
			//Debug.Print("worldArray[" + (curX * gridSizeX + xPos) + ", " + (curY * gridSizeY + yPos) +"]");
		}
		while (worldArray[curX*gridSizeX+xPos, curY*gridSizeY + yPos] != 0);
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
