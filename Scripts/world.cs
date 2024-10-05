using Godot;
using System;
using System.Diagnostics;

public partial class world : Node2D
{
	[Export] public PathFollow2D EnemySpawnPath;
    string[] enemyString;
    private int randEnemy;

    public override void _Ready()
	{
        Globals.instance.WorldReady();
        ResourceDiscoveries.instance.WorldReady();


        enemyString = new string[]
        {
            "res://Scenes/Enemies/en_Bat.tscn",
            "res://Scenes/Enemies/en_Slime.tscn",
            "res://Scenes/Enemies/en_Lizard.tscn",
            "res://Scenes/Enemies/en_Spider.tscn",
            "res://Scenes/Enemies/en_PoisonSlime.tscn",
            "res://Scenes/Enemies/en_Bat2.tscn",
            "res://Scenes/Enemies/en_FireSlime.tscn",
            "res://Scenes/Enemies/en_Scorp.tscn",
            "res://Scenes/Enemies/en_Skeleton.tscn",
            "res://Scenes/SmallGolem.tscn",
            "res://Scenes/Enemies/en_Wolf.tscn",
            "res://Scenes/Enemies/en_FloatingSkull.tscn",
            "res://Scenes/Enemies/en_EvilEye.tscn",
            "res://Scenes/AgroGolem.tscn",
        };

        // initialize stuff

        Node rdNode = Globals.rootNode.GetNode(Globals.NodeResourceDiscoveries);
        ResourceDiscoveries rD = (ResourceDiscoveries)rdNode;
        rD.PlaceStartingStructures();
        rD.PlaceResourceDiscoveries();
        if (Globals.settings_Decorations)
            rD.PlaceDecorations();


        // full screen
        if (Globals.settings_FullScreen)
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        else
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);

    }

	public void SpawnMob()
	{
        int minutes = ResourceDiscoveries.minutes;
        string enString = "";

        switch (minutes)
        {
            case 0: // bat
                {
                    enString = enemyString[0];
                    break;
                }
            case 1: // slime
            {
                    enString = enemyString[1];
                    break;
            }
            case 2: // lizard
            {
                enString = enemyString[2];
                break;
            }
            case 3: // spider
            {
                enString = enemyString[3];
                break;
            }
            case 4: // poison slime
            {
                enString = enemyString[4];
                break;
            }
            case 5: // Bat, slime, lizard, spider, poison slime
            {
                if (Globals.nightMode==false)
                    Globals.instance.ToggleNight();

                // pick random enemy
                randEnemy = GD.RandRange(0,4);
                enString = enemyString[randEnemy]; 
                break;
            }
            case 6: // fire bat
            {
                    if (Globals.nightMode == true)
                        Globals.instance.ToggleNight();
                    enString = enemyString[5];
                break;
            }
            case 7: // scorp
            {
                enString = enemyString[6];
                break;
            }
            case 8: // skele
            {
                enString = enemyString[7];
                break;
            }
            case 9: // Bat, slime, lizard, spider, poison slime
            {
                    if (Globals.nightMode == false)
                        Globals.instance.ToggleNight();

                // pick random enemy
                    randEnemy = GD.RandRange(5, 7);
                enString = enemyString[randEnemy];
                break;
            }
            case 10: // small golem
            {
                    if (Globals.nightMode == true)
                        Globals.instance.ToggleNight();

                    if (!Globals.agroGolemAlive)
                {
                    Globals.agroGolemAlive = true;
                    //Debug.Print("AG=true");

                    // create minimap golem
                    Node miniMap = GetNode(Globals.NodeMiniMap);
                    MiniMap mm = (MiniMap)miniMap;
                    mm.CreateGolemIcons();
                    //Debug.Print("AgroGolemAlive:" + Globals.agroGolemAlive);
                    mm.DisplayAgroGolem();
                }
                break;
            }
            case 11: // wolf
                {
                    enString = enemyString[10];
                    break;
                }
            case 12: // floating skull
                {
                    enString = enemyString[11];
                    break;
                }
            case 13: // evil eye
                {
                    enString = enemyString[12];
                    break;
                }
            case 14: // Wolf, floating skull, evil eye
                {
                    if (Globals.nightMode == false)
                        Globals.instance.ToggleNight();

                    // pick random enemy
                    randEnemy = GD.RandRange(10, 12);
                    enString = enemyString[randEnemy];
                    break;
                }
            case 15: // agro golem
                {
                    if (Globals.nightMode == true)
                        Globals.instance.ToggleNight();

                    if (!Globals.agroGolemAlive)
                    {
                        Globals.agroGolemAlive = true;
                        //Debug.Print("AG=true");

                        // create minimap golem
                        Node miniMap = GetNode(Globals.NodeMiniMap);
                        MiniMap mm = (MiniMap)miniMap;
                        mm.CreateGolemIcons();
                        //Debug.Print("AgroGolemAlive:" + Globals.agroGolemAlive);
                        mm.DisplayAgroGolem();
                    }
                    break;
                }
        }

        PackedScene newMobScene = (PackedScene)ResourceLoader.Load(enString);
        Node2D newMob = (Node2D)newMobScene.Instantiate();
        EnemySpawnPath.ProgressRatio = (float)GD.Randf();
        newMob.GlobalPosition = ((PathFollow2D)GetNode("Player/Path2D/EnemySpawnPath")).GlobalPosition;
        AddChild(newMob);
        Debug.Print("enemy:" + enemyString[minutes]);


        //EnemySpawnPath.ProgressRatio = (float)GD.Randf();

        if (enemyString[minutes]== "res://Scenes/SmallGolem.tscn")
        {
            Globals.agroGolem = newMob;
        }


        /*
        // enemy 2
        if (minutes>0 && minutes!=5 && minutes!=9 && minutes!= 14)
        {
            string en2String = enemyString[minutes];
            //Debug.Print("enemy:"+ enemyString[ResourceDiscoveries.GetMinutes()] + "agroAlive:" + Globals.agroGolemAlive);
            if (en2String != "res://Scenes/AgroGolem.tscn" && enString != "res://Scenes/SmallGolem.tscn") // don't creat golem as second enemy
            {
                PackedScene newMobScene = (PackedScene)ResourceLoader.Load(en2String);
                Nosde2D newMob = (Node2D)newMobScene.Instantiate();

                EnemySpawnPath.ProgressRatio = (float)GD.Randf();
                newMob.GlobalPosition = ((PathFollow2D)GetNode("Player/Path2D/EnemySpawnPath")).GlobalPosition;
                AddChild(newMob);
            }
        }
        */

    }


	public void _on_enemy_timer_timeout()
	{
		//Debug.Print("spawning mob");
		SpawnMob();
	}

}
