using Godot;
using System;
using System.Diagnostics;

public partial class ResourceDiscoveries : Node2D
{

//private PackedScene resourceDiscoveryTemplate = (PackedScene)GD.Load("res://Scenes/resource_template.tscn");
//private PackedScene resourceDiscoveryTemplate = (PackedScene)GD.Load("ResourceDiscoveries/IronMine.tres");
//RDResource=(ResourceDiscoveryResource)ResourceLoader.Load("ResourceDiscoveries/IronMine.tres").Duplicate(true);

[Export] private PackedScene rTemplateIronMine;
[Export] private PackedScene rTemplateGoldMine;
[Export] private PackedScene rTemplateManaWell;
[Export] private PackedScene rTemplateWood;

[Export] public int gridSizeX;
[Export] public int gridSizeY;
[Export] public int cellSizeX;
[Export] public int cellSizeY;
[Export] public int resourcesPerCell;

private Node2D resourceDiscovery;

	public override void _Ready()
	{
		GD.Randomize();
		//Place resource discoveries
		PlaceResourceDiscoveries();	
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

private void PlaceResourceDiscoveries()
{
	var resourceDiscoveries = new System.Collections.Generic.List<Node>();
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
				
				rdType=GD.Randi() % 4;	

				switch (rdType)
				{
					case 0:
						resourceDiscovery = (Node2D)rTemplateIronMine.Instantiate();
						Debug.Print("rTemplateIronMine");
						break;
					case 1:
						resourceDiscovery = (Node2D)rTemplateGoldMine.Instantiate();
						Debug.Print("rTemplateGoldMine");
						break;
					case 2:
						resourceDiscovery = (Node2D)rTemplateManaWell.Instantiate();
						Debug.Print("rTemplateManaWell");
						break;
					case 3:
						resourceDiscovery = (Node2D)rTemplateWood.Instantiate();
						Debug.Print("rTemplateWood");
						break;
				}

				resourceDiscoveries.Add(resourceDiscovery);
				AddChild(resourceDiscovery);

				Vector2 pos=GetRandomPos();
				resourceDiscovery.Position=new Vector2(x*cellSizeX+pos.X,y*cellSizeY+pos.Y);

				ResourceDiscovery rd=(ResourceDiscovery)GetNode(resourceDiscovery.GetPath());

				Debug.Print("pos:"+resourceDiscovery.Position+" type:"+rd.RDResource.recoverType);
				
			}

		//Sprite2D n = GetNode<Sprite2D>("ResourceTemplate");
		//n.Position=new Vector2(345,167);

		Debug.Print("Resources: "+resourceDiscoveries.Count);
	}

	private Vector2 GetRandomPos()
	{
		// get random x and y pos within the cell
		float xPos=(float)GD.Randi() % cellSizeX;
		float yPos=(float)GD.Randi() % cellSizeY;

// TODO: repeat this until it gets a pos that is at least a certain minimum distance away from all other resources

		return new Vector2(xPos,yPos);

	}

}
