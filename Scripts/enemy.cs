using Godot;
using System;
using System.Diagnostics;
using static ItemScript;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.Xml;
using System.Linq;

public partial class enemy : RigidBody2D
{
    [Export] public float health { get; set; } = 1;
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

    [Export] public bool leavesTrail = false;

    [Export] public string enemyName;
    [Export] public Laser laser;

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
    [Export] public float randomAttackRange = 0;
    private float dist;
    public bool playerInDamageArea = false;
    private bool hasDamagedPlayer = false;
    private bool canAttack = true;

    private int flipCounter = 0;

    [Export] public AudioStreamPlayer2D[] enemyNoise;
    [Export] public AudioStreamPlayer2D[] enemyDeathSound;
    [Export] public AudioStreamPlayer2D enemyAttackSound;

    [Export] public PackedScene bloodScene;
    [Export] public Color bloodColor;
    private Node2D newBlood;
    [Export] public int bloodFrame;
    private Vector2 offset;
    private bool occluded = false;
    private float occlusionDistance = 920;
    private float randOcclusionVariance;

    Node colNode;
    CollisionShape2D col;

    public override void _Ready()
    {
        offset=new Vector2(0, -36);
        randOcclusionVariance = GD.RandRange(0, 400);
        colNode = (Node)GetNode("CollisionShape2D2");
        col = (CollisionShape2D)colNode;

        if (randomAttackRange > 0)
            attackRange += (float)GD.RandRange(-randomAttackRange, randomAttackRange);
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

        PlayRandomSound(enemyNoise); // plays a random enemy noise

        Globals.enemies++;
        Globals.UpdateEnemies();

        Name="Bat"+Globals.enemies.ToString();
    }

