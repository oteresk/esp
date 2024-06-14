using Godot;
using System;
using System.Diagnostics;
using static ItemScript;
using System.Threading.Tasks;

public partial class enemy : RigidBody2D
{
	[Export] public float health {get; set; } = 1;
    private float maxHealth;
    [Export] public float speed;
    public float poisonSpeed = 1f;
    Vector2 velocity;
	AnimatedSprite2D enemySprite;

    [Export] public PackedScene itemScene;

    [Export] public DamageType damageType;
    [Export] public int damage;
    [Export] public int damageTime;

    [Export] public bool leavesTrail=false;

    private double curTime = 0;

    private player pl;
	Vector2 playerPosition;

    public bool isDead = false;

    private Callable OnBodyEnteredCallable; // used to disconnect signal when enemy dies

    public bool frozen = false;

    [Export] public int XP;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		enemySprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		enemySprite.Animation = "default";
		enemySprite.Play();
        Node2D nodEnemies = (Node2D)GetNode(Globals.NodeEnemies);
		this.Reparent(nodEnemies);

        pl = (player)Globals.pl;
        Vector2 playerPosition = pl.GlobalPosition;

        enemySprite.FlipV = false;

        OnBodyEnteredCallable = (Callable)this.GetNode("Area2D").Get("area_entered");// get signal callable

        maxHealth = health;

    }



    public override void _Process(double delta)
    {
        if (Globals.playerAlive && !frozen && Visible)
        {
            curTime += delta;
            Vector2 direction = GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
            velocity = direction * speed * poisonSpeed;

            if (direction.X < 0) // if facing left
            {
                enemySprite.FlipH = true;
            }
            else
                enemySprite.FlipH = false;

            Position += velocity * (float)delta;
        }
    }


    public enum DamageType
    {
        Normal, Poison
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
            QueueFree();
        }

    }

    public void OnBodyEntered(Node2D col) // hit player
	{
        //Debug.Print("Enemy col:" + col.Name);
        if (curTime > .9f) // only allow enemy to hit player every 0.9 seconds
        {
            // enemy collide with player
            if (col.Name == "Player" && Globals.playerAlive && pl.canBeDamaged)
            {
                //Debug.Print("Hit: " + col.Name);

                if (damageType == DamageType.Normal)
                {
                    Globals.DamagePlayer(damage);
                }
                else
                    if (damageType == DamageType.Poison)
                {
                    Globals.PoisonPlayer(damage, damageTime);
                }

                // bounce back away from player
                Vector2 direction = GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
                velocity = -direction * speed * 2;
                Position += velocity * (float).2f;
                curTime = 0;
            }
        }
        // enemy collide with attack slash
        if (col.Name == "SlashArea2D" && Globals.playerAlive)
        {
            AttackSlash aSlash= (AttackSlash)GetNode(col.GetParent().GetPath());

            take_damage(aSlash.GetDamage());

            Node nodAtk = col.GetParent();
            AttackSlash atk = (AttackSlash)nodAtk;
            if (atk.element == "ice")
            {
                FreezeEnemy(atk.freezeTime);
            }

        }
    }

    public async void FreezeEnemy(float fTime)
    {
        frozen = true;
        enemySprite.Modulate = new Color(0, 0, 1, 1);
        enemySprite.Stop();
        await Task.Delay(TimeSpan.FromMilliseconds(fTime*1000));
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

            Timer tmrTrail = (Timer)GetNode("Timer");
            tmrTrail.WaitTime = GD.RandRange(2.2f, 3.9f);
        }
    }

    public void _on_occlusion_area_collider_area_entered(Area2D area)
    {
        if (area.IsInGroup("Players") && Globals.useOcclusion)
        {
            Visible = true;
            //Debug.Print("occlusion: enter:" + RDResource.ToString());

        }
    }

    public void _on_occlusion_area_collider_area_exited(Area2D area)
    {
        if (area.IsInGroup("Players") && Globals.useOcclusion)
        {
            Visible = false;
            //Debug.Print("occlusion: exit" + RDResource.ToString());
        }
    }

}
