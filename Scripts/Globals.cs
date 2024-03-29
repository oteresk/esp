using Godot;
using System;
using System.Diagnostics;

public partial class Globals : Node
{
	static public string NodeMiniMap = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D";
    static public string NodeMiniMapContainer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/MiniMap";
    static public string NodeMiniMapPlayer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/PlayerIcon/TextureRect";
    static public string NodeStructureGUI = "/root/World/GUI/StructureGUI";
    static public string NodeStructureGUICanvas = "/root/World/GUI/StructureGUI/CanvasGroup";
    static public string NodeResourceDiscoveries = "/root/World/ResourceDiscoveries";
    static public string NodePlayer = "/root/World/Player";
    static public string NodeStructures = "/root/World/Structures";
    static public string NodeGems = "/root/World/Gems";

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

    static public int windowSizeY;
    static public int headerOffset; // used for fullscreen and not

    static private ProgressBar xpBar;

    // player attributes and stats
    static public float XP = 0;
    static public float XPGoal;
    static private Label lblLevel;
    static public int level = 1;
    static private float XPGoalIncrease = 1.5f;
    static public float magnetism = 30;


    public override void _Ready()
    {
        worldArray = new int[gridSizeX * subGridSizeX, gridSizeY * subGridSizeY];
        // windowSizeY
        // 1071 - without header
        // 1009 - with
        windowSizeY = (int)GetViewport().GetVisibleRect().Size.Y;
        headerOffset = windowSizeY - 1009;

        level = 1;
        XPGoal = 20;
        xpBar = (ProgressBar)GetNodeOrNull("../GUI/XPBar");
        lblLevel = (Label)GetNodeOrNull("../GUI/XPBar/Label");

        xpBar.Value = XP;
        UpdateLevel();
    }

    static public void AddXP(float xAdd)
    {
        XP += xAdd;

        CheckNextLevel();

        xpBar.Value = XP/XPGoal;
        //Debug.Print("XP: " + XP + " XPGoal: " + XPGoal + " final: " + xpBar.Value);
    }

    static private void CheckNextLevel()
    {
        if (XP>=XPGoal)
        {
            XP = (XP - XPGoal);
            level++;
            XPGoal = XPGoal * XPGoalIncrease;
            UpdateLevel();
        }
    }

    static private void UpdateLevel()
    {
        lblLevel.Text = "LVL: " + level.ToString();
    }



}
