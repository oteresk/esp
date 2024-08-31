using Godot;
using System;
using System.Diagnostics;
using static ItemScript;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

public partial class enemy : RigidBody2D
{
	[Export] public float health {get; set; } = 1;
    private float maxHealth;
    [Export] public float speed;
    public float poisonSpeed = 1f;
    Vector2 velocity;
	AnimatedSprite2D enemySprite;

    [Export] public PackedScene itemScene;

    public enum DamageType { Normal, Poison, Fire }
    [Export] public DamageType damageType;
    [Export] public int damage;
    [Export] public int damageTime;

    [Export] public bool leavesTrail=false;

    [Export] public string enemyName;

    //private double curTime = 0;

    private player pl;
	Vector2 playerPosition;
    Vector2 direction;

    public bool isDead = false;

    private Callable OnBodyEnteredCallable; // used to disconnect signal when enemy dies

    public bool frozen = false;

    [Export] public int XP;

    private float wolfDist;
    private bool wolfRunning = false;
    private float bowtime;

    [Export] public float attackRange = 0;
    private float dist;
    public bool playerInDamageArea = false;
    private bool hasDamagedPlayer = false;
    private bool canAttack = true;

    private int flipCounter = 0;

    public override void _Ready()
	{
        //Globals.enemies++;
        //Globals.UpdateEnemies();
        enemySprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		enemySprite.Animation = "default";
		enemySprite.Play();
        Node2D nodEnemies = (Node2D)GetNode(Globals.NodeEnemies);
		this.Reparent(nodEnemies);

        enemySprite.FlipV = false;

 //       OnBodyEnteredCallable = (Callable)this.GetNode("Area2D").Get("area_entered");// get signal callable

        maxHealth = health;

        if (enemyName == "Wolf")
            wolfDist = GD.RandRange(300, 700);

        if (attackRange > 0)
            attackRange += GD.RandRange(-25, 25); // randomize attack distance a bit

        if (enemyName == "Floating Skull")
            FloatingSkullMovement();

    }



    public override void _PhysicsProcess(double delta)
    {
        if (Globals.playerAlive && !frozen && Visible) // move towards player
        {
            if (enemyName == "Skeleton" && enemySprite.Animation == "Attack")
            {
                // don't move
            }
            else
            {
                direction = GlobalTransform.Origin.DirectionTo(Globals.pl.GlobalPosition + new Vector2(0, -36));
                velocity = direction * speed * poisonSpeed;

                AdjustFlip();

                Position += velocity * (float)delta;

                if (enemyName == "Wolf" && !wolfRunning)
                    WolfMovement();

                if (attackRange > 0 && canAttack == true) // if enemy has an attack
                    Attack();

                //Debug.Print("Dist: " + GlobalTransform.Origin.DistanceTo(Globals.pl.GlobalPosition));
            }
        }
    }

    private void AdjustFlip()
    {
        // only process every 5th time
        flipCounter++;
        if (flipCounter>4)
        {
            flipCounter = 0;
            if (direction.X < 0) // if facing left
                enemySprite.FlipH = true;
            else
                enemySprite.FlipH = false;
        }
    }

    private void Attack()
    {
        dist = GlobalTransform.Origin.DistanceTo(Globals.pl.GlobalPosition);
        if (dist < attackRange)
        {
            canAttack = false;
            enemySprite.Animation = "Attack";
            enemySprite.Play();
            DelayAttack();
        }
    }

