using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class AttackSlash : Area2D
{
    private player ps;
    public float damage;
    public float AOE;
    private int dmgLevel = 1;
    private float dmgBase = 1.5f;
    private float dmgInc = .3f;
    private int AOELevel = 1; // (1-12)
    private float AOEInc = .6f;
    private int attackSpeedLevel = 1; // max 12
    public float freezeTime = 3.0f;

    private PackedScene slashScene;
    private AnimatedSprite2D newSlash;

    public string element; // attack element (energy, fire, etc)

    // attack Speed
    private Timer bulletTimer;
    public float finalAtkSpd; // = base*IAS-(PAS/50)+rndOffset-(ASL/50)
    public float atkSpdRndOffset; // 0.0-.1
    public float baseAtkSpd = 1.4f; // can be different for each attack type (must be no less than 1)
    public float baseAOE = 1.2f;

    [Export] public AudioStreamPlayer sndSwipe;

    private List<enemy> enemies;
    private List<AgroGolem> aGolems;

    public override void _Ready()
    {
        bulletTimer = (Timer)GetNode("BulletTimer");
        enemies = new List<enemy>();
        aGolems = new List<AgroGolem>();
        atkSpdRndOffset = (float)GD.RandRange(0.0, 0.1);
        UpdateAttributes();
        DelayAtkSpdStart();
    }

    async void DelayAtkSpdStart()
    {
        //wait for random offset then set attackspeed
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        SetAttackSpeed();
    }

    // set attack speed
    public void SetAttackSpeed() // =base*IAS-(PAS/50)-(ASL/10)
    {
        finalAtkSpd = baseAtkSpd * Globals.itemAtkSpd - (attackSpeedLevel / 12f)-Globals.statAtkSpd;
        if (finalAtkSpd < .01f)
            finalAtkSpd = .01f;
        bulletTimer.WaitTime = finalAtkSpd;
        //Debug.Print("atkspd:" + finalAtkSpd+" base:"+ baseAtkSpd+" item:"+ Globals.itemAtkSpd+" perm:"+ Globals.permItemAtkSpd+" atkspdlvl:"+ (attackSpeedLevel / 12f));
    }

    public void SetAOE()
    {
        AOE = (baseAOE + AOELevel * AOEInc+(Globals.statAoE * AOEInc))/2;
        if (IsInstanceValid(this))
           Scale=new Vector2(AOE,AOE);
        //Debug.Print("SetAoE");
    }

    private void UpdateAttributes()
    {
        damage = dmgBase * dmgLevel * dmgLevel * dmgInc * Globals.statDamage;
        if (element=="leeches")
        {
            Globals.HealPlayer(damage * Globals.healingModifier);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void FlipAttack()
    {
        AnimatedSprite2D aSpr = (AnimatedSprite2D)GetNode("Slash");
        // set propper flip acording to player
        if (ps!=null)
        {
            aSpr.FlipH = ps.animatedSprite2D.FlipH;

            // set collisionShape flip with player
            if (aSpr.FlipH == false)
            {
                GetNode<CollisionShape2D>("SlashArea2D/CollisionShape2D").Position = new Vector2(80, -9);
            }
            else
            {
                GetNode<CollisionShape2D>("SlashArea2D/CollisionShape2D").Position = new Vector2(-80, -9);
            }
        }
    }
    public void Shoot()
    {
        // play sound
        Globals.PlayRandomizedSound(sndSwipe);

        ps = (player)Globals.pl;
        // play player attack anim
        ps.PlayAttackAnim();
        AnimatedSprite2D aSpr = (AnimatedSprite2D)GetNode("Slash");
        aSpr.Play("default");
        FlipAttack();


        //Debug.Print("Shoot - enemies: " + enemies.Count);
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                //Debug.Print("en:"+enemies[i].Name+" dmg: "+GetDamage());
                enemy en = enemies[i];
                en.take_damage(GetDamage());
                if (element == "ice")
                {
                    en.FreezeEnemy(freezeTime);
                }

            }
        }

        // attack golems
        if (aGolems.Count > 0)
        {
            for (int i = 0; i < aGolems.Count; i++)
            {
                //Debug.Print("aGolems:" + aGolems[i].Name + " dmg: " + GetDamage());
                AgroGolem ag = aGolems[i];
                ag.take_damage(GetDamage());
            }
        }


    }

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            // disable and then re-enable collider so enemies and get triggered again
            //Area2D sArea = (Area2D)GetNode("SlashArea2D");
            //sArea.Monitoring = false;
            Shoot();
            //sArea.Monitoring = true;
        }
    }

    public float GetDamage()
    {
        UpdateAttributes();
        return damage;
    }
    // set Slash anim2D acording to element
    public void SetWeaponElement(string eType)
    {
        element = eType;
        switch (eType)
        {
            case "energy":
                //slashScene.Free();
                slashScene = (PackedScene)ResourceLoader.Load("res://Scenes/slash_energy.tscn");
                if (newSlash != null)
                    newSlash.Free();
                newSlash = (AnimatedSprite2D)slashScene.Instantiate();
                AddChild(newSlash);
                dmgBase = 1f;
                break;
          /*  case "fire":
                //slashScene.Free();
                slashScene = (PackedScene)ResourceLoader.Load("res://Scenes/slash_fire.tscn");
                if (newSlash != null)
                    newSlash.Free();
                newSlash = (AnimatedSprite2D)slashScene.Instantiate();
                AddChild(newSlash);
                break;
            case "poison":
                //slashScene.Free();
                slashScene = (PackedScene)ResourceLoader.Load("res://Scenes/slash_poison.tscn");
                if (newSlash != null)
                    newSlash.Free();
                newSlash = (AnimatedSprite2D)slashScene.Instantiate();
                AddChild(newSlash);
                break;
          */
            case "ice":
                //slashScene.Free();
                slashScene = (PackedScene)ResourceLoader.Load("res://Scenes/slash_ice.tscn");
                if (newSlash != null)
                    newSlash.Free();
                newSlash = (AnimatedSprite2D)slashScene.Instantiate();
                AddChild(newSlash);
                dmgBase = .5f;
                break;
            case "leeches":
                //slashScene.Free();
                slashScene = (PackedScene)ResourceLoader.Load("res://Scenes/slash_leeches.tscn");
                if (newSlash != null)
                    newSlash.Free();
                newSlash = (AnimatedSprite2D)slashScene.Instantiate();
                AddChild(newSlash);
                dmgBase = .3f;
                break;
        }
        
        SetAOE();

    }

    // used for upgrades to see current levels
    public int GetDmgLevel()
    {
        return dmgLevel;
    }
    public int GetAOELevel()
    {
        return AOELevel;
    }
    public int GetAttackSpeedLevel()
    {
        return attackSpeedLevel;
    }


    public void AddUpgrade(string upgradeType,int rarityMult)
    {
        //Debug.Print("Upgrade Attack: "+"Slash"+" - "+element+" - " + upgradeType);
        switch (upgradeType)
        {
            case "Dmg":
                dmgLevel += rarityMult;
                break;
            case "AoE":
                AOELevel += rarityMult;
                SetAOE();
                break;
            case "Speed":
                attackSpeedLevel += rarityMult;
                SetAttackSpeed();
                break;
        }
    }

    public void OnBodyEntered(Node2D body) // enemy
    {
        // if enemy
        if (body.IsInGroup("Enemies"))
        {
            
            enemy en = (enemy)body;
            
            enemies.Add(en);
            //Debug.Print("Entered!!!!:"+en.Name+" - "+enemies.Count);
        }
    }
    public void OnBodyExited(Node2D body)
    {
        // if enemy
        if (body.IsInGroup("Enemies"))
        {
            
            enemy en = (enemy)body;
            enemies.Remove(en);
            //Debug.Print("Exited!!!!:" + en.Name + " - " + enemies.Count);
        }
    }

    public void OnAreaEntered(Area2D area) // hit agro golem
    {
        if (area.IsInGroup("Enemies"))
        {
            if (area.GetParent<RigidBody2D>().GetType() == typeof(AgroGolem))
            {
                AgroGolem ag = (AgroGolem)area.GetParent<RigidBody2D>();

                aGolems.Add(ag);
                //Debug.Print("Slash area entered: " + ag.Name);
            }
        }
    }

    public void OnAreaExited(Area2D area)
    {
        if (area.IsInGroup("Enemies")) // exit agro golem
        {
            if (area.GetParent<RigidBody2D>().GetType() == typeof(AgroGolem))
            {
                AgroGolem ag = (AgroGolem)area.GetParent<RigidBody2D>();

                aGolems.Remove(ag);
            }
        }
    }

}
