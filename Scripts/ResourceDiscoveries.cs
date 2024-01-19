using Godot;
using System;
using System.Diagnostics;

public partial class ResourceDiscoveries : Node2D
{

	[Export] private PackedScene rTemplateIronMine;
	[Export] private PackedScene rTemplateGoldMine;
	[Export] private PackedScene rTemplateManaWell;
	[Export] private PackedScene rTemplateWood;
	[Export] public int gridSizeX;
	[Export] public int gridSizeY;
	[Export] public static int cellSizeX=1920;
	[Export] public static int cellSizeY=1080;
	[Export] public int resourcesPerCell;

	[Export] public int resourceUpdateFreq;
	private int curResourceTimer=0;

	private Node2D resourceDiscovery;
	private Node2D capTimer;

	private int gold;
	static private int goldResourceCount=0;
	private int iron;
    private static int ironResourceCount;
    private int mana;
    private static int manaResourceCount;
    private int wood;
    private static int woodResourceCount;

    private int seconds = 0;
	private int minutes = 0;

	private Node rGUI;
	private resourceGUI rG2;

    public override void _Ready()
	{
		Node pn = GetNode("../Player");

        CallDeferred("Reparent", pn);

		GD.Randomize();
		//Place resource discoveries
		PlaceResourceDiscoveries();

		rGUI = GetNode("../GUI");
        rG2 = (resourceGUI)GetNode(rGUI.GetPath());
        UpdateResourceGUI();
    }

	void Reparent(Node n)
	{
        n.GetParent().RemoveChild(n);
        AddChild(n);
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

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
            wood += woodResourceCount;
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
	int py=-(gridSizeY/2);
	int py2=gridSizeY/2;
	int px=-(gridSizeX/2);
	int px2=gridSizeX/2;
	Debug.Print("y:"+py+" to:"+py2+" x:"+px+" to:"+px2+" resourcesPerCell:"+resourcesPerCell);

	for (int y=-(gridSizeY/2);y<=gridSizeY/2;y++)
		for (int x=-(gridSizeX/2);x<=gridSizeX/2;x++)
			for (int i=1;i<=resourcesPerCell;i++)
			{
					Debug.Print("RD Len: " + GetChildCount());
                Vector2 pos = GetRandomPos();
                rdType =GD.Randi() % 4;

                switch (rdType)
				{
					case 0:
						resourceDiscovery = (Node2D)rTemplateIronMine.Instantiate();
						break;
					case 1:
						resourceDiscovery = (Node2D)rTemplateGoldMine.Instantiate();
						break;
					case 2:
						resourceDiscovery = (Node2D)rTemplateManaWell.Instantiate();
						break;
					case 3:
						resourceDiscovery = (Node2D)rTemplateWood.Instantiate();
						break;
				}

				AddChild(resourceDiscovery);


				resourceDiscovery.Position=new Vector2(x*cellSizeX+pos.X,y*cellSizeY+pos.Y);
                ResourceDiscovery rdp = (ResourceDiscovery)GetNode(resourceDiscovery.GetPath());

					string myType = rdp.RDResource.resourceType.ToString();

                    Debug.Print("pos:"+resourceDiscovery.Position+" type:"+ myType);
				
			}

	}

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
        if (resourceType == "Wood")
        {
            woodResourceCount += amount;
        }
    }
	public static Vector2 GetRandomPos()
	{
			// get random x and y pos within the cell
			float xPos = (float)GD.Randi() % cellSizeX;
			float yPos = (float)GD.Randi() % cellSizeY;

		Debug.Print("xRand=" + xPos+" yRand="+yPos);

		return new Vector2(xPos,yPos);
	}

}
