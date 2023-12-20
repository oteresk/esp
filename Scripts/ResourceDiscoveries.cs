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

private Node2D resourceDiscovery;
private Node2D capTimer;

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

	public static Vector2 GetRandomPos()
	{
			// get random x and y pos within the cell
			float xPos = (float)GD.Randi() % cellSizeX;
			float yPos = (float)GD.Randi() % cellSizeY;

		return new Vector2(xPos,yPos);
	}

}
