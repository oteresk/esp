using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static Godot.TextServer;

public partial class AttackOrbit : Area2D
{
    public enum BulletSource { Player, Tower }

    [Export] public BulletSource bSource;

    public int dmgLevel = 1;
    public int AOELevel = 1;
    public int attackSpeedLevel = 1;

    private string element; // attack element (energy, fire, etc)
    public float damage;
    public float AOE;
    private float dmgInc = .5f;
    private float AOEInc = .18f;
    private float attackSpeedInc = .4f;
    private float dmgBase = 1.5f;
    public float baseAOE = .07f;
    public float baseAtkSpd = 18.2f; // can be different for each attack type (must be no less than 1)

    public float freezeTime = 3.0f;
    public int poisonTime = 3;


    // attack Speed
    public float finalAtkSpd; // = base*IAS-(PAS/50)+rndOffset-(ASL/50)

    private Vector2 direction;


    [Export] public float radius;
    [Export] public Sprite2D pivot;
    [Export] public Sprite2D bulletEnergy;
    [Export] public Sprite2D bulletFire;
    [Export] public Sprite2D bulletPoison;
    private Sprite2D bullet;
    public override void _Ready()
	{
        bullet=new Sprite2D();

        // AOE
        bullet.Scale = new Vector2(AOE, AOE);
        bullet.Scale = new Vector2(AOE, AOE);



        //bScript = (BulletScript)newBullet;
        //bScript.bType = BulletScript.BulletType.Flame;
        //bScript.element = element;
        //bScript.poisonTime = poisonTime;

        //bScript.damage = GetDamage();
        //bScript.range = 1200;

        DelayedStart();

        bullet.Position=new Vector2(radius, radius);

    }

    public async void DelayedStart()
    {
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(100));

        SetAttackSpeed();
        SetAOE();
        SetDamage();
    }
    
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        pivot.GlobalRotation += (float)((double)finalAtkSpd * delta);
    }

    public float GetDamage()
    {
        SetDamage();
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
                bullet = bulletEnergy;
                bulletEnergy.Visible = true;
                dmgBase = 1f;
                break;
            case "fire":
                bullet = bulletFire;
                bulletFire.Visible = true;
                break;
            
            case "poison":
                bullet = bulletPoison;
                bulletPoison.Visible = true;
                dmgBase = 0.3f;
                break;
        }
    }
    public void SetAOE()
    {
        AOE = baseAOE + AOELevel * AOEInc + (Globals.statAoE * AOEInc);
        Debug.Print("AOE: " + AOE);
        bullet.Scale = new Vector2(AOE, AOE);
        //radius = 70+(AOE*10);
        radius = 70;
        bullet.Position = new Vector2(radius, radius);
    }

    // set attack speed
    public void SetAttackSpeed() // =base*IAS-(PAS/50)-(ASL/50)
    {
        finalAtkSpd = baseAtkSpd * (1.5f-Globals.itemAtkSpd) * (attackSpeedLevel* attackSpeedInc) * ((Globals.statAtkSpd + 1) * attackSpeedInc );
        if (finalAtkSpd < .01f)
            finalAtkSpd = .01f;

        Debug.Print("finalAtkSpd:" + finalAtkSpd+" statatkspeed:"+ Globals.statAtkSpd);

    }

    public void SetDamage()
    {
        damage = dmgBase * dmgLevel * dmgLevel * dmgInc * Globals.statDamage;
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

    // bullet hit an enemy
    public void OnBodyEntered(Node body)
    {
        if (body.HasMethod("take_damage"))
        {
            body.Call("take_damage", damage);

            if (element=="energy")
            {
                Node aBullet=bullet.GetNode("AnimatedSprite2D");
                AnimatedSprite2D animBullet = (AnimatedSprite2D)aBullet;
                animBullet.Play();
            }

            if (element == "poison") // if poison
            {
                if (body.GetType() == typeof(enemy))
                {

                    // instantiate poison object on enemy
                    var poisonScene = (PackedScene)ResourceLoader.Load("res://Scenes/poison.tscn");
                    var newPoison = (AnimatedSprite2D)poisonScene.Instantiate();
                    Node2D pScene = (Node2D)newPoison;
                    Node2D body2D = (Node2D)body;
                    pScene.Scale = body2D.Scale;

                    newPoison.Play();
                    body.AddChild(newPoison);
                    var pScript = (Poison)newPoison;
                    pScript.pTarget = Poison.PoisonTarget.Enemy;
                    pScript.poisonTime = poisonTime;
                    pScript.poisonDamage = damage;
                    pScript.enemy = (Node2D)body;
                }
            }

            if (element == "fire")
            {
                if (body.GetType() == typeof(enemy))
                {
                    // add flame to world landing spot
                    var fireScene = (PackedScene)ResourceLoader.Load("res://Scenes/Fire.tscn");
                    var newFire = (Area2D)fireScene.Instantiate();
                    Node2D fScene = (Node2D)newFire;
                    Node2D body2D = (Node2D)body;

                    // instantiate flame object on enemy
                    if (!body.HasNode("Area2D/Flame")) // only create flame if one doesn't exist on enemy already
                    {
                        var flameScene2 = (PackedScene)ResourceLoader.Load("res://Scenes/Flame.tscn");
                        var newFlame2 = (AnimatedSprite2D)flameScene2.Instantiate();
                        Node2D fScene2 = (Node2D)newFlame2;
                        fScene2.Scale = body2D.Scale;

                        newFlame2.Play();
                        body.AddChild(newFlame2);
                        var fScript = (Flame)newFlame2;
                        fScript.fTarget = Flame.FlameTarget.Enemy;
                        fScript.flameTime = Globals.flameTime;
                        fScript.flameDamage = damage*.17f;
                        fScript.slowDown = false;
                        fScript.scaleMod = new Vector2(.3f, .3f);
                        fScript.enemy = (Node2D)body;
                        fScript.onFire = true;
                    }
                }
            }


        }
    }

    public void ExplodeBullet()
    {
        var expBulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/ExplodeBullet.tscn");
        var newBullet = (Node2D)expBulletScene.Instantiate();
        GetParent().AddChild(newBullet);
        newBullet.Name = "Explosion";
        newBullet.Position = Position;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Visible = false;
    }


}
