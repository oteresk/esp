using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

public partial class AttackCross : Area2D
{
    private player ps;
    public float damage;
    public float AOE;
    private int dmgLevel = 1;
    private float dmgBase = 1.5f;
    private int AOELevel = 1;
    private int attackSpeedLevel = 1;
    private float dmgInc = .5f;
    private float AOEInc = 1.2f;
    public float baseAOE = .1f;
    private float attackSpeedInc = .9f;
    public float freezeTime = 3.0f;
    public int poisonTime = 3;

    private PackedScene bulletScene;
    private string element; // attack element (energy, fire, etc)

    // attack Speed
    private Timer bulletTimer;
    public float finalAtkSpd; // = base*IAS-(PAS/50)+rndOffset-(ASL/50)
    public float atkSpdRndOffset; // 0.0-.5
    public float baseAtkSpd = 2; // can be different for each attack type (must be no less than 1)
    [Export] public AudioStreamPlayer sndCross;

    public override void _Ready()
    {
        bulletTimer = (Timer)GetNode("BulletTimer");
        atkSpdRndOffset = (float)GD.RandRange(0.0, 0.1);
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
        finalAtkSpd = baseAtkSpd * Globals.itemAtkSpd - (attackSpeedLevel / 13f - Globals.statAtkSpd);
        if (finalAtkSpd < .01f)
            finalAtkSpd = .01f;
        bulletTimer.WaitTime = finalAtkSpd;
        //Debug.Print("finalatkspd: " + finalAtkSpd);
    }
    public void SetAOE()
    {
        AOE = (baseAOE + AOELevel * AOEInc + (Globals.statAoE * AOEInc))/2;
        Debug.Print("AOR: " + AOE);
        Scale = new Vector2(AOE, AOE);
    }
    public float GetDamage()
    {
        UpdateAttributes();
        return damage;
    }
    private void UpdateAttributes()
    {
        damage = dmgBase * dmgLevel * dmgLevel * dmgInc * Globals.statDamage;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // play sound
        Globals.PlayRandomizedSound(sndCross);

        // play player attack anim
        ps = (player)Globals.pl;
        ps.PlayAttackAnim();
        var bullet = new Godot.Collections.Array { (Area2D)bulletScene.Instantiate(), (Area2D)bulletScene.Instantiate(), (Area2D)bulletScene.Instantiate(), (Area2D)bulletScene.Instantiate() };
        //Debug.Print("Bullet count:" + bullet.Count);
        Area2D a2D;
        BulletScript bScript;
        var bullets = GetNode("/root/World/Bullets");

        for (int iter=0; iter<bullet.Count; iter++)
        {
            a2D=(Area2D)bullet[iter];

            bScript = (BulletScript)bullet[iter];
            bScript.damage = GetDamage();
            bScript.bType = BulletScript.BulletType.Straight;
            bScript.range = 450;
            bScript.freezeTime = freezeTime;
            bScript.element = element;

            a2D.GlobalPosition = GetNode<Node2D>("ShootingPoint").GlobalPosition;
            //a2D.GlobalRotation = GetNode<Node2D>("ShootingPoint").GlobalRotation;
            bullets.AddChild(a2D);

            // set direction
            if (iter==0) // up
            {
                //a2D.LookAt(Vector2.Down);
                a2D.GlobalRotation = 290;
                bScript.direction = new Vector2(0, -1);
            }

            if (iter == 1) // down
            {
                a2D.LookAt(Vector2.Down);
                a2D.GlobalRotation = 180;
                bScript.direction = new Vector2(0, 1);
            }
            if (iter == 2) // left
            {
                a2D.LookAt(Vector2.Left);
                a2D.GlobalRotation = 90;
                bScript.direction = new Vector2(-1, 0);
            }
            if (iter == 3) // right
            {
                a2D.LookAt(Vector2.Right);
                a2D.GlobalRotation = 270;
                bScript.direction = new Vector2(1, 0);
            }

        }

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
            case "fire":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_fire.tscn");
                dmgBase = .8f;
                break;
            case "poison":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_poison.tscn");
                dmgBase = 0.3f;
                break;
            case "ice":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_ice.tscn");
                dmgBase = .5f;
                break;
            case "leeches":
                bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet_leeches.tscn");
                dmgBase = .3f;
                break;
        }
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

}
