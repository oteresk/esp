using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class AgroGolem : RigidBody2D
{
    private AnimatedSprite2D animatedSprite2D;
    private Area2D AttackProjectile;
    private PackedScene bulletScene;
    public float checkDistanceFreq = 500; // in milliseconds
    public float ThrowFreq = 5000; // in milliseconds
    private float dist;
    private float minDistance = 600;
    private float runDistance = 1000;
    private float walkToDistance = 350;
    public string mode = "stand";
    public float moveSpeed = 60f;
    public float poisonSpeed = 1f;
    Vector2 velocity;
    private float damage = 60;
    public bool throwing = false;
    private Node2D targetEnemy;
    private Vector2 targetEnemyPos;
    [Export] private AudioStreamPlayer sndFootStep;
    [Export] private AudioStreamPlayer sndGolemDeathp;
    [Export] private AudioStreamPlayer sndPound1;
    [Export] private AudioStreamPlayer sndPound2;
    [Export] private AnimatedSprite2D shockwave;
    [Export] private Area2D poundArea;
    [Export] public bool canThrow = true;

    private bool inPoundArea = false;
    private bool pounded = false;

    private Area2D HitByEnemyArea;
    private ProgressBar HPBar;
    private Vector2 startPos;
    float poundDistance;
    float poundProgression;

    public bool isDead = false;

    public float maxHP = 200;
    public float HP;

    private Control nodWinMessage;

    public override void _Ready()
    {
        if (canThrow == false) // if small golem
        {
            maxHP = maxHP / 3;
        }

        nodWinMessage = (Control)GetNode(Globals.NodeGUIWinMessage);

        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Animation = "Idle";
        animatedSprite2D.Play();

        AttackProjectile = GetNode<Area2D>("AttackProjectile");

        HitByEnemyArea = GetNode<Area2D>("HitByEnemyArea");

        HPBar = GetNode<ProgressBar>("HPBar");
        HPBar.MaxValue = maxHP;
        HP = maxHP;
        HPBar.Value = HP;

        //Debug.Print("play idle");
        startPos = Position; // used to move towards player

        CheckDistanceToPlayer();
        ThrowProjectile(); // recurring function that with either throw a boulder, or do a pound

        mode = "walk";
        dist = GlobalPosition.DistanceTo(Globals.ps.GlobalPosition);
    }

    public override void _Process(double delta)
    {
        if (mode == "walk")
        {
            if (dist > walkToDistance)
            {
                Vector2 direction = GlobalTransform.Origin.DirectionTo(Globals.ps.GlobalPosition);
                velocity = direction * moveSpeed * poisonSpeed;

                if (direction.X < 0) // if facing left
                {
                    animatedSprite2D.FlipH = true;
                }
                else
                {
                    animatedSprite2D.FlipH = false;
                }

                Position += velocity * (float)delta;

            }
            else
            {
                mode = "stand";
                animatedSprite2D.Animation = "Idle";
                animatedSprite2D.Play();
                //Debug.Print("mode: stand");
            }
        }

        if (mode == "run")
        {
            if (dist > walkToDistance)
            {
                Vector2 direction = GlobalTransform.Origin.DirectionTo(Globals.ps.GlobalPosition);
                velocity = direction * moveSpeed * poisonSpeed * 3;

                if (direction.X < 0) // if facing left
                {
                    animatedSprite2D.FlipH = true;
                }
                else
                {
                    animatedSprite2D.FlipH = false;
                }

                Position += velocity * (float)delta;
            }
        }


    }

    private async void CheckDistanceToPlayer()
    {
        if (IsInstanceValid(Globals.ps) && IsInstanceValid(this))
            dist = GlobalPosition.DistanceTo(Globals.ps.GlobalPosition);
        if (mode == "stand" || mode == "run")
        {
            if (dist > minDistance && dist <= runDistance)
            {
                mode = "walk";
                animatedSprite2D.Animation = "Walk";
                animatedSprite2D.Play();
                animatedSprite2D.SpeedScale = 2;
            }
        }

        if (dist > runDistance && mode != "dead" && mode == "walk")
        {
            mode = "run";
            animatedSprite2D.Animation = "Walk";
            animatedSprite2D.Play();
            animatedSprite2D.SpeedScale = 5;
        }

        if (dist < minDistance && mode != "throw")
        {
            if (mode == "walk" || mode == "run")
            {
                mode = "stand";
                animatedSprite2D.Animation = "Idle";
                animatedSprite2D.Play();
            }
        }

        //Debug.Print("dist: " + dist);

        await Task.Delay(TimeSpan.FromMilliseconds(checkDistanceFreq));
        CheckDistanceToPlayer();
    }

    public void take_damage(float dmg)
    {
        if (mode!="dead")
        {
            HP -= dmg;
            DamageBlink();

            HPBar.Value = HP;

            if (HP < 1 && mode != "dead")
            {
                mode = "dead";
                animatedSprite2D.Animation = "Death";
                animatedSprite2D.Play();

                // play golem death sound
                sndGolemDeathp.Play();
                HPBar.Visible = false;
                DeleteGolem();

                if (!canThrow) // if small golem, then whenever one dies, another is born
                   Globals.agroGolemAlive = false;
            }

        }
    }

    public void OnPoundEntered(Node2D col) // golem pound player
    {
        if (col.Name == "Player")
        {
            inPoundArea = true;
            Debug.Print("In pound area");
        }
    }
    public void OnPoundExited(Node2D col) // golem pound player
    {
        if (col.Name == "Player")
        {
            inPoundArea = false;
            Debug.Print("Out of pound area");
        }
    }

    private async void DeleteGolem()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(7000));
        QueueFree();
    }
    private async void DamageBlink()
    {
        if (animatedSprite2D != null && !isDead)
        {
            ShaderMaterial enemyMat = (ShaderMaterial)animatedSprite2D.Material;
            if (enemyMat != null)
                enemyMat.SetShaderParameter("active", true);
            else
                Debug.Print("*** enemy.NotificationEnterCanvas DamageBlink shader not found on AnimatedSprite2D ***");
        }

        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        if (animatedSprite2D != null && !isDead)
        {
            if (IsInstanceValid(this))
            {
                ShaderMaterial enemyMat = (ShaderMaterial)animatedSprite2D.Material;
                if (enemyMat != null)
                    enemyMat.SetShaderParameter("active", false);
                else
                    Debug.Print("*** enemy.NotificationEnterCanvas DamageBlink shader not found on AnimateSprite2D ***");
            }

        }
    }
    private async void ThrowProjectile() // throw or pound
    {
        await Task.Delay(TimeSpan.FromMilliseconds(ThrowFreq+GD.RandRange(0,400))); // TODO: this will change with agrogolem attack speed
        if (mode == "stand" && Globals.rootNode.GetTree().Paused == false) // if paused then wait for unpause)
            {
            //Debug.Print("Throw");
            throwing = false; // reset
                              // 
            if (dist < minDistance)
            {
                int atk = GD.RandRange(0, 1);
                if (atk==0 || !canThrow) // pound
                {
                    sndPound1.Play();
                    animatedSprite2D.Animation = "Pound";
                    animatedSprite2D.Play();

                    startPos = Position; // used to move towards player
                    poundProgression = 0;
                    pounded = false;

                    if (GlobalPosition.X < Globals.ps.GlobalPosition.X)
                    {
                        targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X + 300 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
                        Debug.Print("<player: " + targetEnemyPos+" player:"+ Globals.ps.GlobalPosition.X+ "GlobalPosition.X:"+ GlobalPosition.X);
                        animatedSprite2D.FlipH = false;
                        if (!canThrow)
                            poundArea.Position = new Vector2(209, 0);
                        else
                            poundArea.Position = new Vector2(135, 0);
                    }
                    else
                    {
                        targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X - 300 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
                        Debug.Print(">player: " + targetEnemyPos + " player:" + Globals.ps.GlobalPosition.X+ " GlobalPosition.X:"+ GlobalPosition.X);
                        animatedSprite2D.FlipH = true;
                        if (!canThrow)
                            poundArea.Position = new Vector2(-126, 0);
                        else
                            poundArea.Position = new Vector2(-67, 0);
                    }

                        Debug.Print("pound dist:" + dist);
                }
                else // throw
                {
                    Debug.Print("Throw");
                    mode = "throw";
                    // play golem attack anim
                    animatedSprite2D.Animation = "Throw";
                    animatedSprite2D.Play();
                    targetEnemy = Globals.ps;
                    targetEnemyPos = targetEnemy.GlobalPosition;

                    if (targetEnemy.GlobalPosition.X < Position.X) // if facing left
                    {
                        animatedSprite2D.FlipH = true;
                    }
                    else
                    {
                        animatedSprite2D.FlipH = false;
                    }
                }
            }
        }

        ThrowProjectile();
    }

    public void OnAnimFinished()
    {
        if (mode != "dead")
        {
            animatedSprite2D.Animation = "Idle";
            animatedSprite2D.Play();
            mode = "stand";
            //Debug.Print("mode=stand");
        }
        else
        {
            if (canThrow == true) // if agro golem
            {
                // win game
                nodWinMessage.Visible = true;
                Globals.PauseGame();
            }
        }



    }

    public void FrameChanged() // triggered by signal
    {
        if (animatedSprite2D.Frame == 15 && throwing == false && animatedSprite2D.Animation == "Throw")
        {
            throwing = true;

            bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/GolemProjectile.tscn"); // default for tower

            var newBullet = (Area2D)bulletScene.Instantiate();

            // AOE
            newBullet.GlobalPosition = GetNode<Node2D>("AttackProjectile/ShootingPoint").GlobalPosition;
            //newBullet.GlobalRotation = GetNode<Node2D>("AttackProjectile/ShootingPoint").GlobalRotation;
            var bullets = GetNode("/root/World/Bullets");
            bullets.AddChild(newBullet);

            GolemProjectile bScript = (GolemProjectile)newBullet;
            bScript.bType = GolemProjectile.BulletType.Homing;
            bScript.element = "boulder";
            bScript.poisonTime = 0;
            bScript.damage = damage;
            bScript.range = 1200;
            bScript.SetTarget(targetEnemyPos,false);
        }

        if (animatedSprite2D.Animation == "Walk")
        {
            if (animatedSprite2D.Frame == 2 || animatedSprite2D.Frame == 15)
            {
                Globals.PlayRandomizedSound(sndFootStep);
            }
        }

        if (animatedSprite2D.Animation=="Pound" && animatedSprite2D.Frame>17 && animatedSprite2D.Frame<24&& canThrow) // pound for agro golem
        {
            AgroPound();
        }

        if (canThrow == false && animatedSprite2D.Animation == "Pound")
            SmallPound();


    }

    private void AgroPound()
    {

        if (animatedSprite2D.Frame == 18)
        {
            sndPound2.Play();
            if (Position.X < Globals.ps.GlobalPosition.X)
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X - 200 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
            else
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X + 200 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));

            poundArea.GlobalPosition = new Vector2(Globals.ps.Position.X, Globals.ps.Position.Y);
            // play shockwave
            shockwave.Play();
            // face player
            if (targetEnemyPos.X < Position.X) // if facing left
            {
                animatedSprite2D.FlipH = true;
                poundArea.Position = new Vector2(-126, 0);
            }
            else
            {
                animatedSprite2D.FlipH = false;
                poundArea.Position = new Vector2(209, 0);
            }
        }

        if (inPoundArea && Globals.playerAlive && pounded == false) // damage player
        {
            pounded = true;
            Debug.Print("Golem pound player:" + damage);
            Globals.DamagePlayer(damage);
        }

        poundProgression += .16f;
        Position = startPos.Lerp(targetEnemyPos, poundProgression);

        if (animatedSprite2D.Frame < 20)
        {
            if (Position.X < Globals.ps.GlobalPosition.X)
            {
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X - 180 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
                animatedSprite2D.FlipH = false;
                poundArea.Position = new Vector2(209, 0);
            }
            else
            {
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X + 180 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
                animatedSprite2D.FlipH = true;
                poundArea.Position = new Vector2(-126, 0);
            }

            //Debug.Print("pos: " + Position + "pd/sp:"+(poundDistance/.16f)+ " startPos:"+ startPos);
        }
    }

    private void SmallPound()
    {
        //Debug.Print("small pound: "+Position);
        if (animatedSprite2D.Frame == 28 || animatedSprite2D.Frame==16)
        {
            sndPound2.Play();
            /*
            if (Position.X < Globals.ps.GlobalPosition.X)
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X + 200 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
            else
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X - 200 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
            */
            //poundArea.GlobalPosition = new Vector2(Globals.ps.Position.X, Globals.ps.Position.Y);
            // play shockwave
            shockwave.Play();
            // face player
            if (targetEnemyPos.X < Position.X) // if facing left
            {
                animatedSprite2D.FlipH = true;
                poundArea.Position = new Vector2(-43, 0);
            }
            else
            {
                animatedSprite2D.FlipH = false;
                poundArea.Position = new Vector2(67, 0);
            }
        }

        if (inPoundArea && Globals.playerAlive && pounded == false) // damage player
        {
            pounded = true;
            Debug.Print("Golem pound player:" + damage);
            Globals.DamagePlayer(damage/3);
        }

        

        

        if (animatedSprite2D.Frame < 20)
        {
            poundProgression += .048f;
            if (poundProgression > .8f)
                poundProgression = .8f;
            if (Position.X < Globals.ps.GlobalPosition.X)
            {
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X - 180 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
                animatedSprite2D.FlipH = false;
                poundArea.Position = new Vector2(104, 0);
            }
            else
            {
                targetEnemyPos = new Vector2(Globals.ps.GlobalPosition.X + 180 + GD.RandRange(-0, 0), Globals.ps.GlobalPosition.Y + GD.RandRange(-0, 0));
                animatedSprite2D.FlipH = true;
                poundArea.Position = new Vector2(-63, 0);
            }
            Debug.Print("poundProgression:" + poundProgression);
            Position = startPos.Lerp(targetEnemyPos, poundProgression);
            //Debug.Print("pos: " + Position + "pd/sp:"+(poundDistance/.16f)+ " startPos:"+ startPos);
        }
    }

}
