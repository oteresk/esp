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

    [Export] public static int pixelSizeX=192*3;
	[Export] public static int pixelSizeY=108*3;
	[Export] public int resourcesPerCell;

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

	public override void _Ready()
	{

        GD.Randomize();
		//Place resource discoveries
		PlaceResourceDiscoveries();	

		// get GUI nodes
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
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

	static public void UpdateResourceGUI()
	{
        Globals.rG2.lblGold.Text = gold.ToString();
        Globals.rG2.lblIron.Text = iron.ToString();
        Globals.rG2.lblMana.Text = mana.ToString();
        Globals.rG2.lblWood.Text = wood.ToString();
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
                rdType = GD.Randi() % 5;
                PlaceDiscovery(x,y,pos.X,pos.Y,rdType);
			}
	// place some discoveries
		PlaceDiscovery(5, 5, 1, 1, 4);
        PlaceDiscovery(5, 5, 2, 2, 1);
        //PlaceDiscovery(5, 5, 1, 0, 1);
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
            case 4:
                resourceDiscovery = (Node2D)rTemplatePlatform.Instantiate();
                Globals.worldArray[x * Globals.subGridSizeX + px, y * Globals.subGridSizeY + py] = 5;
                break;
        }

        AddChild(resourceDiscovery);


        resourceDiscovery.Position = new Vector2(x* Globals.subGridSizeX * pixelSizeX + px * pixelSizeX, y* Globals.subGridSizeY * pixelSizeY + py * pixelSizeY);

        rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());
        rdp.gridXPos = x * Globals.subGridSizeX + px;
        rdp.gridYPos = y * Globals.subGridSizeY + py;
        resourceDiscovery.Name=(rdp.gridXPos.ToString())+", "+(rdp.gridYPos.ToString());
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
