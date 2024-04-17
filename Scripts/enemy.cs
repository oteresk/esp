using Godot;
using System;
using System.Diagnostics;
using static ItemScript;
using System.Threading.Tasks;

public partial class enemy : RigidBody2D
{
	[Export] public int health {get; set; } = 1;
    [Export] public float speed;
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

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Globals.playerAlive)
		{
            curTime += delta;
            Vector2 direction = GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
			velocity = direction * speed;

            if (direction.X<0) // if facing left
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

    public void take_damage() 
	{
		health -= 1;
		if(health <= 0) 
		{
			EnemyDrop();
            
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
		iScript.CreateItem();

        item.Position = Position;
        Node2D nodItems = (Node2D)GetNode(Globals.NodeItems);
        nodItems.AddChild(item);

		// remove enemy
		//Debug.Print("queuefree:" + Name);
        QueueFree();
    }

	public void OnBodyEntered(Node2D col) // hit player
	{
        if (curTime > .9f) // only allow enemy to hit player every 0.9 seconds
        {
            if (col.Name == "Player" && Globals.playerAlive)
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
    }

    public void LeaveTrail()
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
