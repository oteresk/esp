using Godot;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Xml.Linq;

public partial class Globals : Node
{
	static public string NodeMiniMap = "/root/World/MiniMapCanvas/Control2/Control/MarginContainer/ctlSub/SubViewportContainer/SubViewport/Node2D";
	static public string NodeMiniMapContainer = "/root/World/MiniMapCanvas/Control2/Control/MarginContainer/ctlSub/SubViewportContainer/SubViewport/Node2D/MiniMap";
    static public string NodeMiniMapCanvas = "/root/World/MiniMapCanvas";
    static public string NodeMiniMapPlayer = "/root/World/MiniMapCanvas/Control2/Control/MarginContainer/ctlSub/SubViewportContainer/SubViewport/Node2D/PlayerIcon/TextureRect";
	static public string NodeMiniMapBorder = "/root/World/MiniMapCanvas/Control2/Control/MarginContainer/ctlBorder/Border";
	static public string NodeStructureGUI = "/root/World/LateGUI/ctlStructureSelect";
    static public string NodeStructureGUIMessage = "/root/World/LateGUI/ctlResourceMessage";
    static public string NodeGUIWinMessage = "/root/World/LateGUI/ctlWinMessage";
    static public string NodeGUI = "/root/World/GUI";
	static public string NodeResourceDiscoveries = "/root/World/ResourceDiscoveries";
	static public string NodePlayer = "/root/World/Player";
	static public string NodeWorld = "/root/World";
	static public string NodeStructures = "/root/World/Structures";
	static public string NodeItems = "/root/World/Items";
	static public string NodeEnemies = "/root/World/Enemies";
	static public string NodePoison = "/root/World/GUI/Control/MarginContainer/MarginContainer/PoisonGUINode";
	static public string NodeGlobals = "/root/World/Globals";
    static public string NodeFPS = "/root/World/FPS";
	static public string NodeBack = "/root/World/CLbtnBack/ctlBack/TextureButton";


    static public Node rootNode;

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

    static public int speedLevel = 1;

    static public Label lblLevel;
	static public int level = 1;
	static public float XPGoalIncrease = 1.5f;
	static public float magnetism;
	static public int magenetismLevel;
	static public float magenetismInc = 10;

	static public int armorLevel;
	static public int attackLevel;
	static public int towerLevel;
	static public int golemLevel;

	static public bool playerAlive = true;
	static public bool playerShieldActive = false;

	static public Area2D pl;
	static public player ps;

	static public int enemies = 0;

	static public Node2D golem;
	static public bool golemAlive = false;
	static public Node2D agroGolem;
	static public bool agroGolemAlive = false;

	static public HBoxContainer poisonEffect;
	static public bool isPoisoned = false;
	static public int poisonCount;
	static public List<Node> poisonNodes;

	static public float itemAtkSpd; // 1 (default) .5 (with item)
	static public float statAtkSpd = 0;
	static public float statDamage = 1;
	static public float statAoE=1;
	static public float statMovementSpeed = 0;
	static public int statArmor = 0;

    static public resourceGUI GUINode;

	// research upgrade stats
	static public Label lblMaxHealth;
	static public Label lblMovementSpeed;
	static public Label lblMagnetism;
	static public Label lblArmor;
	static public Label lblAttack;
	static public Label lblTowerLevel;
	static public Label lblGolemLevel;

	static public Sprite2D black;
	//    GDScript saveStateSystem;
	//    GodotObject nodSaveStateSystem;

	static public int potionFreq = 26; // how often enemy drops a potion instead of gem

	const int MAXPOISONS = 3;

	static public bool[] weaponTypeUnlocked;

    static public int[] costIron;
    static public int[] costWood;

    // starting structures
    static public bool StartingTower=true;
    static public bool StartingPlatform = true;

	static public int platformManaCost = 5;

	// stat upgrades
	static public int[,] coststatUpgrade;

	static public int MAXUPGRADES = 5;
    static public int[] statUpgradeLevel;

	static public int MAXRELICS = 7;

    static public bool[] hasRelic;

	static public float settings_SFXVolume = 0;
    static public float settings_MusicVolume = 0;
    static public float settings_MasterVolume = 0;

