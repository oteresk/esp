using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

// Projectile Attack
public partial class AttackRange : Area2D
{
    public enum BulletSource { Player, Tower }

    [Export] public BulletSource bSource;

    private player ps;
    public float damage;
    public float AOE;
    private int dmgLevel = 1;
    private float dmgBase = 1;
    private int AOELevel = 1;
    private int attackSpeedLevel = 1;
    private float dmgInc = 1.2f;
    private float AOEInc = .05f;
    public float baseAOE = .1f;
    private float attackSpeedInc = .9f;
    private float healingModifier = .2f; // the percentage of damage done by leeches that goes towards healing 
    public float freezeTime = 3.0f;
    public int poisonTime = 3;

    private PackedScene bulletScene;
    private string element; // attack element (energy, fire, etc)

    // attack Speed
    private Timer bulletTimer;
    public float finalAtkSpd; // = base*IAS-(PAS/50)+rndOffset-(ASL/50)
    public float atkSpdRndOffset; // 0.0-.5
    public float baseAtkSpd = 1.2f; // can be different for each attack type (must be no less than 1)

    public override void _Ready()
    {
        bulletTimer = (Timer)GetNode("BulletTimer");
        atkSpdRndOffset = (float)GD.RandRange(0.0, 0.5);
        UpdateAttributes();
        DelayAtkSpdStart();
        SetAOE();
    }

    async void DelayAtkSpdStart()
    {
        //wait for random offset then set attackspeed
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        SetAttackSpeed();
    }

    // set attack speed
    public void SetAttackSpeed() // =base*IAS-(PAS/50)-(ASL/50)
    {
        finalAtkSpd = baseAtkSpd * Globals.itemAtkSpd - (Globals.permItemAtkSpd / 50f) - (attackSpeedLevel / 13f);
        if (finalAtkSpd < .01f)
            finalAtkSpd = .01f;
        bulletTimer.WaitTime = finalAtkSpd;
    }

    public void SetAOE()
    {
        AOE = baseAOE + AOELevel * AOEInc;
        Debug.Print("AOR: " + AOE);
        //Scale = new Vector2(AOE, AOE);
    }

    private void UpdateAttributes()
    {
        damage = dmgBase*dmgLevel * dmgLevel * dmgInc;

        // if leeches, then heal player
        if (element == "leeches")
        {
            Globals.HealPlayer(damage * healingModifier);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var enemiesInRange = GetOverlappingBodies();
        if (enemiesInRange.Count > 0)
        {
            var targetEnemy = (Node2D)enemiesInRange[0];
            LookAt(targetEnemy.GlobalPosition);
        }
    }

    public void Shoot()
    {
        bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_energy.tscn"); // default for tower

        var newBullet = (Area2D)bulletScene.Instantiate();
        // AOE
        newBullet.Scale = new Vector2(AOE, AOE);

        newBullet.GlobalPosition = GetNode<Node2D>("ShootingPoint").GlobalPosition;
        newBullet.GlobalRotation = GetNode<Node2D>("ShootingPoint").GlobalRotation;
        var bullets = GetNode("/root/World/Bullets");
        bullets.AddChild(newBullet);

        BulletScript bScript = (BulletScript)newBullet;
        bScript.bType = BulletScript.BulletType.Homing;
        bScript.element = element;
        bScript.poisonTime = poisonTime;

        switch (bSource)
        {
            case BulletSource.Player:
                //newBullet.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale = new Vector2(0.3f, 0.3f);
                //newBullet.Modulate = new Color(0.2f, 0.2f, 1, 1);
                bScript.damage = GetDamage();
                bScript.range = 1200;

                //Debug.Print("dam: " + GetDamage());
                // play player attack anim
                ps = (player)Globals.pl;
                ps.PlayAttackAnim();
                break;
            case BulletSource.Tower:
                bScript.damage = 2;
                bScript.range = 1000;
                newBullet.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale = new Vector2(3, 3);
                newBullet.Modulate = new Color(1, 0.3f, 0.8f, 1);
                newBullet.Name = "Tower bullet";
                dmgBase = 1f; // TODO: make this increase when you upgrade tower
                break;
        }
    }

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            var enemiesInRange = GetOverlappingBodies();
            if (enemiesInRange.Count > 0)
            {
                Shoot();
            }
        }
    }

    public float GetDamage()
    {
        UpdateAttributes();
        return damage;
    }
    // set bullet acording to element
    public void SetWeaponElement(string eType)
    {
        element = eType;
        Debug.Print("SetWeaponElement:" + eType);
        switch (eType)
        {
            case "energy":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_energy.tscn");
                dmgBase = 1f;
                break;
                /*
            case "fire":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_fire.tscn");
                break;
                */
            case "poison":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_poison.tscn");
                dmgBase = 0.3f;
                break;
                /*
            case "ice":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_ice.tscn");
                break;
                */
            case "leeches":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_leeches.tscn");
                dmgBase = .3f;
                break;
        }
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
    public void AddUpgrade(string upgradeType, int rarityMult)
    {
        Debug.Print("Upgrade Attack: " + "Slash" + " - " + element + " - " + upgradeType);
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
