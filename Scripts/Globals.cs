using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Globals : Node
{
	static public string NodeMiniMap = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D";
	static public string NodeMiniMapContainer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/MiniMap";
	static public string NodeMiniMapPlayer = "/root/World/MiniMapCanvas/Control/SubViewportContainer/SubViewport/Node2D/PlayerIcon/TextureRect";
	static public string NodeStructureGUI = "/root/World/GUI/StructureGUI";
	static public string NodeGUI = "/root/World/GUI";
	static public string NodeStructureGUICanvas = "/root/World/GUI/StructureGUI/SettlementSelect";
	static public string NodeResourceDiscoveries = "/root/World/ResourceDiscoveries";
	static public string NodePlayer = "/root/World/Player";
	static public string NodeWorld = "/root/World";
	static public string NodeStructures = "/root/World/Structures";
	static public string NodeItems = "/root/World/Items";
	static public string NodeEnemies = "/root/World/Enemies";
	static public string NodePoison = "/root/World/GUI/Control/MarginContainer/PoisonGUINode";
	static public string NodeGlobals = "/root/World/Globals";

	static Node rootNode;

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
									 // 13 = Golem Factory

	static public bool useOcclusion = true; // use this for efficiency

	static public resourceGUI rG2; // use for updating Resource Discoveries GUI (includes timer)

	static public int windowSizeY;
	static public int headerOffset; // used for fullscreen and not

	static public ProgressBar hpBar;
	static public ProgressBar xpBar;

	// player attributes and stats
	static public float XP = 0;
	static public float XPGoal;
	static public float HP;
	static public float MaxHP;
	static public int HPLevel = 1;
	static public float HPInc = 10;

    static public Label lblLevel;
	static public int level = 1;
	static public float XPGoalIncrease = 1.5f;
	static public float magnetism;
	static public int magenetismLevel;
	static public float magenetismInc = 10;

	static public int armorLevel;

	static public bool playerAlive = true;
	static public bool playerShieldActive = false;

	static public Area2D pl;
	static public player ps;

	static public Node2D golem;
	static public bool golemAlive = false;

	static public HBoxContainer poisonEffect;
	static public bool isPoisoned=false;
	static public int poisonCount;
	static public List <Node> poisonNodes;

	static public float itemAtkSpd; // 1 (default) .5 (with item)
	static public float permItemAtkSpd = 0; // 0-25

	static public resourceGUI GUINode;

	static private Label lblMaxHealth;
    static private Label lblMovementSpeed;
    static private Label lblMagnetism;
    static private Label lblArmor;
    static private Label lblTowerLevel;
    static private Label lblGolemLevel;

    static public Sprite2D black;
	//    GDScript saveStateSystem;
	//    GodotObject nodSaveStateSystem;

	static public int potionFreq = 26; // how often enemy drops a potion instead of gem

	const int MAXPOISONS= 3;

	public override void _Ready()
	{
		worldArray = new int[gridSizeX * subGridSizeX, gridSizeY * subGridSizeY];
		// windowSizeY
		// 1071 - without header
		// 1009 - with
		windowSizeY = (int)GetViewport().GetVisibleRect().Size.Y;
		headerOffset = windowSizeY - 1009;

		DelayedStart();

		rootNode = GetNode("..");
        // "GUI/Control/MarginContainer/StatsGUI/HBoxContainer/MaxHealth/VBoxContainer/lblMaxHealth"
        // get stat labels     GUI/Control/MarginContainer/StatsGUI/HBoxContainer/MovementSpeed/VBoxContainer/lblMovementSpeed
        Node lbl = GetNode("../GUI/Control/MarginContainer/StatsGUI/HBoxContainer/MaxHealth/VBoxContainer/lblMaxHealth");
		lblMaxHealth = (Label)lbl;
        lbl = GetNode("../GUI/Control/MarginContainer/StatsGUI/HBoxContainer/MovementSpeed/VBoxContainer/lblMovementSpeed");
        lblMovementSpeed=(Label)lbl;
        lbl = GetNode("../GUI/Control/MarginContainer/StatsGUI/HBoxContainer/Magnetism/VBoxContainer/lblMagnetism");
        lblMagnetism = (Label)lbl;
        lbl = GetNode("../GUI/Control/MarginContainer/StatsGUI/HBoxContainer/Armor/VBoxContainer/lblArmor");
        lblArmor = (Label)lbl;

        ResetGame();
    }

	async void DelayedStart()
	{
		// wait a bit
		await Task.Delay(TimeSpan.FromMilliseconds(200));
        UpdateStatsGUI();
    }

	public void ResetGame()
	{
		GUINode = (resourceGUI)GetNode(NodeGUI);

		playerAlive = true;
		level = 1;
		XPGoal = 20;
		xpBar = (ProgressBar)GetNodeOrNull("../GUI/XPBar");
		lblLevel = (Label)GetNodeOrNull("../GUI/XPBar/Label");
		xpBar.Value = XP;
		UpdateLevel();
		itemAtkSpd = 1; // attackSpeed modifier for temp items

		black = (Sprite2D)GetNodeOrNull("../Black");

		// hide poison effect
		poisonEffect = (HBoxContainer)GetNode(NodePoison);

		poisonNodes = new List<Node>();

		Globals.XP = 0;

		SetMaxHP();
		Debug.Print("maxHP:" + MaxHP);
		Globals.HP = MaxHP;
        hpBar = (ProgressBar)GetNodeOrNull("../Player/HPBar");
        hpBar.Value = HP / MaxHP;

        XPGoalIncrease = 1.5f;
		magnetism = 30;
        magenetismLevel = 1;

        armorLevel = 1;

		ResourceDiscoveries.seconds = 0;
		ResourceDiscoveries.minutes = 0;

		ResourceDiscoveries.iron = 10;
		ResourceDiscoveries.mana = 0;
		ResourceDiscoveries.wood = 10;
		ResourceDiscoveries.gold = 0;
		ResourceDiscoveries.goldResourceCount = 0;
		ResourceDiscoveries.ironResourceCount = 0;
		ResourceDiscoveries.manaResourceCount = 0;

		Node2D fG = (Node2D)GetNodeOrNull("../FriendlyGolem");
		if (fG != null)
		{
			Globals.golem = fG;
			golemAlive = true;

            Debug.Print("Golem not null");
		}

		magenetismLevel = 1;
		SetMagnetism();
		if (ps !=null)
			ps.SetMagnetismShape();

        // initiate save_state system
        //        saveStateSystem = GD.Load<GDScript>("res://addons/save_system/save_system.gd");
        //        nodSaveStateSystem = (GodotObject)saveStateSystem.New();
    }

	static public void SetMaxHP()
	{
		Globals.MaxHP = 90 + HPInc * HPLevel*HPLevel;
		if (hpBar!=null)
			if (IsInstanceValid(hpBar))
				hpBar.Value = HP / MaxHP;
    }

	static public void SetMagnetism()
	{
		magnetism = 20 + magenetismLevel * magenetismInc;
        if (ps != null)
            ps.SetMagnetismShape();
    }

	static public void PoisonPlayer(float dmg, float dmgTime)
	{
		if (poisonCount<MAXPOISONS && playerShieldActive == false)
		{
			Debug.Print("Poison player");
			var poisonGUIScene = (PackedScene)ResourceLoader.Load("res://Scenes/PoisonGUI.tscn");
			var newPoisonGUI = (Control)poisonGUIScene.Instantiate();
			poisonEffect.AddChild(newPoisonGUI);


			isPoisoned = true;
			poisonCount++;

			ps.poisonSpeed = .5f;

			// instantiate poison object on player
			var poisonScene = (PackedScene)ResourceLoader.Load("res://Scenes/poison.tscn");
			var newPoison = (AnimatedSprite2D)poisonScene.Instantiate();
			newPoison.Play();
			//Debug.Print("pl: "+pl.Name);
			pl.AddChild(newPoison);
			var pScript = (Poison)newPoison;
			pScript.poisonTime = dmgTime;
			pScript.poisonDamage = dmg;
			pScript.pTarget = Poison.PoisonTarget.Player;
			poisonNodes.Add(newPoison);
		}
	}

	public static void PoisonEnded()
	{
		poisonCount--;

		// remove first GUI poisin effect
		Control pe = (Control)poisonEffect.GetChild(0);
		pe.QueueFree();

		poisonNodes.RemoveAt(0); // remove first node in array/list

		if (poisonCount == 0)
		{
			isPoisoned = false;

			player p = (player)pl;
			p.poisonSpeed = 1;
		}
	}

	public static void RemoveAllPoison()
	{
		poisonCount = 0;
		isPoisoned = false;
		Control peCont;
		Control pe;

		if (poisonEffect.GetChildCount() > 0)
		{
			for (int iter = 1; iter <= MAXPOISONS; iter++)
			{
				peCont = (Control)poisonEffect;
				if (peCont.GetChildCount() > 0)
				{
					pe = (Control)poisonEffect.GetChild(0);
					pe.Free();
				}
			}
		}

		player p = (player)pl;
		p.poisonSpeed = 1;

		foreach (var poi in poisonNodes)
		{
			poi.QueueFree();
		}
		poisonNodes.Clear();
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

		// For testing
		if (Input.IsKeyPressed(Key.U)) // energy
		{
			Globals.ShowUpgrades();
			
			PackedScene upgradeScene = (PackedScene)ResourceLoader.Load("res://Scenes/upgrade_gui.tscn");
			var upgrade1 = upgradeScene.Instantiate();
			//GUINode.AddChild(upgrade1);
			Upgrade ug = (Upgrade)upgrade1;
			upgrade1.QueueFree();
			ug.RandomizeUpgrade();
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

			xDmg = xDmg - GD.RandRange(0, armorLevel);

			if (xDmg > 0)
			{
				HP -= xDmg;
				ShowPlayerDamage();

				if (HP < 0 && playerAlive)
				{
					//Debug.Print("dmg");
					HP = 0;
					playerAlive = false;
					player p = (player)pl;
					p.PlayDeathAnim();
				}

				hpBar.Value = HP / MaxHP;
			}
		}
	}

	static public void HealPlayer(float amount)
	{

		HP += amount;
		ShowPlayerHeal();

		if (HP > MaxHP)
		{
			HP = MaxHP;
		}

		hpBar.Value = HP / MaxHP;
	}
	static async void ShowPlayerHeal()
	{
		ps.animatedSprite2D.Modulate = new Color(0, 0, 1, 1);
		// wait a bit
		await Task.Delay(TimeSpan.FromMilliseconds(100));
		ps.animatedSprite2D.Modulate = new Color(1, 1, 1, 1);
	}

	static async void ShowPlayerDamage()
	{
		ps.animatedSprite2D.Modulate = new Color(1, 0, 0, 1);
		// wait a bit
		await Task.Delay(TimeSpan.FromMilliseconds(100));
		ps.animatedSprite2D.Modulate = new Color(1, 1, 1, 1);
	}
	static private void CheckNextLevel()
	{
		if (XP >= XPGoal)
		{
			XP = (XP - XPGoal);
			level++;
			XPGoal = XPGoal * XPGoalIncrease;

			UpdateLevel();
			ShowUpgrades();
		}
	}

	public static void UpdateLevel()
	{
		lblLevel.Text = "LVL: " + level.ToString();
	}

	static public void ShowUpgrades()
	{
		// show Upgrades
		PackedScene upgradeScene = (PackedScene)ResourceLoader.Load("res://Scenes/upgrade_gui.tscn");

		var upgrade1 = upgradeScene.Instantiate();
		GUINode.AddChild(upgrade1);
		CanvasLayer nUpgrade1 = (CanvasLayer)upgrade1;
		nUpgrade1.Offset = new Vector2(60, 90);
		// get name of upgrade1 to prevent duplicates
		Upgrade ug = (Upgrade)upgrade1;
		string UGname1 = ug.GetName();
		Debug.Print("up1:" + UGname1);

		var upgrade2 = upgradeScene.Instantiate();
		GUINode.AddChild(upgrade2);
		CanvasLayer nUpgrade2 = (CanvasLayer)upgrade2;
		nUpgrade2.Offset = new Vector2(700, 90);
		// get name of upgrade2 to prevent duplicates
		ug = (Upgrade)upgrade2;
		string UGname2 = ug.GetName();
		// repeat until name doesn't match upgrade1
		while (UGname2 == UGname1)
		{
			ug.RandomizeUpgrade();
			UGname2 = ug.GetName();
		}

		Debug.Print("up1:" + UGname1+" up2:"+UGname2);


		var upgrade3 = upgradeScene.Instantiate();
		GUINode.AddChild(upgrade3);
		CanvasLayer nUpgrade3 = (CanvasLayer)upgrade3;
		nUpgrade3.Offset = new Vector2(1330, 90);
		// get name of upgrade2 to prevent duplicates
		ug = (Upgrade)upgrade3;
		string UGname3 = ug.GetName();
		// repeat until name doesn't match upgrade1
		while (UGname3 == UGname1 || UGname3 == UGname2)
		{
			ug.RandomizeUpgrade();
			UGname3 = ug.GetName();
		}

		Debug.Print("up1:" + UGname1 + " up2:" + UGname2 + " up3:"+ UGname3);

		// pause game
		rootNode.GetTree().Paused = true;
	}

	static public void UnPauseGame()
	{
		// unpause game
		rootNode.GetTree().Paused = false;
	}

    public static void UpdateStatsGUI()
    {
		lblMaxHealth.Text = HPLevel.ToString();
		lblMovementSpeed.Text = ps.speedLevel.ToString();
		lblMagnetism.Text = magenetismLevel.ToString();
        lblArmor.Text = armorLevel.ToString();
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
