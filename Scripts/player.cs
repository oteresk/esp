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
	public AnimatedSprite2D animatedSprite2DTop;

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
	private PackedScene attackSceneOrbit;

	public List<AttackSlash> atkSlashEnergy;
	public List<AttackSlash> atkSlashIce;
	public List<AttackSlash> atkSlashLeeches;
	public List<AttackRange> atkProjectileEnergy;
	public List<AttackRange> atkProjectilePoison;
	public List<AttackRange> atkProjectileLeeches;
	public List<AttackCross> atkCrossFire;
	public List<AttackCross> atkCrossIce;
	public List<AttackCross> atkCrossLeeches;
	public List<AttackOrbit> atkOrbitEnergy;
	public List<AttackOrbit> atkOrbitPoison;
	public List<AttackOrbit> atkOrbitFire;


	[Export] public AudioStreamPlayer sndHurtPlayer;
	[Export] public AudioStreamPlayer sndHurtPlayer2;
	[Export] public AudioStreamPlayer sndDash;
	[Export] public AudioStreamPlayer sndPlayerDeath;
	[Export] public AudioStreamPlayer sndWarp;
	[Export] public AudioStreamPlayer sndRingingLoop;
	[Export] public AudioStreamPlayer sndGainLevel;

	private List<enemy> enemies;
	private float dmgFreq = 1000; // how often in ms an enemy damages a play when it stays in his collider

	[Export] public Sprite2D wavy;
	private ShaderMaterial matWavy;

	[Export] public WorldEnvironment worldEnvironment;
	[Export] public DirectionalLight2D ambientLight;

	[Export] public Label lblTime;
	[Export] public TextureButton btnBack;

	private bool OnFire = false;
	private float curFireTime;
	private string curAnim;

	[Export] public ColorRect GodRays;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// set player in Globals
		Globals.pl = (Area2D)GetNode(Globals.NodePlayer);
		Globals.ps = (player)Globals.pl;

		enemies = new List<enemy>();
		DamagePlayer(); // this will call itself recursively

		ScreenSize = GetViewportRect().Size;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite2D.Animation = "idle";
		animatedSprite2D.Play();
		curAnim = "idle";
		animatedSprite2D.FlipV = false;

		animatedSprite2DTop = GetNode<AnimatedSprite2D>("AnimatedSprite2DTop");
		animatedSprite2DTop.Animation = "idle";
		animatedSprite2DTop.Play();
		animatedSprite2DTop.FlipV = false;

		SetMagnetismShape();

		matWavy = (ShaderMaterial)wavy.Material;

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
		attackSceneOrbit = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/attack_Orbit.tscn");


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
		atkOrbitEnergy = new List<AttackOrbit>();
		atkOrbitPoison = new List<AttackOrbit>();
		atkOrbitFire = new List<AttackOrbit>(); ;



		// add default attack
		
		newAttack = (Node2D)attackSceneSlash.Instantiate();
		AttackSlash newAttackSlash = (AttackSlash)newAttack;
		AddChild(newAttackSlash);
		atkSlashEnergy.Add(newAttackSlash);
		newAttack.Call("SetWeaponElement", "energy");

		

		//*/

		/*
		newAttack = (Node2D)attackSceneProjectile.Instantiate();
		AttackRange newAttackProjectile = (AttackRange)newAttack;
		AddChild(newAttackProjectile);
		atkProjectileEnergy.Add(newAttackProjectile);
		newAttack.Call("SetWeaponElement", "energy");
		*/
		/*
		// orbit attack
		newAttack = (Node2D)attackSceneOrbit.Instantiate();
		AttackOrbit newAttackOrbit = (AttackOrbit)newAttack;
		AddChild(newAttackOrbit);
		atkOrbitEnergy.Add(newAttackOrbit);
		newAttack.Call("SetWeaponElement", "energy");
		*/

		
	}

	public void SetMagnetismShape()
	{
		// set magnetism size
		if (IsInstanceValid(magnetismShape))
			magnetismShape.Scale = new Vector2(Globals.magnetism, Globals.magnetism);
	}

	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseMotion eventMouseMotion && Globals.playerAlive)
		{
			//GD.Print("Mouse Motion at: ", eventMouseMotion.Position);
			// >960 == right
			// if using variable resolution, use GetViewport().GetVisibleRect().Size.X\2
			if (eventMouseMotion.Position.X> Globals.screenWidth/2)
			{
				animatedSprite2D.FlipH = false;
				animatedSprite2D.Offset = new Vector2(0, 0);
				animatedSprite2DTop.FlipH = false;
				animatedSprite2DTop.Offset = new Vector2(0, 0);
				SetAttackFlips();
			}
			else
			{
				animatedSprite2D.FlipH = true;
				animatedSprite2D.Offset = new Vector2(-100, 0);
				animatedSprite2DTop.FlipH = true;
				animatedSprite2DTop.Offset = new Vector2(-100, 0);
				SetAttackFlips();
			}

		}



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

			// gamepad face left or right
			if (Input.IsActionPressed("FaceLeft") && Globals.playerAlive)
			{
				animatedSprite2D.FlipH = true;
				animatedSprite2D.Offset = new Vector2(-100, 0);
				animatedSprite2DTop.FlipH = true;
				animatedSprite2DTop.Offset = new Vector2(-100, 0);
				SetAttackFlips();
			}
			else
				if (Input.IsActionPressed("FaceRight") && Globals.playerAlive)
			{
				animatedSprite2D.FlipH = false;
				animatedSprite2D.Offset = new Vector2(0, 0);
				animatedSprite2DTop.FlipH = false;
				animatedSprite2DTop.Offset = new Vector2(0, 0);
				SetAttackFlips();
			}


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
				Globals.PlayRandomizedSound(sndDash);
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
			{ // TODO: efficiency - precalc this
			  //Debug.Print("Player pos:" + Position);
				velocity *= (baseSpeed * speedMultiplier * poisonSpeed) + (Globals.speedLevel * speedInc) + (Globals.statMovementSpeed * speedInc);
			}

			// Moving the character around the screen
			Position += velocity * (float)delta;
			// Position = new Vector2( x: Position.X, y: Position.Y);


			// Setting the animations for the character
			if (velocity.X != 0)
			{
				if (isDashing && curAnim != "attack")
				{
					curAnim = "dash";
				}
				else
				{
					if (curAnim != "attack")
					{
						curAnim = "walk";
						// flip is now set in _input
					}
				}
			}
			else if (velocity.Y != 0)
			{
				if (isDashing && curAnim != "attack")
				{
					curAnim = "dash";
				}

				else
				{
					if (curAnim != "attack")
					{
						curAnim = "walk";
					}
				}
			}
			else
			{
				if (curAnim != "attack")
				{
					curAnim = "idle";
				}
			}
			if (!OnFire)
			{
				animatedSprite2D.Animation = curAnim;
				animatedSprite2DTop.Animation = curAnim;
			}
			else
			{
				if (curAnim != "dash")
				{
					animatedSprite2D.Animation = curAnim + "_flame";
					animatedSprite2DTop.Animation = curAnim;
				}	
			}
		}

		// check if out of bounds
		CheckOutOfBounds();

	}

	private void CheckOutOfBounds()
	{
		if (Position.X>28800 || Position.X<-28800 || Position.Y>16200 || Position.Y<-16200)
		{
			wavy.Visible = true;
			wavy.Position = Position;
			float dist = 0;
			float distX = (Mathf.Abs(Position.X) - 28800);
			if (distX > 0)
				dist += distX;
			float distY=(Mathf.Abs(Position.Y) - 16200);
			if (distY > 0)
				dist += distY;

			// mult = .01 to .04
			matWavy.SetShaderParameter("mult", dist/100000);
			// alpha .1 to 10
			// dist 0 to 5000
			matWavy.SetShaderParameter("alpha", dist / 500);

			// .75-2
			worldEnvironment.Environment.GlowIntensity = dist/2500+.75f;
			// 0 -1.5
			ambientLight.Energy = dist / 3400;

			// play rining loop sound
			if (!sndRingingLoop.Playing)
				sndRingingLoop.Play();
			// volume -20 to 0
			sndRingingLoop.VolumeDb = (float)(dist / 250) - 20;

			if (dist>5000) // if too far then teleport back to center
			{
				Position = new Vector2(0, 0);
				worldEnvironment.Environment.GlowIntensity = .75f;
				ambientLight.Energy = 0;
				matWavy.SetShaderParameter("mult", 0);
				matWavy.SetShaderParameter("alpha", 0);
				sndRingingLoop.Stop();
				sndWarp.Play();
			}
			//Debug.Print("Player pos:" + Position + "dist:" + dist+" vol:"+ sndRingingLoop.VolumeDb);
		}
		else
		{
			wavy.Visible = false;
		}
	}

	private void SetAttackFlips() // set the flip of all attacks that are children of Player to be same flip
	{
		Godot.Collections.Array<Node> attacks=GetChildren(false);
		foreach(Node attack in attacks)
		{
			if (attack.GetType() == typeof(AttackSlash))
			{
				AttackSlash aSlash = (AttackSlash)attack;
				aSlash.FlipAttack();
			}  
		}
	}

	public void MagnetismEntered(Area2D area)
	{
		if (area.IsInGroup("Pickups"))
		{
			ItemScript gs = (ItemScript)area;
			if (gs != null)
			{
				gs.collected = true; // start magnetism 
			}
		}

		if (area.IsInGroup("Relics"))
		{
			Relic gs = (Relic)area;
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

	public void SetAllAoE()
	{
		if (atkSlashEnergy.Count > 0)
			atkSlashEnergy[0].SetAOE();
		if (atkSlashIce.Count > 0)
			atkSlashIce[0].SetAOE();
		if (atkSlashLeeches.Count > 0)
			atkSlashLeeches[0].SetAOE();
		if (atkProjectileEnergy.Count > 0)
			atkProjectileEnergy[0].SetAOE();
		if (atkProjectilePoison.Count > 0)
			atkProjectilePoison[0].SetAOE();
		if (atkProjectileLeeches.Count > 0)
			atkProjectileLeeches[0].SetAOE();
		if (atkCrossFire.Count > 0)
			atkCrossFire[0].SetAOE();
		if (atkCrossIce.Count > 0)
			atkCrossIce[0].SetAOE();
		if (atkCrossLeeches.Count > 0)
			atkCrossLeeches[0].SetAOE();
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
		curAnim = "attack";
		if (!OnFire)
		{
			animatedSprite2D.Animation = curAnim;
			animatedSprite2DTop.Animation = curAnim;
		}
		else
		{ 
			animatedSprite2D.Animation = curAnim + "_flame";
			animatedSprite2DTop.Animation = curAnim;
		}
		animatedSprite2D.Play();
		animatedSprite2DTop.Play();
	}

	public void AnimationFinished()
	{
		if (Globals.playerAlive)
		{
			curAnim = "idle";
			if (!OnFire)
			{
				animatedSprite2D.Animation = curAnim;
				animatedSprite2DTop.Animation = curAnim;
			}
			else
			{
				animatedSprite2D.Animation = curAnim + "_flame";
				animatedSprite2DTop.Animation = curAnim;
			}
				
			animatedSprite2D.Play();
			animatedSprite2DTop.Animation = animatedSprite2D.Animation;
			animatedSprite2DTop.Play();
			//Debug.Print("Attack over");
		}
	}

	public void PlayDeathAnim()
	{
		Globals.canUnPause = false;
		//Debug.Print("Play attack anim");
		animatedSprite2D.Animation = "death";
		animatedSprite2D.Play();
		animatedSprite2DTop.Animation = "death";
		animatedSprite2DTop.Play();
		FadeToBlack();
		// hide mini map
		CanvasLayer miniMap = (CanvasLayer)GetNode(Globals.NodeMiniMapCanvas);
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

		// hide orbits
		if (atkOrbitEnergy.Count > 0)
			atkOrbitEnergy[0].Visible = false;

		if (atkOrbitPoison.Count > 0)
			atkOrbitPoison[0].Visible = false;

		if (atkOrbitFire.Count > 0)
			atkOrbitFire[0].Visible = false;

		// Don't process settlement select GUI
		StructureSelect settle = (StructureSelect)GetNode(Globals.NodeStructureGUI);
		settle.ProcessMode = Godot.Node.ProcessModeEnum.Disabled;

		ResourceDiscoveries.enemyTimer.OneShot = true;

		GameOver();

	}

	private void FadeToBlack()
	{
		ZIndex = 1000;
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Globals.black, "modulate:a", 1f, 3.0f);
		Globals.btnBack.Visible = true;
		Globals.btnBack.Modulate = new Color(1, 1, 1, 0);
		tween.TweenProperty(Globals.btnBack, "modulate:a", 1f, 1.5f);
		
	}

	private async void GameOver()
	{
		Globals.PlayRandomizedSound(sndPlayerDeath);
		Debug.Print("Play game over sound");

		ShowTime();

		// wait 5 seconds
		await Task.Delay(TimeSpan.FromMilliseconds(4400));

		Globals.rootNode.GetTree().Paused = true;
		
		TextureButton backBtn = (TextureButton)GetNode("/root/World/CLbtnBack/ctlBack/TextureButton");
		backBtn.GrabFocus();
	}



	private void ShowTime()
	{
		lblTime.Visible = true;

		string strSeconds = ResourceDiscoveries.seconds.ToString();
		if (ResourceDiscoveries.seconds < 10)
			strSeconds = "0" + strSeconds;

		string strMinutes = ResourceDiscoveries.minutes.ToString();

		if (ResourceDiscoveries.minutes < 10)
			strMinutes = "0" + strMinutes;


		lblTime.Text = "You lasted " + strMinutes + ":" + strSeconds;

	}

	public void PlayHurtSound()
	{
		int snd = GD.RandRange(0, 1);
		if (snd == 0)
		{
			Globals.PlayRandomizedSound(sndHurtPlayer);
		}
		else
		{
			Globals.PlayRandomizedSound(sndHurtPlayer2);
		}
	}
	public void AddAttack(string wType, string eType)
	{
		//Debug.Print("Add attack: " + wType + " - " + eType);
		if (wType == "Slash")
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
		if (wType == "Orbit")
		{
			if (eType == "Energy")
			{
				newAttack = (Node2D)attackSceneOrbit.Instantiate();
				AttackOrbit newAtk = (AttackOrbit)newAttack;
				AddChild(newAtk);
				atkOrbitEnergy.Add(newAtk);
				newAttack.Call("SetWeaponElement", "energy");
			}
			if (eType == "Poison")
			{
				newAttack = (Node2D)attackSceneOrbit.Instantiate();
				AttackOrbit newAtk = (AttackOrbit)newAttack;
				AddChild(newAtk);
				atkOrbitPoison.Add(newAtk);
				newAttack.Call("SetWeaponElement", "poison");
			}
			if (eType == "Fire")
			{
				newAttack = (Node2D)attackSceneOrbit.Instantiate();
				AttackOrbit newAtk = (AttackOrbit)newAttack;
				AddChild(newAtk);
				atkOrbitFire.Add(newAtk);
				newAttack.Call("SetWeaponElement", "fire");
			}
		}
	}

	public void OnBodyEntered(Node body)
	{
		// if enemy
		if (body.IsInGroup("Enemies")) // enemy damages player
		{
			enemy en = (enemy)body;
			enemies.Add(en);

			if (en.damageType == enemy.DamageType.Normal)
			{
				Globals.DamagePlayer(en.damage);
			}
			else
				if (en.damageType == enemy.DamageType.Poison)
			{
				Globals.PoisonPlayer(en.damage, en.damageTime);
			}
			else
				if (en.damageType == enemy.DamageType.Fire)
				IgnitePlayer(en.damage, en.damageTime);

			// bounce back away from player
			Vector2 dir = GlobalTransform.Origin.DirectionTo(en.GlobalPosition);
			en.ApplyForce(dir * en.speed * .4f);

			// skeleton attack damage
			if (en.enemyName == "Skeleton")
				en.playerInDamageArea = true;

		}
	}

	private void IgnitePlayer(float dmg, float dmgTime)
	{
		OnFire = true;
		curFireTime = dmgTime;

		string anim = animatedSprite2D.Animation;

		if (anim=="attack")
			anim = "attack_flame";

		if (anim == "idle")
			anim = "idle_flame";

		if (anim == "walk")
			anim = "walk_flame";

		FlamePlayer(dmg, dmgTime);
	}

	private async void FlamePlayer(float dmg, float dmgTime)
	{
		await Task.Delay(TimeSpan.FromMilliseconds(1000));
		Globals.DamagePlayer(dmg);
		curFireTime--;
		if (curFireTime >0)
			FlamePlayer(dmg, dmgTime);
		else
		{ // end fire
			OnFire = false;
			string anim = animatedSprite2D.Animation;

			if (anim == "attack_flame")
				anim = "attack";

			if (anim == "idle_flame")
				anim = "idle";

			if (anim == "walk_flame")
				anim = "walk";
		}

	}

	public void OnBodyExited(Node body)
	{
		// if enemy
		if (body.IsInGroup("Enemies"))
		{
			enemy en = (enemy)body;
			enemies.Remove(en);
			if (en.enemyName == "Skeleton")
				en.playerInDamageArea = false;
		}
	}

	public void OnAreaEntered(Area2D area) // enemy enter player
	{

		if (area.IsInGroup("Pickups") || area.IsInGroup("Relics"))
		{
			MagnetismEntered(area);
		}
		if (area.IsInGroup("SlimeTrail"))
		{
			SlimeTrail st=area.GetParent<SlimeTrail>();
			st.DamagePlayer();
		}
		// occlusion for enemies
		if (area.IsInGroup("Occlusion"))
		{
			area.GetParent<RigidBody2D>().Visible = true;
		}
		if (area.IsInGroup("OcclusionTrail")) // slime trail
		{
			area.GetParent<AnimatedSprite2D>().Visible = true;
		}
	}

	public void OnAreaExited(Area2D area) // enemy exit
	{
		// occlusion for enemies
		if (area.IsInGroup("Occlusion"))
		{
			area.GetParent<RigidBody2D>().Visible = false;
		}
		if (area.IsInGroup("OcclusionTrail")) // slime trail
		{
			area.GetParent<AnimatedSprite2D>().Visible = false;
		}
	}

	private async void DamagePlayer()
	{
		await Task.Delay(TimeSpan.FromMilliseconds(dmgFreq));
		if (enemies.Count > 0 && Globals.rootNode.GetTree().Paused == false)
		{
			for (int i = 0; i < enemies.Count; i++)
			{
				enemy en = enemies[i];
				if (en.damageType == enemy.DamageType.Normal)
				{
					Globals.DamagePlayer(en.damage);
				}
				else
				if (en.damageType == enemy.DamageType.Poison)
				{
					Globals.PoisonPlayer(en.damage, en.damageTime);
				}
			}
		}
		DamagePlayer();
	}

	public async void GainLevel()
	{
		Debug.Print("God");
		Globals.PlayRandomizedSound(sndGainLevel);
		GodRays.Visible = true;
		ShaderMaterial godRaysMat = (ShaderMaterial)GodRays.Material;

		
		await Task.Delay(TimeSpan.FromMilliseconds(1100));
		GodRays.Visible = false;
		await Task.Delay(TimeSpan.FromMilliseconds(300));
		Globals.UpdateLevel();
		Globals.ShowUpgrades();
	}


}