    private async void DelayAttack()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(GD.RandRange(2000,4000)));
        hasDamagedPlayer = false;
        canAttack = true;
        //Debug.Print("can attack");
    }

    public void FrameChanged()
    {
        // skeleton attack
        if (enemyName == "Skeleton" && enemySprite.Animation=="Attack")
        {
            if (enemySprite.Frame>=7 && enemySprite.Frame<11)
            {
                if (playerInDamageArea && hasDamagedPlayer==false)
                {
                    // damage player
                    Globals.DamagePlayer(damage*2);
                    hasDamagedPlayer = true;
                }
            }

            if (enemySprite.Frame==13) // return to walk anim
            {
                enemySprite.Animation = "default";
                enemySprite.Play();
            }

        }
    }

    private async void WolfMovement()
    {
        float dist = GlobalPosition.DistanceTo(Globals.ps.GlobalPosition);
        if (dist<wolfDist)
        {
            wolfRunning = true;
            enemySprite.Animation = "bow";
            enemySprite.Play();
            speed = 0;
            bowtime = GD.RandRange(1500, 3400);
            await Task.Delay(TimeSpan.FromMilliseconds(bowtime));
            speed = GD.RandRange(140, 180);
            enemySprite.Animation = "run";
            enemySprite.Play();
        }
    }

    private async void FloatingSkullMovement()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(GD.RandRange(5000, 6000)));
        speed = speed * 3;
        enemySprite.Animation = "Run";
        //Debug.Print("Run");

        await Task.Delay(TimeSpan.FromMilliseconds(GD.RandRange(1500, 2000)));
        speed = speed / 3;
        enemySprite.Animation = "default";
        FloatingSkullMovement();
        //Debug.Print("Walk");
    }
    public void take_damage(float dmg) 
	{
		health -= dmg;
        DamageBlink();

        if (health <= 0) 
		{
			EnemyDrop();
		}
	}

    private async void DamageBlink()
    {
        if (enemySprite != null && !isDead)
        {
            ShaderMaterial enemyMat = (ShaderMaterial)enemySprite.Material;
            if (enemyMat!=null)
                enemyMat.SetShaderParameter("active", true);
            else
                Debug.Print("*** enemy.NotificationEnterCanvas DamageBlink shader not found on AnimatedSprite2D ***");
        }
            
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        if (enemySprite != null && !isDead)
        {
            ShaderMaterial enemyMat = (ShaderMaterial)enemySprite.Material;
            if (enemyMat != null)
                enemyMat.SetShaderParameter("active", false);
            else
                Debug.Print("*** enemy.NotificationEnterCanvas DamageBlink shader not found on AnimateSprite2D ***");
        }
    }

    async void EnemyDrop()
	{
		//Debug.Print("EnemyDrop");
		// wait a second for enemy to explode
        await Task.Delay(TimeSpan.FromMilliseconds(10));

        // spawn item
        Area2D item = (Area2D)itemScene.Instantiate();
        ItemScript iScript = (ItemScript)item;
        iScript.XP = XP;
		iScript.CreateItem();
        if (!isDead)
        {
            item.Position = Position;
            Node2D nodItems = (Node2D)GetNode(Globals.NodeItems);
            nodItems.AddChild(item);

            // remove enemy
            //Debug.Print("queuefree:" + Name);
            isDead = true;
            //Globals.enemies--;
            //Globals.UpdateEnemies();
            QueueFree();
        }

    }

    public async void FreezeEnemy(float fTime)
    {
        int cycles = (int)fTime;

        frozen = true;
        enemySprite.Modulate = new Color(0, 0, 1, 1);
        enemySprite.Stop();
        //await Task.Delay(TimeSpan.FromMilliseconds(fTime*1000));

        waitagain:
        await Task.Delay(TimeSpan.FromMilliseconds(1000));

        if (Globals.rootNode.GetTree().Paused == true) // if paused then wait for unpause
        {
            //Debug.Print("paused");
            goto waitagain;
        }

        cycles--;

        if (cycles > 0)
            goto waitagain;

        frozen = false;
        if (IsInstanceValid(enemySprite)) // don't access disposed nodes
        {
            enemySprite.Modulate = new Color(1, 1, 1, 1);
            enemySprite.Play();
        }
    }

    public void LeaveTrail()
    {
        if (Visible)
        {
            var trailScene = (PackedScene)ResourceLoader.Load("res://Scenes/SlimeTrail.tscn");
            var newSlimeTrail = (AnimatedSprite2D)trailScene.Instantiate();
            Node2D nodEnemies = (Node2D)GetNode(Globals.NodeEnemies);
            nodEnemies.AddChild(newSlimeTrail);
            newSlimeTrail.GlobalPosition = GlobalPosition;

            if (enemyName == "Poison Slime")
                newSlimeTrail.Modulate = new Color(0, 1, 0, .6f);
            else
                newSlimeTrail.Modulate = new Color(1, 1, 1, .6f);

            Timer tmrTrail = (Timer)GetNode("Timer");
            tmrTrail.WaitTime = GD.RandRange(2.2f, 3.9f);
        }
    }

    // this is when enemy is hidden for efficiency reasons
    async void MoveTowardsPlayer()
    {
        //Debug.Print("Move TowardsPlayer");
        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        if (Globals.playerAlive && IsInstanceValid(this) && Globals.rootNode.GetTree().Paused == false)
        {
            Vector2 direction = GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
            velocity = direction * speed * poisonSpeed;
            Position += velocity * (float)1;
            if (!Visible)
                MoveTowardsPlayer();
        }
    }

}
