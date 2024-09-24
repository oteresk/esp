using Godot;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public partial class SaveLoad : Node2D
{
	static private Godot.Collections.Dictionary settingsData;
	static private string settingsFilename = "Settings.json";

    static private Godot.Collections.Dictionary gameSaveData;
    static private string gameSaveFilename = "GameSave.dat";

    public override void _Ready()
	{
        string path = ProjectSettings.GlobalizePath(settingsFilename);

        if (!File.Exists(path))
            SaveSettings();

        path = ProjectSettings.GlobalizePath(gameSaveFilename);

        if (!File.Exists(path))
            SaveGameNewFile();
    }


    // clears the dictionary
    static public void ClearSettings()
    {
        settingsData.Clear();
    }


    static public void SaveSettings()
	{
        Debug.Print("Save settings");
        // resource starting numbers (used for testing)
        settingsData.Clear();

        settingsData.Add("[ResourceStart] StartIron", ResourceDiscoveries.iron);
        settingsData.Add("[ResourceStart] StartMana", ResourceDiscoveries.mana);
        settingsData.Add("[ResourceStart] StartWood", ResourceDiscoveries.wood);
        settingsData.Add("[ResourceStart] StartResearch", ResourceDiscoveries.research);

        // Structure Costs
        settingsData.Add("[StructureCosts] AlchemyLabIron", Globals.costIron[0]);
        settingsData.Add("[StructureCosts] AlchemyLabWood", Globals.costWood[0]);
        settingsData.Add("[StructureCosts] BlacksmithIron", Globals.costIron[1]);
        settingsData.Add("[StructureCosts] BlacksmithWood", Globals.costWood[1]);
        settingsData.Add("[StructureCosts] HerbalistIron", Globals.costIron[3]);
        settingsData.Add("[StructureCosts] HerbalistWood", Globals.costWood[3]);
        settingsData.Add("[StructureCosts] LodestoneIron", Globals.costIron[4]);
        settingsData.Add("[StructureCosts] LodestoneWood", Globals.costWood[4]);
        settingsData.Add("[StructureCosts] SettlementIron", Globals.costIron[5]);
        settingsData.Add("[StructureCosts] SettlementWood", Globals.costWood[5]);
        settingsData.Add("[StructureCosts] TowerIron", Globals.costIron[6]);
        settingsData.Add("[StructureCosts] TowerWood", Globals.costWood[6]);
        settingsData.Add("[StructureCosts] TrainingCenterIron", Globals.costIron[7]);
        settingsData.Add("[StructureCosts] TrainingCenterWood", Globals.costWood[7]);
        settingsData.Add("[StructureCosts] GolemFactoryIron", Globals.costIron[2]);
        settingsData.Add("[StructureCosts] GolemFactoryWood", Globals.costWood[2]);

        // starting structures
        settingsData.Add("[StartingStructures] Tower", Globals.StartingTower);
        settingsData.Add("[StartingStructures] Platform", Globals.StartingPlatform);

        settingsData.Add("[StructureCosts] Platform", Globals.platformManaCost);

        // stat upgrade costs
        settingsData.Add("[StatUpgradeCosts] AttackSpeed1", Globals.coststatUpgrade[0,0]);
        settingsData.Add("[StatUpgradeCosts] AttackSpeed2", Globals.coststatUpgrade[0, 1]);
        settingsData.Add("[StatUpgradeCosts] AttackSpeed3", Globals.coststatUpgrade[0, 2]);
        settingsData.Add("[StatUpgradeCosts] AttackSpeed4", Globals.coststatUpgrade[0, 3]);
        settingsData.Add("[StatUpgradeCosts] AttackSpeed5", Globals.coststatUpgrade[0, 4]);

        settingsData.Add("[StatUpgradeCosts] Damage1", Globals.coststatUpgrade[1,0]);
        settingsData.Add("[StatUpgradeCosts] Damage2", Globals.coststatUpgrade[1,1]);
        settingsData.Add("[StatUpgradeCosts] Damage3", Globals.coststatUpgrade[1,2]);
        settingsData.Add("[StatUpgradeCosts] Damage4", Globals.coststatUpgrade[1,3]);
        settingsData.Add("[StatUpgradeCosts] Damage5", Globals.coststatUpgrade[1,4]);

        settingsData.Add("[StatUpgradeCosts] AoE1", Globals.coststatUpgrade[2, 0]);
        settingsData.Add("[StatUpgradeCosts] AoE2", Globals.coststatUpgrade[2, 1]);
        settingsData.Add("[StatUpgradeCosts] AoE3", Globals.coststatUpgrade[2, 2]);
        settingsData.Add("[StatUpgradeCosts] AoE4", Globals.coststatUpgrade[2, 3]);
        settingsData.Add("[StatUpgradeCosts] AoE5", Globals.coststatUpgrade[2, 4]);

        settingsData.Add("[StatUpgradeCosts] MovementSpeed1", Globals.coststatUpgrade[3, 0]);
        settingsData.Add("[StatUpgradeCosts] MovementSpeed2", Globals.coststatUpgrade[3, 1]);
        settingsData.Add("[StatUpgradeCosts] MovementSpeed3", Globals.coststatUpgrade[3, 2]);
        settingsData.Add("[StatUpgradeCosts] MovementSpeed4", Globals.coststatUpgrade[3, 3]);
        settingsData.Add("[StatUpgradeCosts] MovementSpeed5", Globals.coststatUpgrade[3, 4]);

        settingsData.Add("[StatUpgradeCosts] Armor1", Globals.coststatUpgrade[4, 0]);
        settingsData.Add("[StatUpgradeCosts] Armor2", Globals.coststatUpgrade[4, 1]);
        settingsData.Add("[StatUpgradeCosts] Armor3", Globals.coststatUpgrade[4, 2]);
        settingsData.Add("[StatUpgradeCosts] Armor4", Globals.coststatUpgrade[4, 3]);
        settingsData.Add("[StatUpgradeCosts] Armor5", Globals.coststatUpgrade[4, 4]);

        // settings menu settings
        settingsData.Add("[Settings] SFXVolume", Globals.settings_SFXVolume);
        settingsData.Add("[Settings] MusicVolume", Globals.settings_MusicVolume);
        settingsData.Add("[Settings] MasterVolume", Globals.settings_MasterVolume);

        settingsData.Add("[Settings] ShowFullScreen", Globals.settings_FullScreen);
        settingsData.Add("[Settings] ShowFPS", Globals.settings_ShowFPS);

        settingsData.Add("[Settings] GodMode", Globals.settings_GodMode);
        settingsData.Add("[Settings] ShowPlayerPosition", Globals.settings_ShowPlayerPosition);

        settingsData.Add("[Settings] Decorations", Globals.settings_Decorations);
        settingsData.Add("[Settings] PlayerGhost", Globals.settings_PlayerGhost);

        string json = Json.Stringify(settingsData, "\t");
        settingsData.Clear();

        string path = ProjectSettings.GlobalizePath(settingsFilename);
        File.WriteAllText(path, json);
	}

    static public void LoadSettings()
	{
        settingsData = new Godot.Collections.Dictionary();
        settingsData.Clear();

        if (File.Exists(settingsFilename))
        {

            string laodedString = File.ReadAllText(settingsFilename);
            Json jsonload = new Json();

            jsonload.Parse(laodedString);

            settingsData = (Godot.Collections.Dictionary)jsonload.Data;

            // resource starting numbers (used for testing)
            ResourceDiscoveries.iron = (float)settingsData["[ResourceStart] StartIron"];
            ResourceDiscoveries.mana = (float)settingsData["[ResourceStart] StartMana"];
            ResourceDiscoveries.wood = (float)settingsData["[ResourceStart] StartWood"];
            ResourceDiscoveries.research = (int)settingsData["[ResourceStart] StartResearch"];
            // update GUI
            ResourceDiscoveries.UpdateResourceGUI();

            // Structure Costs
            Globals.costIron[0] = (int)settingsData["[StructureCosts] AlchemyLabIron"];
            Globals.costWood[0] = (int)settingsData["[StructureCosts] AlchemyLabWood"];
            Globals.costIron[1] = (int)settingsData["[StructureCosts] BlacksmithIron"];
            Globals.costWood[1] = (int)settingsData["[StructureCosts] BlacksmithWood"];
            Globals.costIron[3] = (int)settingsData["[StructureCosts] HerbalistIron"];
            Globals.costWood[3] = (int)settingsData["[StructureCosts] HerbalistWood"];
            Globals.costIron[4] = (int)settingsData["[StructureCosts] LodestoneIron"];
            Globals.costWood[4] = (int)settingsData["[StructureCosts] LodestoneWood"];
            Globals.costIron[5] = (int)settingsData["[StructureCosts] SettlementIron"];
            Globals.costWood[5] = (int)settingsData["[StructureCosts] SettlementWood"];
            Globals.costIron[6] = (int)settingsData["[StructureCosts] TowerIron"];
            Globals.costWood[6] = (int)settingsData["[StructureCosts] TowerWood"];
            Globals.costIron[7] = (int)settingsData["[StructureCosts] TrainingCenterIron"];
            Globals.costWood[7] = (int)settingsData["[StructureCosts] TrainingCenterWood"];
            Globals.costIron[2] = (int)settingsData["[StructureCosts] GolemFactoryIron"];
            Globals.costWood[2] = (int)settingsData["[StructureCosts] GolemFactoryWood"];

            // starting structures
            Globals.StartingTower = (bool)settingsData["[StartingStructures] Tower"];
            Globals.StartingPlatform = (bool)settingsData["[StartingStructures] Platform"];

            Globals.platformManaCost = (int)settingsData["[StructureCosts] Platform"];

            // stat upgrade costs
            Globals.coststatUpgrade[0, 0] = (int)settingsData["[StatUpgradeCosts] AttackSpeed1"];
            Globals.coststatUpgrade[0, 1] = (int)settingsData["[StatUpgradeCosts] AttackSpeed2"];
            Globals.coststatUpgrade[0, 2] = (int)settingsData["[StatUpgradeCosts] AttackSpeed3"];
            Globals.coststatUpgrade[0, 3] = (int)settingsData["[StatUpgradeCosts] AttackSpeed4"];
            Globals.coststatUpgrade[0, 4] = (int)settingsData["[StatUpgradeCosts] AttackSpeed5"];

            Globals.coststatUpgrade[1, 0] = (int)settingsData["[StatUpgradeCosts] Damage1"];
            Globals.coststatUpgrade[1, 1] = (int)settingsData["[StatUpgradeCosts] Damage2"];
            Globals.coststatUpgrade[1, 2] = (int)settingsData["[StatUpgradeCosts] Damage3"];
            Globals.coststatUpgrade[1, 3] = (int)settingsData["[StatUpgradeCosts] Damage4"];
            Globals.coststatUpgrade[1, 4] = (int)settingsData["[StatUpgradeCosts] Damage5"];

            Globals.coststatUpgrade[2, 0] = (int)settingsData["[StatUpgradeCosts] AoE1"];
            Globals.coststatUpgrade[2, 1] = (int)settingsData["[StatUpgradeCosts] AoE2"];
            Globals.coststatUpgrade[2, 2] = (int)settingsData["[StatUpgradeCosts] AoE3"];
            Globals.coststatUpgrade[2, 3] = (int)settingsData["[StatUpgradeCosts] AoE4"];
            Globals.coststatUpgrade[2, 4] = (int)settingsData["[StatUpgradeCosts] AoE5"];

            Globals.coststatUpgrade[3, 0] = (int)settingsData["[StatUpgradeCosts] MovementSpeed1"];
            Globals.coststatUpgrade[3, 1] = (int)settingsData["[StatUpgradeCosts] MovementSpeed2"];
            Globals.coststatUpgrade[3, 2] = (int)settingsData["[StatUpgradeCosts] MovementSpeed3"];
            Globals.coststatUpgrade[3, 3] = (int)settingsData["[StatUpgradeCosts] MovementSpeed4"];
            Globals.coststatUpgrade[3, 4] = (int)settingsData["[StatUpgradeCosts] MovementSpeed5"];

            Globals.coststatUpgrade[4, 0] = (int)settingsData["[StatUpgradeCosts] Armor1"];
            Globals.coststatUpgrade[4, 1] = (int)settingsData["[StatUpgradeCosts] Armor2"];
            Globals.coststatUpgrade[4, 2] = (int)settingsData["[StatUpgradeCosts] Armor3"];
            Globals.coststatUpgrade[4, 3] = (int)settingsData["[StatUpgradeCosts] Armor4"];
            Globals.coststatUpgrade[4, 4] = (int)settingsData["[StatUpgradeCosts] Armor5"];

            // settings menu settings
            Globals.settings_SFXVolume = (float)settingsData["[Settings] SFXVolume"];
            Globals.settings_MusicVolume = (float)settingsData["[Settings] MusicVolume"];
            Globals.settings_MasterVolume = (float)settingsData["[Settings] MasterVolume"];

            if (settingsData.ContainsKey("[Settings] FullScreen"))
                Globals.settings_FullScreen = (bool)settingsData["[Settings] FullScreen"];
            else
                settingsData.Add("[Settings] FullScreen", true);

            if (settingsData.ContainsKey("[Settings] ShowFPS"))
                Globals.settings_ShowFPS = (bool)settingsData["[Settings] ShowFPS"];
            else
                settingsData.Add("[Settings] ShowFPS", false);

            if (settingsData.ContainsKey("[Settings] GodMode"))
                Globals.settings_GodMode = (bool)settingsData["[Settings] GodMode"];
            else
                settingsData.Add("[Settings] GodMode", false);

            if (settingsData.ContainsKey("[Settings] ShowPlayerPosition"))
                Globals.settings_ShowPlayerPosition = (bool)settingsData["[Settings] ShowPlayerPosition"];
            else
                settingsData.Add("[Settings] ShowPlayerPosition", false);

            if (settingsData.ContainsKey("[Settings] Decorations"))
                Globals.settings_Decorations = (bool)settingsData["[Settings] Decorations"];
            else
                settingsData.Add("[Settings] Decorations", true);

            if (settingsData.ContainsKey("[Settings] PlayerGhost"))
                Globals.settings_PlayerGhost = (bool)settingsData["[Settings] PlayerGhost"];
            else
                settingsData.Add("[Settings] PlayerGhost", true);





            // initialize stuff

            Node rdNode = Globals.rootNode.GetNodeOrNull(Globals.NodeResourceDiscoveries);
            Debug.Print("Scene name: " + Globals.rootNode.Name);
            if (Globals.rootNode.Name == "World") // if world scene
            {
                ResourceDiscoveries rD = (ResourceDiscoveries)rdNode;
                rD.PlaceStartingStructures();
                rD.PlaceResourceDiscoveries();
                if (Globals.settings_Decorations)
                    rD.PlaceDecorations();
            }
            else // if title
            if (Globals.rootNode.Name == "Title")
            {

            }

            // full screen
            if (Globals.settings_FullScreen)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
            else
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);


            Debug.Print("Load settings");
        }

    }

    static public void SaveGameNewFile()
    {
        // init stat upgrades
        Globals.statUpgradeLevel = new int[Globals.MAXUPGRADES];
        for (int iter = 0; iter < 5; iter++)
            Globals.statUpgradeLevel[iter] = 0;

        // init relics
        Globals.hasRelic = new bool[Globals.MAXRELICS];

        for (int iter = 0; iter < Globals.MAXRELICS; iter++)
            Globals.hasRelic[iter] = false;

        SaveGame();
    }

        static public void SaveGame()
    {
        Debug.Print("save game:" + ResourceDiscoveries.gold);
        gameSaveData = new Godot.Collections.Dictionary();
        gameSaveData.Clear();

        gameSaveData.Add("StartGold", ResourceDiscoveries.gold);
        gameSaveData.Add("[StatUpgradeLevel] AttackSpeed", Globals.statUpgradeLevel[0]);
        gameSaveData.Add("[StatUpgradeLevel] Damage", Globals.statUpgradeLevel[1]);
        gameSaveData.Add("[StatUpgradeLevel] AoE", Globals.statUpgradeLevel[2]);
        gameSaveData.Add("[StatUpgradeLevel] MovementSpeed", Globals.statUpgradeLevel[3]);
        gameSaveData.Add("[StatUpgradeLevel] Armor", Globals.statUpgradeLevel[4]);

        // stats
        gameSaveData.Add("[Stats] HPLevel", Globals.HPLevel);
        gameSaveData.Add("[Stats] SpeedLevel", Globals.speedLevel);
        gameSaveData.Add("[Stats] MagnetismLevel", Globals.magenetismLevel);
        gameSaveData.Add("[Stats] ArmorLevel", Globals.armorLevel);
        gameSaveData.Add("[Stats] AttackLevel", Globals.attackLevel);
        gameSaveData.Add("[Stats] TowerLevel", Globals.towerLevel);
        gameSaveData.Add("[Stats] GolemLevel", Globals.golemLevel);

        // relics
        gameSaveData.Add("[Relic] Citrine Bracelet", Globals.hasRelic[0]);
        gameSaveData.Add("[Relic] Chalice of Suffering", Globals.hasRelic[1]);
        gameSaveData.Add("[Relic] Crown", Globals.hasRelic[2]);
        gameSaveData.Add("[Relic] Horn of Desolation", Globals.hasRelic[3]);
        gameSaveData.Add("[Relic] Sapphire Necklace", Globals.hasRelic[4]);
        gameSaveData.Add("[Relic] Topaz Necklace", Globals.hasRelic[5]);
        gameSaveData.Add("[Relic] Ruby Ring", Globals.hasRelic[6]);

        string json = Json.Stringify(gameSaveData, "\t");
        gameSaveData.Clear();

        string js2= UpdateFile(json, 94);
        //string js2 = json;

        string path = ProjectSettings.GlobalizePath(gameSaveFilename);
        File.WriteAllText(path, js2);

    }

    static public void LoadGame()
    {
        gameSaveData = new Godot.Collections.Dictionary();
        gameSaveData.Clear();

        if (File.Exists(gameSaveFilename))
        {
            string laodedString = File.ReadAllText(gameSaveFilename);

            //Debug.Print("L: " + laodedString);

            string js2 = UpdateFile(laodedString, -94);
            //string js2 = laodedString;

            //Debug.Print("L: " + js2);

            Json jsonload = new Json();

            try
            {
                jsonload.Parse(js2);
            }
            catch (Exception e) { GD.Print(e.ToString()); }


            gameSaveData = (Godot.Collections.Dictionary)jsonload.Data;

            // resource starting numbers (used for testing)
            if (gameSaveData.ContainsKey("StartGold"))
                ResourceDiscoveries.gold = (float)gameSaveData["StartGold"];

            // stats
            Globals.HPLevel = (int)gameSaveData["[Stats] HPLevel"];
            Globals.speedLevel = (int)gameSaveData["[Stats] SpeedLevel"];
            Globals.magenetismLevel = (int)gameSaveData["[Stats] MagnetismLevel"];
            Globals.armorLevel = (int)gameSaveData["[Stats] ArmorLevel"];
            Globals.attackLevel = (int)gameSaveData["[Stats] AttackLevel"];
            Globals.towerLevel = (int)gameSaveData["[Stats] TowerLevel"];
            Globals.golemLevel = (int)gameSaveData["[Stats] GolemLevel"];
            // update stats GUI
            Globals.UpdateStatsGUI();
            if (Globals.lblAttack != null)
                Stats.UpdateStats(); // set available stat upgrades
            Globals.SetAttackLevel();
            Globals.SetTowerLevel();

            // stat upgrades
            Globals.InitArrays();

            if (gameSaveData.ContainsKey("[StatUpgradeLevel] AttackSpeed"))
                Globals.statUpgradeLevel[0] = (int)gameSaveData["[StatUpgradeLevel] AttackSpeed"];
            if (gameSaveData.ContainsKey("[StatUpgradeLevel] Damage"))
                Globals.statUpgradeLevel[1] = (int)gameSaveData["[StatUpgradeLevel] Damage"];
            if (gameSaveData.ContainsKey("[StatUpgradeLevel] AoE"))
                Globals.statUpgradeLevel[2] = (int)gameSaveData["[StatUpgradeLevel] AoE"];
            if (gameSaveData.ContainsKey("[StatUpgradeLevel] MovementSpeed"))
                Globals.statUpgradeLevel[3] = (int)gameSaveData["[StatUpgradeLevel] MovementSpeed"];
            if (gameSaveData.ContainsKey("[StatUpgradeLevel] Armor"))
                Globals.statUpgradeLevel[4] = (int)gameSaveData["[StatUpgradeLevel] Armor"];

            //Debug.Print("Globals.statUpgradeLevel[0]:" + Globals.statUpgradeLevel[0]);

            // relics
            if (gameSaveData.ContainsKey("[Relic] Citrine Bracelet"))
                Globals.hasRelic[0] = (bool)gameSaveData["[Relic] Citrine Bracelet"];
            if (gameSaveData.ContainsKey("[Relic] Chalice of Suffering"))
                Globals.hasRelic[1] = (bool)gameSaveData["[Relic] Chalice of Suffering"];
            if (gameSaveData.ContainsKey("[Relic] Crown"))
                Globals.hasRelic[2] = (bool)gameSaveData["[Relic] Crown"];
            if (gameSaveData.ContainsKey("[Relic] Horn of Desolation"))
                Globals.hasRelic[3] = (bool)gameSaveData["[Relic] Horn of Desolation"];
            if (gameSaveData.ContainsKey("[Relic] Sapphire Necklace"))
                Globals.hasRelic[4] = (bool)gameSaveData["[Relic] Sapphire Necklace"];
            if (gameSaveData.ContainsKey("[Relic] Topaz Necklace"))
                Globals.hasRelic[5] = (bool)gameSaveData["[Relic] Topaz Necklace"];
            if (gameSaveData.ContainsKey("[Relic] Ruby Ring"))
                Globals.hasRelic[6] = (bool)gameSaveData["[Relic] Ruby Ring"];

            // testing
            Globals.hasRelic[1] = true;
            Globals.hasRelic[2] = true;
            //Globals.hasRelic[6] = true;
            Globals.hasRelic[5] = true;


            Debug.Print("Load Game Save");

            Globals.UpdateStatLevels();

            // update stat upgrades
            if (Globals.rootNode.Name == "StatUpgrades")
            {
                Debug.Print("SaveLoad Stat Upgrades");

                Debug.Print("Damage Upgrade: " + Globals.statUpgradeLevel[1]);
                Debug.Print("Damage Upgrade key: " + gameSaveData["[StatUpgradeLevel] Damage"]);

                Node nd = Globals.rootNode.GetNode(".");
                StatUpgrades su = (StatUpgrades)nd;
                su.UpdateAllSlots();
            }

            // update GUI
            ResourceDiscoveries.UpdateResourceGUI();
        }
    }

    private static string UpdateFile(string source, Int16 shift)
    {
        var maxChar = Convert.ToInt32(char.MaxValue);
        var minChar = Convert.ToInt32(char.MinValue);

        var buffer = source.ToCharArray();

        for (var i = 0; i < buffer.Length; i++)
        {
            var shifted = Convert.ToInt32(buffer[i]) + shift;

            if (shifted > maxChar)
            {
                shifted -= maxChar;
            }
            else if (shifted < minChar)
            {
                shifted += maxChar;
            }

            buffer[i] = Convert.ToChar(shifted);
        }
        return new string(buffer);
    }


}