	static public bool settings_ShowFPS = false;
	static public bool settings_GodMode = false;
    static public bool settingsLoaded = false;
	static public bool settings_ShowPlayerPosition = false;

    static public float healingModifier = .5f;
    static public float fireTime = 6; // how long the big fire lasts in world
    static public float flameTime=2.5f; // how long the flame on enemy lasts

	static public int screenWidth;
    static public int screenHeight;

	static public TextureButton btnBack;

	static public Globals instance;

    static public PackedScene TitleScene;
    static public PackedScene StatUpgradesScene;
    static public PackedScene OptionsScene;
    static public PackedScene WorldScene;

	static public bool canUnPause = true;

	[Export] public AudioStreamPlayer musInGame;

    public override void _Ready()
	{
		instance = this;
        btnBack = (TextureButton)GetNodeOrNull(NodeBack);

        weaponTypeUnlocked = new bool[12];

		worldArray = new int[gridSizeX * subGridSizeX, gridSizeY * subGridSizeY];
		// windowSizeY
		// 1071 - without header
		// 1009 - with
		windowSizeY = (int)GetViewport().GetVisibleRect().Size.Y;
		headerOffset = windowSizeY - 1009;

        // stat upgrade cost
        coststatUpgrade = new int[5,5] { { 2, 6, 18, 54, 162 }, { 2, 6, 18, 54, 162 }, { 2, 6, 18, 54, 162 }, { 2, 6, 18, 54, 162 }, { 2, 6, 18, 54, 162 } };

        statUpgradeLevel = new int[MAXUPGRADES];
		for (int iter=0;iter<5;iter++)
			statUpgradeLevel[iter] = 0;

        hasRelic= new bool[MAXRELICS];
;
        for (int iter = 0; iter < MAXRELICS; iter++)
            hasRelic[iter] = false;

        DelayedStart();

		rootNode = GetNode("..");
        // "GUI/Control/MarginContainer/StatsGUI/HBoxContainer/MaxHealth/VBoxContainer/lblMaxHealth"
        // get stat labels     GUI/Control/MarginContainer/StatsGUI/HBoxContainer/MovementSpeed/VBoxContainer/lblMovementSpeed

        Node lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer/MaxHealth/VBoxContainer/lblMaxHealth");
		lblMaxHealth = (Label)lbl;
		lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer/MovementSpeed/VBoxContainer/lblMovementSpeed");
		lblMovementSpeed = (Label)lbl;
		lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer/Magnetism/VBoxContainer/lblMagnetism");
		lblMagnetism = (Label)lbl;
		lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer/Armor/VBoxContainer/lblArmor");
		lblArmor = (Label)lbl;
		lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer2/Attack/VBoxContainer/lblAttack");
        lblAttack = (Label)lbl;
		lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer2/Towers/VBoxContainer/lblTowerLevel");
		lblTowerLevel = (Label)lbl;
		lbl = GetNodeOrNull("../GUI/ctlStatus/StatusBG/Area2D/StatsGUI/HBoxContainer2/Golems/VBoxContainer/lblGolemLevel");
        lblGolemLevel = (Label)lbl;

		costIron = new int[8];
        costWood = new int[8];

        screenWidth = (int)GetViewport().GetVisibleRect().Size.X;
        screenHeight = (int)GetViewport().GetVisibleRect().Size.Y;

        StatUpgradesScene = (PackedScene)ResourceLoader.Load("res://Scenes/StatUpgrades.tscn");
        OptionsScene = (PackedScene)ResourceLoader.Load("res://Scenes/Options.tscn");
        WorldScene = (PackedScene)ResourceLoader.Load("res://Scenes/world.tscn");
        TitleScene = (PackedScene)ResourceLoader.Load("res://Scenes/Title.tscn");

        ResetGame();
    }

    private void Input_JoyConnectionChanged(long device, bool connected)
    {
        if (connected)
            Debug.Print("Gamepad connected: " + device);
        else
            Debug.Print("Gamepad disconnected: " + device);

        //throw new NotImplementedException();
    }



