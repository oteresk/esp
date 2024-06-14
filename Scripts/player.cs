using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class player : Area2D
{
	[Export] public int baseSpeed { get; set; } = 170;
	public int speedLevel=1;
	private float speedInc = 70;
	public float Speed;
	private float speedMultiplier = 1;
	public float poisonSpeed = 1;
	[Export] public float DashSpeed = 600;
	[Export] public float DashDuration = 0.2f;
	[Export] public float DashCooldown = 1.0f;
	public float dashTimer = 0;
	private float dashCooldownTimer = 0;
	private bool isDashing = false;

	public Vector2 ScreenSize;
	Vector2 velocity;
	public AnimatedSprite2D animatedSprite2D;
	[Export] public bool canBeDamaged = true;
	[Export] public CollisionShape2D magnetismShape;
	[Export] public Sprite2D itemIconSpeed;
	[Export] public Sprite2D itemIconAttackSpeed;
	[Export] public Sprite2D itemIconDamage;
	[Export] public Sprite2D itemIconAoE;
	[Export] public Sprite2D itemIconShield;
	[Export] public Sprite2D txtSpeed;
	[Export] public Sprite2D txtAtkSpeed;
	[Export] public Sprite2D txtDamage;
	[Export] public Sprite2D txtAoE;
	[Export] public Sprite2D txtShield;
	[Export] public AnimatedSprite2D shield;
	private Sprite2D[] itemIcons;
	private Node2D newAttack;

	private PackedScene attackSceneProjectile;
	private PackedScene attackSceneSlash;
	private PackedScene attackSceneCross;

	public List<AttackSlash> atkSlashEnergy;
	public List<AttackSlash> atkSlashIce;
	public List<AttackSlash> atkSlashLeeches;
	public List<AttackRange> atkProjectileEnergy;
	public List<AttackRange> atkProjectilePoison;
	public List<AttackRange> atkProjectileLeeches;
	public List<AttackCross> atkCrossFire;
	public List<AttackCross> atkCrossIce;
	public List<AttackCross> atkCrossLeeches;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// set player in Globals
		Globals.pl = (Area2D)GetNode(Globals.NodePlayer);
		Globals.ps = (player)Globals.pl;

		ScreenSize = GetViewportRect().Size;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "idle";
		animatedSprite2D.Play();

		SetMagnetismShape();

		// setup itemIcons List
		itemIcons = new Sprite2D[5];
		itemIcons[0] = itemIconSpeed;
		itemIcons[1] = itemIconAttackSpeed;
		itemIcons[2] = itemIconDamage;
		itemIcons[3] = itemIconAoE;
		itemIcons[4] = itemIconShield;

		// load attack
		attackSceneProjectile = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/attack_projectile.tscn");
		attackSceneSlash = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/attack_slash.tscn");
		attackSceneCross = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/attack_Cross.tscn");


		// init attack arrays
		atkSlashEnergy = new List<AttackSlash>();
		atkSlashIce = new List<AttackSlash>();
		atkSlashLeeches = new List<AttackSlash>();
		atkProjectileEnergy = new List<AttackRange>();
		atkProjectilePoison = new List<AttackRange>();
		atkProjectileLeeches = new List<AttackRange>(); ;
		atkCrossFire = new List<AttackCross>();
		atkCrossIce = new List<AttackCross>();
		atkCrossLeeches = new List<AttackCross>();

		// add default attack

		newAttack = (Node2D)attackSceneSlash.Instantiate();
		AttackSlash newAttackSlash = (AttackSlash)newAttack;
		AddChild(newAttackSlash);
		atkSlashEnergy.Add(newAttackSlash);
		newAttack.Call("SetWeaponElement", "energy");


		/*
		newAttack = (Node2D)attackSceneProjectile.Instantiate();
		AttackRange newAttackProjectile = (AttackRange)newAttack;
		AddChild(newAttackProjectile);
		atkProjectileEnergy.Add(newAttackProjectile);
		newAttack.Call("SetWeaponElement", "energy");
		*/
	}

	public void SetMagnetismShape()
	{
        // set magnetism size
		if (IsInstanceValid(magnetismShape))
	        magnetismShape.Scale = new Vector2(Globals.magnetism, Globals.magnetism);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Globals.playerAlive)
		{

			velocity = Vector2.Zero;

			// Basic movement handlers
			if (Input.IsActionPressed("move_right"))
				velocity.X += 0.1f;
			if (Input.IsActionPressed("move_left"))
				velocity.X -= 0.1f;
			if (Input.IsActionPressed("move_down"))
				velocity.Y += 0.1f;
			if (Input.IsActionPressed("move_up"))
				velocity.Y -= 0.1f;

			velocity = velocity.Normalized();

			// If the dash cooldown is currently up, then subtract delta time from it
			if (dashCooldownTimer > 0)
				dashCooldownTimer -= (float)delta;

			// Check if dash needs to be set
			if (Input.IsActionPressed("dash") && !isDashing && dashCooldownTimer <= 0 && (velocity.X != 0 || velocity.Y != 0))
			{
				isDashing = true;
				dashTimer = DashDuration;
				dashCooldownTimer = DashCooldown;
			}

			// If player is dashing, multiply velocity by dash speed and check dash cooldown
			if (isDashing)
			{
				velocity *= DashSpeed;
				dashTimer -= (float)delta;

				if (dashTimer <= 0)
				{
					isDashing = false;
				}
			}
			// If the player is moving then multiply the velocity by speed variable
			else if (velocity.Length() > 0)
			{
				velocity *= (baseSpeed * speedMultiplier * poisonSpeed)+(speedLevel*speedInc);
			}

			// Moving the character around the screen
			Position += velocity * (float)delta;
			// Position = new Vector2( x: Position.X, y: Position.Y);


			// Setting the animations for the character
			if (velocity.X != 0)
			{
				if (isDashing && animatedSprite2D.Animation != "attack")
					animatedSprite2D.Animation = "dash";
				else
				{
					if (animatedSprite2D.Animation != "attack")
					{
						animatedSprite2D.Animation = "walk";
						animatedSprite2D.FlipV = false;
						animatedSprite2D.FlipH = velocity.X < 0;
					}
				}
			}
			else if (velocity.Y != 0)
			{
				if (isDashing && animatedSprite2D.Animation != "attack")
					animatedSprite2D.Animation = "dash";
				else
				{
					if (animatedSprite2D.Animation != "attack")
						animatedSprite2D.Animation = "walk";
				}
			}
			else
			{
				if (animatedSprite2D.Animation != "attack")
					animatedSprite2D.Animation = "idle";
			}
		}

	}

	public void OnMagnetismEntered(Area2D area)
	{
		if (area.IsInGroup("Pickups"))
		{
			ItemScript gs = (ItemScript)area;
			if (gs != null)
			{
				gs.collected = true; // start magnetism 
			}
		}

	}

	public void IncreaseSpeed(float mult)
	{
		speedMultiplier = mult;
		itemIconSpeed.Visible = true;
		ShowText(txtSpeed);
	}

	public void EnableShield()
	{
		itemIconShield.Visible = true;
		ShowText(txtShield);
		shield.Visible = true;
		Globals.playerShieldActive = true;
		shield.Play();
	}

	public void DisableShield()
	{
		itemIconShield.Visible = false;
		shield.Visible = false;
		Globals.playerShieldActive = false;
	}

	private void ShowText(Sprite2D txt)
	{
		// fade out text
		txt.Visible = true;
		Tween tween = GetTree().CreateTween();
		tween.Parallel().TweenProperty(txt, "modulate:a", 0f, 3.0f);
		tween.Parallel().TweenProperty(txt, "position", new Vector2(120, txt.Position.Y), 3.0f);
		tween.TweenProperty(txt, "visible", false, 0);
		tween.TweenProperty(txt, "modulate:a", 1f, 0);
		tween.TweenProperty(txt, "position", new Vector2(0, txt.Position.Y), 0);
	}

	public void SetAllAttackSpeed(float itemAtkSpdMult)
	{
		// set attackspeed for all attacks
		Globals.itemAtkSpd = Globals.itemAtkSpd * itemAtkSpdMult;

		if (atkSlashEnergy.Count > 0)
			atkSlashEnergy[0].SetAttackSpeed();
		if (atkSlashIce.Count > 0)
			atkSlashIce[0].SetAttackSpeed();
		if (atkSlashLeeches.Count > 0)
			atkSlashLeeches[0].SetAttackSpeed();
		if (atkProjectileEnergy.Count > 0)
			atkProjectileEnergy[0].SetAttackSpeed();
		if (atkProjectilePoison.Count > 0)
			atkProjectilePoison[0].SetAttackSpeed();
		if (atkProjectileLeeches.Count > 0)
			atkProjectileLeeches[0].SetAttackSpeed();
		if (atkCrossFire.Count > 0)
			atkCrossFire[0].SetAttackSpeed();
		if (atkCrossIce.Count > 0)
			atkCrossIce[0].SetAttackSpeed();
		if (atkCrossLeeches.Count > 0)
			atkCrossLeeches[0].SetAttackSpeed();

		itemIconAttackSpeed.Visible = true;
		ShowText(txtAtkSpeed);
	}

	public void ResetSpeed()
	{
		speedMultiplier = 1;
		itemIconSpeed.Visible = false;
	}

	public void ResetAllAttackSpeed()
	{
		Globals.itemAtkSpd = Globals.itemAtkSpd * 2;

		if (atkSlashEnergy.Count > 0)
			atkSlashEnergy[0].SetAttackSpeed();
		if (atkSlashIce.Count > 0)
			atkSlashIce[0].SetAttackSpeed();
		if (atkSlashLeeches.Count > 0)
			atkSlashLeeches[0].SetAttackSpeed();
		if (atkProjectileEnergy.Count > 0)
			atkProjectileEnergy[0].SetAttackSpeed();
		if (atkProjectilePoison.Count > 0)
			atkProjectilePoison[0].SetAttackSpeed();
		if (atkProjectileLeeches.Count > 0)
			atkProjectileLeeches[0].SetAttackSpeed();
		if (atkCrossFire.Count > 0)
			atkCrossFire[0].SetAttackSpeed();
		if (atkCrossIce.Count > 0)
			atkCrossIce[0].SetAttackSpeed();
		if (atkCrossLeeches.Count > 0)
			atkCrossLeeches[0].SetAttackSpeed();

		itemIconAttackSpeed.Visible = false;
	}

	// adjust item icon positions
	public void AdjustItemIcons(int itemCount)
	{
		//Debug.Print("Adjust - count:" + itemCount);
		int curItem = 0;
		if (itemCount == 1)
		{
			// 2 icons
			for (int i = 1; i <= itemIcons.Length; i++)
			{
				if (itemIcons[i - 1].Visible) // if icon visibility==true
				{
					itemIcons[i - 1].Position = new Vector2(0, itemIcons[i - 1].Position.Y);
				}
			}
		}

		if (itemCount == 2)
		{
			// 2 icons
			for (int i = 1; i <= itemIcons.Length; i++)
			{
				if (itemIcons[i - 1].Visible) // if icon visibility==true
					curItem++;
				if (curItem == 1 && itemIcons[i - 1].Visible)
				{
					itemIcons[i - 1].Position = new Vector2(-40, itemIcons[i - 1].Position.Y);
				}
				if (curItem == 2 && itemIcons[i - 1].Visible)
				{
					itemIcons[i - 1].Position = new Vector2(40, itemIcons[i - 1].Position.Y);
				}
			}
		}
		if (itemCount == 3)
		{
			// 3 icons
			for (int i = 1; i <= itemIcons.Length; i++)
			{
				if (itemIcons[i - 1].Visible) // if icon visibility==true
					curItem++;
				if (curItem == 1 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(-68, itemIcons[i - 1].Position.Y);
				if (curItem == 2 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(0, itemIcons[i - 1].Position.Y);
				if (curItem == 3 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(68, itemIcons[i - 1].Position.Y);
			}
		}

		if (itemCount == 4)
		{
			// 4 icons
			for (int i = 1; i <= itemIcons.Length; i++)
			{
				if (itemIcons[i - 1].Visible) // if icon visibility==true
					curItem++;
				if (curItem == 1 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(-106, itemIcons[i - 1].Position.Y);
				if (curItem == 2 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(-40, itemIcons[i - 1].Position.Y);
				if (curItem == 3 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(40, itemIcons[i - 1].Position.Y);
				if (curItem == 4 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(106, itemIcons[i - 1].Position.Y);
			}
		}

		if (itemCount == 5)
		{
			// 5 icons
			for (int i = 1; i <= itemIcons.Length; i++)
			{
				if (itemIcons[i - 1].Visible) // if icon visibility==true
					curItem++;
				if (curItem == 1 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(-136, itemIcons[i - 1].Position.Y);
				if (curItem == 2 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(-68, itemIcons[i - 1].Position.Y);
				if (curItem == 3 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(0, itemIcons[i - 1].Position.Y);
				if (curItem == 4 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(68, itemIcons[i - 1].Position.Y);
				if (curItem == 5 && itemIcons[i - 1].Visible)
					itemIcons[i - 1].Position = new Vector2(136, itemIcons[i - 1].Position.Y);
			}
		}

	}

	public void PlayAttackAnim()
	{
		//Debug.Print("Play attack anim");
		animatedSprite2D.Animation = "attack";
		animatedSprite2D.Play();
	}

	public void AnimationFinished()
	{
		if (Globals.playerAlive)
		{
			animatedSprite2D.Animation = "idle";
			animatedSprite2D.Play();
			//Debug.Print("Attack over");
		}
	}

	public void PlayDeathAnim()
	{
		//Debug.Print("Play attack anim");
		animatedSprite2D.Animation = "death";
		animatedSprite2D.Play();
		FadeToBlack();
		// hide mini map
		Node2D miniMap = (Node2D)GetNode(Globals.NodeMiniMap);
		miniMap.Visible = false;
		// hide GUI
		CanvasLayer gui = (CanvasLayer)GetNode(Globals.NodeGUI);
		gui.Visible = false;
		itemIconSpeed.Visible = false;
		itemIconAoE.Visible = false;
		itemIconAttackSpeed.Visible = false;
		itemIconDamage.Visible = false;
		itemIconShield.Visible = false;

		Globals.hpBar.Visible = false;

		GameOver();

	}

	private void FadeToBlack()
	{
		ZIndex = 1000;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Globals.black, "modulate:a", 1f, 3.0f);

	}

	private async void GameOver()
	{
		// wait 7 seconds
		await Task.Delay(TimeSpan.FromMilliseconds(7000));
		GetTree().ReloadCurrentScene();

		Node glN = GetNode(Globals.NodeGlobals);
		Globals g = (Globals)glN;
		g.ResetGame();
	}

	public void AddAttack(string wType, string eType)
	{
		//Debug.Print("Add attack: " + wType + " - " + eType);
		if (wType == "Swipe")
		{
			if (eType == "Energy")
			{
				newAttack = (Node2D)attackSceneSlash.Instantiate();
				AttackSlash newAtk = (AttackSlash)newAttack;
				AddChild(newAtk);
				atkSlashEnergy.Add(newAtk);
				newAttack.Call("SetWeaponElement", "energy");
			}
			if (eType == "Ice")
			{
				newAttack = (Node2D)attackSceneSlash.Instantiate();
				AttackSlash newAtk = (AttackSlash)newAttack;
				AddChild(newAtk);
				atkSlashIce.Add(newAtk);
				newAttack.Call("SetWeaponElement", "ice");
			}
			if (eType == "Leech")
			{
				newAttack = (Node2D)attackSceneSlash.Instantiate();
				AttackSlash newAtk = (AttackSlash)newAttack;
				AddChild(newAtk);
				atkSlashLeeches.Add(newAtk);
				newAttack.Call("SetWeaponElement", "leeches");
			}
		}
		if (wType == "Projectile")
		{
			if (eType == "Energy")
			{
				newAttack = (Node2D)attackSceneProjectile.Instantiate();
				AttackRange newAtk = (AttackRange)newAttack;
				AddChild(newAtk);
				atkProjectileEnergy.Add(newAtk);
				newAttack.Call("SetWeaponElement", "energy");
			}
			if (eType == "Poison")
			{
				newAttack = (Node2D)attackSceneProjectile.Instantiate();
				AttackRange newAtk = (AttackRange)newAttack;
				AddChild(newAtk);
				atkProjectilePoison.Add(newAtk);
				newAttack.Call("SetWeaponElement", "poison");
			}
			if (eType == "Leech")
			{
				newAttack = (Node2D)attackSceneProjectile.Instantiate();
				AttackRange newAtk = (AttackRange)newAttack;
				AddChild(newAtk);
				atkProjectileLeeches.Add(newAtk);
				newAttack.Call("SetWeaponElement", "leeches");
			}
		}
		if (wType == "Cross")
		{
			if (eType == "Fire")
			{
				newAttack = (Node2D)attackSceneCross.Instantiate();
				AttackCross newAtk = (AttackCross)newAttack;
				AddChild(newAtk);
				atkCrossFire.Add(newAtk);
				newAttack.Call("SetWeaponElement", "fire");
			}
			if (eType == "Ice")
			{
				newAttack = (Node2D)attackSceneCross.Instantiate();
				AttackCross newAtk = (AttackCross)newAttack;
				AddChild(newAtk);
				atkCrossIce.Add(newAtk);
				newAttack.Call("SetWeaponElement", "ice");
			}
			if (eType == "Leech")
			{
				newAttack = (Node2D)attackSceneCross.Instantiate();
				AttackCross newAtk = (AttackCross)newAttack;
				AddChild(newAtk);
				atkCrossLeeches.Add(newAtk);
				newAttack.Call("SetWeaponElement", "leeches");
			}
		}
	}

}
