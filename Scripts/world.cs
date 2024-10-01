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

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void SpawnMob()
	{
        if (ResourceDiscoveries.GetMinutes() == 5 || ResourceDiscoveries.GetMinutes()==11 || ResourceDiscoveries.GetMinutes()==16) // night mode
        {
            if (Globals.nightMode == false)
            {
                Globals.instance.ToggleNight();
            }
            // pick random enemy
            randEnemy = GD.RandRange(ResourceDiscoveries.GetMinutes()-5, ResourceDiscoveries.GetMinutes()-1);
            string enString = enemyString[randEnemy];
            Debug.Print("Enemy: "+enString);
        }
        else
        {
            if (Globals.nightMode == true)
            {
                Globals.instance.ToggleNight();
            }


            if (ResourceDiscoveries.GetMinutes() < enemyString.Length)
            {
                string enString = enemyString[ResourceDiscoveries.GetMinutes()];

                if ((enString != "res://Scenes/AgroGolem.tscn" && enString != "res://Scenes/SmallGolem.tscn") || !Globals.agroGolemAlive) // only allow 1 agro golem
                {
                    //Debug.Print("enemy:" + enemyString[ResourceDiscoveries.GetMinutes()] + "agroAlive:" + Globals.agroGolemAlive);
                    PackedScene newMobScene = (PackedScene)ResourceLoader.Load(enString);
                    Node2D newMob = (Node2D)newMobScene.Instantiate();

                    EnemySpawnPath.ProgressRatio = (float)GD.Randf();
                    newMob.GlobalPosition = ((PathFollow2D)GetNode("Player/Path2D/EnemySpawnPath")).GlobalPosition;
                    AddChild(newMob);
                    if (enString == "res://Scenes/AgroGolem.tscn" || enString == "res://Scenes/SmallGolem.tscn")
                    {
                        Globals.agroGolemAlive = true;
                        Globals.agroGolem = newMob;
                        //Debug.Print("AG=true");

                        // create minimap golem
                        Node miniMap = GetNode(Globals.NodeMiniMap);
                        MiniMap mm = (MiniMap)miniMap;
                        mm.CreateGolemIcons();
                        //Debug.Print("AgroGolemAlive:" + Globals.agroGolemAlive);
                        mm.DisplayAgroGolem();
                    }
                }
                // enemy 2
                if (ResourceDiscoveries.GetMinutes() > 0 && ResourceDiscoveries.GetMinutes()!=6 && ResourceDiscoveries.GetMinutes()!=12 && ResourceDiscoveries.GetMinutes()!=17)
                {
                    string en2String = enemyString[ResourceDiscoveries.GetMinutes() - 1];
                    //Debug.Print("enemy:"+ enemyString[ResourceDiscoveries.GetMinutes()] + "agroAlive:" + Globals.agroGolemAlive);
                    if (en2String != "res://Scenes/AgroGolem.tscn" && enString != "res://Scenes/SmallGolem.tscn") // don't creat golem as second enemy
                    {
                        PackedScene newMobScene = (PackedScene)ResourceLoader.Load(en2String);
                        Node2D newMob = (Node2D)newMobScene.Instantiate();

                        EnemySpawnPath.ProgressRatio = (float)GD.Randf();
                        newMob.GlobalPosition = ((PathFollow2D)GetNode("Player/Path2D/EnemySpawnPath")).GlobalPosition;
                        AddChild(newMob);
                    }
                }


            }
        }

	}

	// this is just a rudimentary way of spawning enemies; a more detailed wave-like spawning system will replace this
	public void _on_enemy_timer_timeout()
	{
		//Debug.Print("spawning mob");
		SpawnMob();
	}

}