    static public void UpdateStatLevels()
	{
        // stat attack speed   0-.625
        statAtkSpd = (float)statUpgradeLevel[0]/8.0f;

		// statDamage   1-26
        statDamage = 1.0f+(float)statUpgradeLevel[1]* statUpgradeLevel[1];
        
		// statAoE   
		statAoE= (float)statUpgradeLevel[2]/1.5f;
		if (ps!=null)
			ps.SetAllAoE(); // update all existing attack AoE

		// statMovementSpeed
        statMovementSpeed = (float)statUpgradeLevel[3];

        // statArmor
        statArmor = (int)statUpgradeLevel[4]* statUpgradeLevel[4];
    }

	public async void DelayedStart()
	{
		// wait a bit
		await Task.Delay(TimeSpan.FromMilliseconds(40));

		//Debug.Print("***********");
        SaveLoad.LoadGame();
        SaveLoad.LoadSettings();
        SetVolumes();

        if (lblAttack != null)
		{
			ResourceDiscoveries.UpdateResourceGUI();
			UpdateStatsGUI();
            if (Globals.lblAttack != null)
                Stats.UpdateStats();
		}

        // show map
        ResourceDiscoveries.mapNotPressed = false;
		if (IsInstanceValid(this))
		{
            Node2D miniMap = (Node2D)GetNodeOrNull(Globals.NodeMiniMap);
            if (miniMap != null)
            {
                miniMap.Call("ShowMiniMap");
            }
        }


		settingsLoaded= true;

        if (settings_ShowFPS)
        {
            CanvasLayer nFPS = (CanvasLayer)GetNodeOrNull(NodeFPS);
            if (nFPS != null)
                nFPS.Visible = true;
        }
        else
        {
            CanvasLayer nFPS = (CanvasLayer)GetNodeOrNull(NodeFPS);
            if (nFPS != null)
                nFPS.Visible = false;
        }

		// set god mode
		if (rootNode.Name == "World") // if world scene
		{ //only set god mode when in world scene
			if (settings_GodMode)
				ps.canBeDamaged = false;
			else
				ps.canBeDamaged = true;
		}

    }

	public void ResetGame()
	{
		GUINode = (resourceGUI)GetNodeOrNull(NodeGUI);

		playerAlive = true;
		level = 1;
		XPGoal = 20;
		xpBar = (ProgressBar)GetNodeOrNull("../GUI/XPOverlay/XPBar");
		lblLevel = (Label)GetNodeOrNull("../GUI/XPOverlay/XPBar/Label");
		if (xpBar != null)
		{
			xpBar.Value = XP;
			UpdateLevel();
		}
		itemAtkSpd = 1; // attackSpeed modifier for temp items

		if (lblLevel!=null)
			black = (Sprite2D)GetNodeOrNull("../Black");

		// hide poison effect
		poisonEffect = (HBoxContainer)GetNodeOrNull(NodePoison);

		poisonNodes = new List<Node>();

		Globals.XP = 0;

		SetMaxHP();
		Debug.Print("maxHP:" + MaxHP);
		Globals.HP = MaxHP;
        hpBar = (ProgressBar)GetNodeOrNull("../Player/HPBar");
        if (hpBar != null)
            hpBar.Value = HP / MaxHP;

		XPGoalIncrease = 1.5f;
		magnetism = 30;

		magenetismLevel = 1;
		armorLevel = 1;
		attackLevel = 2; // for testing
		towerLevel = 1;
		golemLevel = 1;

		ResourceDiscoveries.seconds = 0;
		ResourceDiscoveries.minutes = 0;

		ResourceDiscoveries.iron = 0;
		ResourceDiscoveries.mana = 0;
		ResourceDiscoveries.wood = 0;
		ResourceDiscoveries.gold = 0;
		ResourceDiscoveries.goldResourceCount = 0;
		ResourceDiscoveries.ironResourceCount = 0;
		ResourceDiscoveries.manaResourceCount = 0;
		ResourceDiscoveries.research = 0;


		Node2D fG = (Node2D)GetNodeOrNull("../FriendlyGolem");
		if (fG != null)
		{
			Globals.golem = fG;
			golemAlive = true;

			Debug.Print("Golem not null");
		}

		Node2D aG = (Node2D)GetNodeOrNull("../AgroGolem");
		if (aG != null)
		{
			Globals.agroGolem = aG;
			agroGolemAlive = true;

			Debug.Print("AgroGolem not null");
		}

		SetMagnetism();
		if (ps != null)
			ps.SetMagnetismShape();

		// set weapon unlocks
		weaponTypeUnlocked[0] = true; // slash
		weaponTypeUnlocked[1] = true; // projectile
		weaponTypeUnlocked[2] = false; // cross
        weaponTypeUnlocked[3] = true; // orbit
                                       // elements
        weaponTypeUnlocked[11] = false; // fire

        costIron[0] = 1; // alchemy lab
        costIron[1] = 2; // blacksmith
        costIron[3] = 1; // herbalist
        costIron[4] = 0; // lodestone
        costIron[5] = 4; // settlement
        costIron[6] = 4; // tower
        costIron[7] = 1; // training center
        costIron[2] = 1; // golem factory

        costWood[0] = 1;
        costWood[1] = 2;
        costWood[3] = 0;
        costWood[4] = 1;
        costWood[5] = 2;
        costWood[6] = 6;
        costWood[7] = 0;
        costWood[2] = 0;

        for (int iter = 0; iter < 5; iter++)
            statUpgradeLevel[iter] = 0;

        for (int iter = 0; iter < MAXRELICS; iter++)
            hasRelic[iter] = false;

        UpdateStatLevels();
    }

