using Godot;
using System;
using System.Diagnostics;

public partial class Globals : Node
{
	static public string NodeMiniMap = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D";
    static public string NodeMiniMapContainer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/MiniMap";
    static public string NodeMiniMapPlayer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/PlayerIcon/TextureRect";
    static public string NodeStructureGUI = "/root/World/StructureGUI";
    static public string NodeResourceDiscoveries = "/root/World/ResourceDiscoveries";
    static public string NodePlayer = "/root/World/Player";

    static public int gridSizeX = 10;
    static public int gridSizeY = 10;
    static public int subGridSizeX = 10;
    static public int subGridSizeY = 10;

    static public int[,] worldArray; // two-dimensional array
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

    static public bool useOcclusion = true; // use this for efficiency

    static public resourceGUI rG2; // use for updating Resource Discoveries GUI (includes timer)

    public override void _Ready()
    {
        worldArray = new int[gridSizeX * subGridSizeX, gridSizeY * subGridSizeY];
        Debug.Print("Globals: gridsizex:"+gridSizeX);
    }

}
