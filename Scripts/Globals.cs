using Godot;
using System;
using System.Diagnostics;
public partial class Globals : Node
{
    static public string NodeMiniMap = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D";
    static public string NodeMiniMapContainer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/MiniMap";
    static public string NodeMiniMapPlayer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/PlayerIcon/TextureRect";
    static public string NodeStructureGUI = "/root/World/GUI/StructureGUI";
    static public string NodeGUI = "/root/World/GUI";
    static public string NodeStructureGUICanvas = "/root/World/GUI/StructureGUI/CanvasGroup";
    static public string NodeResourceDiscoveries = "/root/World/ResourceDiscoveries";
    static public string NodePlayer = "/root/World/Player";
    static public string NodeStructures = "/root/World/Structures";
    static public string NodeItems = "/root/World/Items";
    static public string NodeEnemies = "/root/World/Enemies";

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

    static public ProgressBar hpBar;
    static private ProgressBar xpBar;

    // player attributes and stats
    static public float XP = 0;
    static public float XPGoal;
    static public float HP = 100;
    static public float MaxHP = 100;
    static private Label lblLevel;
    static public int level = 1;
    static private float XPGoalIncrease = 1.5f;
    static public float magnetism = 30;

    static public bool playerAlive = true;
    static public bool playerShieldActive = false;

    static public Area2D pl;

    static public Sprite2D black;
    //    GDScript saveStateSystem;
    //    GodotObject nodSaveStateSystem;

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

        hpBar = (ProgressBar)GetNodeOrNull("../Player/HPBar");
        hpBar.Value = HP / MaxHP;

        black = (Sprite2D)GetNodeOrNull("../Black");

        // initiate save_state system
        //        saveStateSystem = GD.Load<GDScript>("res://addons/save_system/save_system.gd");
        //        nodSaveStateSystem = (GodotObject)saveStateSystem.New();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("save"))
        {
            Debug.Print("Save");
            SaveGameState();
        }

        if (Input.IsActionJustReleased("load"))
        {
            Debug.Print("Load");
            LoadGameState();
        }
    }

    static public void AddXP(float xAdd)
    {
        XP += xAdd;

        CheckNextLevel();

        xpBar.Value = XP / XPGoal;
        //Debug.Print("XP: " + XP + " XPGoal: " + XPGoal + " final: " + xpBar.Value);
    }

    static public void DamagePlayer(float xDmg)
    {
        if (playerShieldActive == false)
        {
            HP -= xDmg;

            if (HP < 0)
            {
                Debug.Print("dmg");
                HP = 0;
                playerAlive = false;
                player p = (player)pl;
                p.PlayDeathAnim();
            }

            hpBar.Value = HP / MaxHP;
        }
    }

    static private void CheckNextLevel()
    {
        if (XP >= XPGoal)
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

    private void SaveGameState()
    {
        // Player
        //nodSaveStateSystem.Call("set_var", "Player:PosX",player.Position.X.ToString()); 
        //nodSaveStateSystem.Call("set_var", "Player:PosY", player.Position.Y.ToString());
    }

    private void LoadGameState()
    {
        // Player
        //float nX= (float)nodSaveStateSystem.Call("get_var", "Player:PosX");
        //float nY = (float)nodSaveStateSystem.Call("get_var", "Player:PosY");
        //Debug.Print("Load: PlayerPos: " + nXS.ToString() + ", " + nYS.ToString());
        //player.Position = new Vector2(nX, nY);
    }

}