	static public void SetMaxHP()
	{
		MaxHP = 90 + HPInc * HPLevel * HPLevel;
        if (hpBar != null)
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
		if (poisonCount < MAXPOISONS && playerShieldActive == false && ps.canBeDamaged && playerAlive)
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
			Debug.Print("Save settings");
			SaveLoad.SaveSettings();
			//SaveGameState();
		}

		if (Input.IsActionJustReleased("load"))
		{
			Debug.Print("Load settings");
			SaveLoad.LoadSettings();
			//LoadGameState();
		}
            // For testing
        if (Input.IsKeyPressed(Key.U)) // Upgrade test
		{
			Globals.ShowUpgrades();

			PackedScene upgradeScene = (PackedScene)ResourceLoader.Load("res://Scenes/upgrade_gui.tscn");
			var upgrade1 = upgradeScene.Instantiate();
			//GUINode.AddChild(upgrade1);
			Upgrade ug = (Upgrade)upgrade1;
			upgrade1.QueueFree();
			ug.RandomizeUpgrade();
		}
		if (Input.IsKeyPressed(Key.K)) // Kill player
		{
			//Globals.DamagePlayer(999999);
		}

            // reset game save
        if (Input.IsActionJustPressed("reset"))
		{
			ResetStats();
        }

	}

	public void ResetStats()
	{
        ResourceDiscoveries.gold = 0;
        for (int iter = 0; iter < 5; iter++)
            Globals.statUpgradeLevel[iter] = 0;
        SaveLoad.SaveGame();
        // Update slots
        Node nd = Globals.rootNode.GetNode(".");
        StatUpgrades su = (StatUpgrades)nd;
        su.UpdateAllSlots();

        // update GUI
        StatUpgrades.lblGold.Text = ResourceDiscoveries.gold.ToString();


        btnUpgrade.DisableButton();
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
		if (playerShieldActive == false && ps.canBeDamaged && playerAlive)
		{
			ps.PlayHurtSound();
			xDmg = xDmg - GD.RandRange(0, armorLevel+Globals.statArmor);

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
        ps.animatedSprite2DTop.Modulate = new Color(0, 0, 1, .5f);
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(100));
		ps.animatedSprite2D.Modulate = new Color(1, 1, 1, 1);
        ps.animatedSprite2DTop.Modulate = new Color(1, 1, 1, .5f);
    }

	static async void ShowPlayerDamage()
	{
		ps.animatedSprite2D.Modulate = new Color(1, 0, 0, 1);
        ps.animatedSprite2DTop.Modulate = new Color(1, 0, 0, .5f);
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(100));
		ps.animatedSprite2D.Modulate = new Color(1, 1, 1, 1);
        ps.animatedSprite2DTop.Modulate = new Color(1, 1, 1, .5f);
    }
	static private void CheckNextLevel()
	{
		if (XP >= XPGoal)
		{
			XP = (XP - XPGoal);
			level++;
			XPGoal = XPGoal * XPGoalIncrease;

			ps.GainLevel();
		}
	}

	public static void UpdateLevel()
	{
		lblLevel.Text = "Level: " + level.ToString();
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

		Debug.Print("up1:" + UGname1 + " up2:" + UGname2);

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

		Debug.Print("up1:" + UGname1 + " up2:" + UGname2 + " up3:" + UGname3);
		PauseGame();
	}

	static public void PauseGame()
	{
		// pause game
		rootNode.GetTree().Paused = true;
	}

	static public void UnPauseGame()
	{
		Debug.Print("Unpause game");
		// unpause game
		rootNode.GetTree().Paused = false;
	}

	public static void UpdateStatsGUI()
	{
        if (ps != null && lblMaxHealth!=null)
		{
            lblMaxHealth.Text = HPLevel.ToString();
            lblMovementSpeed.Text = speedLevel.ToString();
            lblMagnetism.Text = magenetismLevel.ToString();
            lblArmor.Text = armorLevel.ToString();
            lblAttack.Text = attackLevel.ToString();
            lblTowerLevel.Text = towerLevel.ToString();
            lblGolemLevel.Text = golemLevel.ToString();
        }

	}

	static public void PlayRandomizedSound(AudioStreamPlayer snd)
	{
		snd.VolumeDb = GD.RandRange(-6, 6);
		snd.PitchScale = (float)GD.RandRange(.79f, 1.21f);
		snd.Play();
	}

    static public void PlayRandomizedSound2D(AudioStreamPlayer2D snd)
    {
        snd.VolumeDb = GD.RandRange(-6, 6);
        snd.PitchScale = (float)GD.RandRange(.79f, 1.21f);
        snd.Play();
    }

    static public void UpdateEnemies()
	{
        GD.PrintRich("[color=blue]Enemies: [/color]" + enemies +" - "+ ResourceDiscoveries.minutes.ToString()+ ":"+ResourceDiscoveries.seconds.ToString()+" - FPS:"+ Engine.GetFramesPerSecond().ToString());


    }

	static public void SetAttackLevel()
	{
		if (attackLevel>1)
	        Globals.weaponTypeUnlocked[2] = true; // unlock cross
        if (attackLevel > 2)
            Globals.weaponTypeUnlocked[11] = true; // unlock fire
    }

	static public void SetTowerLevel()
	{
            ResourceDiscovery[] rD = GetStructures("Tower"); // get all towers
			// set level of tower
			foreach (ResourceDiscovery rd in rD)
			{
                rd.SetTowerLevel();
            }
	}

    // gets an array of ResourceDiscoveries that are in the Node Group "Tower"
    static private ResourceDiscovery[] GetStructures(string resourceName)
	{
        var mainLoop = Godot.Engine.GetMainLoop();
        var sceneTree = mainLoop as SceneTree;

        var rds = sceneTree.GetNodesInGroup(resourceName);
        ResourceDiscovery[] blockList = new ResourceDiscovery[rds.Count];
        for (int i = 0; i < rds.Count; i++)
        {
            blockList[i] = (ResourceDiscovery)rds[i];

        }

        return blockList;
    }

    // returns how many relics have not been found yet
	static public int GetRelicsNeeded()
	{
		int needed = 0;
		for (int i = 0;i<MAXRELICS;i++)
		{
			if (hasRelic[i] == false)
			{
				needed++;
				Debug.Print("Need: " + i);
			}
		}
		return needed;
	}

	public void SetVolumes()
	{
		Debug.Print("volume settings_MusicVolume:"+ settings_MusicVolume);
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"),settings_SFXVolume);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), settings_MusicVolume);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), settings_MasterVolume);
        Debug.Print("volume AudioServer.GetBusIndex(\"Music\"), settings_MusicVolume:" + AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Music")));
    }
}
