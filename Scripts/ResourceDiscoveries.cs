using Godot;
using System;
using System.Diagnostics;

public partial class ResourceDiscoveries : Node2D
{

	[Export] private PackedScene rTemplateIronMine;
	[Export] private PackedScene rTemplateGoldMine;
	[Export] private PackedScene rTemplateManaWell;
	[Export] private PackedScene rTemplateWood;
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

// resource timer
	[Export] public int resourceUpdateFreq;
	private int curResourceTimer=0;

	private Node2D resourceDiscovery;
	private Node2D capTimer;

// resource tracking
	private int gold; // how much gold you have
	static private int goldResourceCount; // how many gold mine resources you have discovered
	private int iron;
	private static int ironResourceCount;
	private int mana;
	private static int manaResourceCount;
	private int wood;

    private int seconds = 0;
	private int minutes = 0;

// GUI node
	private Node rGUI;
	private resourceGUI rG2;
	public override void _Ready()
	{
		worldArray = new int[gridSizeX*subGridSizeX, gridSizeY*subGridSizeY];
		GD.Randomize();
		//Place resource discoveries
		PlaceResourceDiscoveries();	

		// get GUI nodes
		rGUI = GetNode("../GUI");
        rG2 = (resourceGUI)GetNode(rGUI.GetPath());
        UpdateResourceGUI();

		// offset resourcediscoveries position to put player in middle of array
		Position=new Vector2(-(gridSizeX*subGridSizeX)/2*pixelSizeX,-(gridSizeY*subGridSizeY)/2*pixelSizeY);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

		// update resources
		curResourceTimer++;
		if (curResourceTimer>=resourceUpdateFreq)
		{
			curResourceTimer = 0;
            gold += goldResourceCount;
            iron += ironResourceCount;
            mana += manaResourceCount;
            //wood += woodResourceCount;
            UpdateResourceGUI(); 
		}

        Debug.Print("Timer: " + minutes+":"+seconds);
	}

	private void UpdateResourceGUI()
	{
		rG2.lblGold.Text = gold.ToString();
        rG2.lblIron.Text = iron.ToString();
        rG2.lblMana.Text = mana.ToString();
        rG2.lblWood.Text = wood.ToString();
    }
private void PlaceResourceDiscoveries()
{
	uint rdType;

	for (int y=0;y<gridSizeY;y++)
		for (int x=0;x<gridSizeX;x++)
			for (int i=1;i<=resourcesPerCell;i++)
			{
				//Debug.Print("RD Len: " + GetChildCount());
				Vector2I pos;
				pos= GetRandomPos(x,y);
                rdType = GD.Randi() % 4;
                PlaceDiscovery(x,y,pos.X,pos.Y,rdType);
			}
	// place some discoveries
		//PlaceDiscovery(5, 5, 0, 0, 1);
        //PlaceDiscovery(5, 5, 1, 0, 1);
        //PlaceDiscovery(5, 5, 0, 1, 1);
        //PlaceDiscovery(5, 5, 1, 1, 1);
    }

	private void PlaceDiscovery(int x, int y, int px, int py, uint rdType)
	{ 
        switch (rdType)
        {
            case 0:
                resourceDiscovery = (Node2D)rTemplateIronMine.Instantiate();
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
        }

        AddChild(resourceDiscovery);


        resourceDiscovery.Position = new Vector2(x*subGridSizeX * pixelSizeX + px * pixelSizeX, y*subGridSizeY * pixelSizeY + py * pixelSizeY);
        ResourceDiscovery rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());

        string myType = rdp.RDResource.resourceType.ToString();

		Debug.Print("x:" + x + " y:" + y + " px:" + px + " py:" + py);

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
			xPos = GD.RandRange(0, subGridSizeX-1);
            yPos = GD.RandRange(0, subGridSizeY-1);

            Debug.Print("xRand=" + xPos + " yRand=" + yPos+" curX:"+curX+" curY:"+curY);
			Debug.Print("worldArray[" + (curX * gridSizeX + xPos) + ", " + (curY * gridSizeY + yPos) +"]");
		}
		while (worldArray[curX*gridSizeX+xPos, curY*gridSizeY + yPos] != 0);

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

}
