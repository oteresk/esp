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
    private float dmgInc = .5f;
    private int AOELevel = 1; // (1-12)
    private int attackSpeedLevel = 1; // max 12
    private float healingModifier = .2f; // the percentage of damage done by leeches that goes towards healing 
    public float freezeTime = 3.0f;

    private PackedScene slashScene;
    private AnimatedSprite2D newSlash;

    public string element; // attack element (energy, fire, etc)

    // attack Speed
    private Timer bulletTimer;
    public float finalAtkSpd; // = base*IAS-(PAS/50)+rndOffset-(ASL/50)
    public float atkSpdRndOffset; // 0.0-.5
    public float baseAtkSpd = 1.4f; // can be different for each attack type (must be no less than 1)
    public float baseAOE = 1.2f;

    public override void _Ready()
    {
        bulletTimer = (Timer)GetNode("BulletTimer");
        atkSpdRndOffset = (float)GD.RandRange(0.0, 0.5);
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
        finalAtkSpd = baseAtkSpd * Globals.itemAtkSpd - (Globals.permItemAtkSpd / 50f) - (attackSpeedLevel / 13f);
        if (finalAtkSpd < .01f)
            finalAtkSpd = .01f;
        bulletTimer.WaitTime = finalAtkSpd;
    }

    public void SetAOE()
    {
        AOE = baseAOE + AOELevel * .6f;
        Scale=new Vector2(AOE,AOE);
    }

    private void UpdateAttributes()
    {
        damage = dmgBase* dmgLevel * dmgLevel*dmgInc;

        // if leeches, then heal player
        if (element== "leeches")
        {
            Globals.HealPlayer(damage*healingModifier);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Shoot()
    {
        ps = (player)Globals.pl;
        // play player attack anim
        ps.PlayAttackAnim();
        AnimatedSprite2D aSpr = (AnimatedSprite2D)GetNode("Slash");
        aSpr.Play("default");
        // set propper flip acording to player
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

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            // disable and then re-enable collider so enemies and get triggered again
            Area2D sArea = (Area2D)GetNode("SlashArea2D");
            sArea.Monitoring = false;
            Shoot();
            sArea.Monitoring = true;
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
        Debug.Print("Upgrade Attack: "+"Slash"+" - "+element+" - " + upgradeType);
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

}
