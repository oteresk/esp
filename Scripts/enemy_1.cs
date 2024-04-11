using Godot;
using System;
using System.Diagnostics;
using static ItemScript;
using System.Threading.Tasks;

public partial class enemy_1 : RigidBody2D
{
	[Export] public int health {get; set; } = 1;
	private const float Speed = 45;
	Vector2 velocity;
	AnimatedSprite2D enemySprite;

	private int potionFreq = 20; // how often enemy drops a potion instead of gem

    [Export] public PackedScene itemScene;

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

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Globals.playerAlive)
		{
            Vector2 direction = GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
			velocity = direction * Speed;

			Position += velocity * (float)delta;
		}
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
		
		if (col.Name=="Player" && Globals.playerAlive)
		{
            Debug.Print("Hit: " + col.Name);
            Globals.DamagePlayer(5);

            // bounce back away from player
            Vector2 direction = GlobalTransform.Origin.DirectionTo(pl.GlobalPosition);
            velocity = -direction * Speed * 2;
            Position += velocity * (float).2f;
        }
    }


}