    private async void PlayRandomSound(AudioStreamPlayer2D[] eSound)
    {
        if (eSound.Length > 0)
        {
            int r=GD.RandRange(0, eSound.Length-1);

            if (IsInstanceValid(eSound[r]))
                Globals.PlayRandomizedSound2D(eSound[r]);

            await Task.Delay(TimeSpan.FromMilliseconds(GD.RandRange(15000, 25000)));
            PlayRandomSound(eSound);
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Globals.playerAlive && !frozen && Visible && !isDead) // move towards player
        {
            //get distance to player
            if (flipCounter == 1) // only check distance every 5th time to save processing
            {
                dist = GlobalTransform.Origin.DistanceTo(Globals.pl.GlobalPosition);
                // check occlusion based on dist
                if (dist<occlusionDistance && occluded==true)
                {
                    //occluded = false;
                    //ShowEnemy();
                }
                else
                if (dist> occlusionDistance+40 && occluded==false)
                {
                    //occluded=true;
                    //HideEnemy();
                }
                if (dist> occlusionDistance*1.3+randOcclusionVariance)
                {
                    int r = GD.RandRange(1, 2);
                    if (r==1) //teleport
                    {
                        randOcclusionVariance = GD.RandRange(0, 400);
                        direction = GlobalTransform.Origin.DirectionTo(Globals.pl.GlobalPosition + offset);
                        Position = Globals.ps.Position+direction * occlusionDistance;
                        occluded = false;
                        ShowEnemy();
                    }
                    else // kill
                    {
                        if (IsInstanceValid(this))
                        {
                            Globals.enemies--;
                            Globals.UpdateEnemies();
                            this.QueueFree();
                        }
                    }
                }

            }

            if ((enemyName == "Skeleton" && enemySprite.Animation == "Attack") || (enemyName == "Evil Eye" && enemySprite.Animation == "Attack")) // don't attack if already attacking
            {
                // don't move
            }
            else
            {
                direction = GlobalTransform.Origin.DirectionTo(Globals.pl.GlobalPosition + offset);
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

        // shorten laser to player
        if (enemyName=="Evil Eye" && canAttack==false && !occluded) // if lasering
        {
            if (dist<400)
            {
                laser.Scale = new Vector2(dist / 400, laser.Scale.Y);
                laser.endParticle.Visible = true;
                laser.endParticle.Emitting = true;
                laser.endParticle.Scale = new Vector2(14, 14);
                laser.endParticle.Spread = 180;

                // damage player
                if (enemySprite.Animation == "Attack" && enemySprite.Frame >= 19)
                {
                    //Debug.Print("HP:" + Globals.HP);
                    Globals.DamagePlayer(damage / 18);
                    //Debug.Print("damage player:" + damage);
                }
            }
            else
            {
                laser.endParticle.Scale = new Vector2(.5f, .5f);
                laser.endParticle.Spread = 0;
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

    private async void Attack()
    {
        
        if (dist < attackRange && !isDead)
        {
            canAttack = false;
            enemySprite.Animation = "Attack";
            enemySprite.Play();
            if (enemyAttackSound != null)
            {
                Globals.PlayRandomizedSound2D(enemyAttackSound);
            }

            DelayAttack();
        }
        await Task.Delay(TimeSpan.FromMilliseconds(20)); // check dist every 20 ms
    }

    private async void DelayAttack()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(GD.RandRange(2000,4000)));

        // wait extra for evil eye attack
        if (enemyName=="Evil Eye")
            await Task.Delay(TimeSpan.FromMilliseconds(2000));

        if (!isDead)
        {
            hasDamagedPlayer = false;
            canAttack = true;
            //Debug.Print("can attack");
        }

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

            if (enemySprite.Animation == "Attack" && enemySprite.Frame==13) // return to walk anim
            {
                enemySprite.Animation = "default";
                enemySprite.Play();
            }

        }

        // spawn blood
        if (enemySprite.Animation=="Death" && enemySprite.Frame==bloodFrame && bloodScene!=null)
        {
            newBlood = (Node2D)bloodScene.Instantiate();
            AddChild(newBlood);
            newBlood.Modulate = bloodColor;
            AnimatedSprite2D blood = (AnimatedSprite2D)newBlood;
            blood.Play();
        }

        // Evil Eye

        if (enemyName == "Evil Eye" && enemySprite.Animation=="Attack" && enemySprite.Frame == 19)
        {
            laser.Shoot();
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
            if (IsInstanceValid(this))
            {
                enemySprite.Animation = "run";
                enemySprite.Play();
            }
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
            KillEnemy();            
        }
        else
            PushBackEnemy();
	}

    private void PushBackEnemy()
    {
        float pushDist = 80;

        if (Position.X < Globals.ps.Position.X)
            pushDist = -pushDist;

        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", new Vector2(Position.X+pushDist, Position.Y), .1f);

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
        if (enemySprite != null && !isDead && IsInstanceValid(this))
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
            isDead = true;
            item.Position = Position;
            Node2D nodItems = (Node2D)GetNode(Globals.NodeItems);
            nodItems.AddChild(item);

            // disable colliders
            col.Disabled = true;

            //colNode = (Node)GetNode("Occlusion Area Collider");
            //Area2D col2 = (Area2D)colNode;
            //col2.CallDeferred("set_monitorable", false);
            //col2.CallDeferred("set_monitoring", false);

        }

    }

    private async void KillEnemy()
    {
        ShaderMaterial enemyMat = (ShaderMaterial)enemySprite.Material;
        if (enemyMat != null) // stop damage flash
            enemyMat.SetShaderParameter("active", false);

        // delay death anim slightly acording to dist from player
        float deathDist = GlobalTransform.Origin.DistanceTo(Globals.pl.GlobalPosition)/1100;
        if (deathDist > .4f)
            deathDist = .4f;
        await Task.Delay(TimeSpan.FromMilliseconds(deathDist));


        enemySprite.Play("Death");

        // play death sound
        PlayRandomSound(enemyDeathSound);

    }

    public void AnimationFinished()
    {
        if (enemySprite.Animation == "Death")
        {
            //Debug.Print("Death visible=false");
            Globals.enemies--;
            Globals.UpdateEnemies();
            //Visible = false;
            if (IsInstanceValid(this)) // don't access disposed nodes
                this.QueueFree();
        }

        if (enemySprite.Animation == "Attack" && enemyName=="Evil Eye")
        {
            enemySprite.Animation = "default";
            enemySprite.Play();
            laser.Visible = false;
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

    private void ShowEnemy()
    {
        enemySprite.Visible = true;
        Sleeping = true;
        col.Disabled = false;
    }

    private void HideEnemy()
    {
        enemySprite.Visible = false;
        Sleeping = false;
        col.Disabled = true;
    }


}
