using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static AttackRange;
using System.Xml.Linq;
using static Godot.TextServer;

public partial class Golem : RigidBody2D
{
    private AnimatedSprite2D animatedSprite2D;
    private Area2D AttackProjectile;
    private PackedScene bulletScene;
    public float checkDistanceFreq = 500; // in milliseconds
    public float ThrowFreq = 5000; // in milliseconds
    private float dist;
    private float minDistance = 700;
    private float runDistance = 1000;
    private float walkToDistance = 400;
    public string mode = "stand";
    public float moveSpeed = 50f;
    public float poisonSpeed = 1f;
    Vector2 velocity;
    private float damage = 1;
    public bool throwing = false;
    private Node2D targetEnemy;
    private Vector2 targetEnemyPos;
    Godot.Collections.Array<Node2D> enemiesInRange;

    [Export] private AudioStreamPlayer sndFootStep;
    [Export] private AudioStreamPlayer sndGolemDeathp;
    private Area2D HitByEnemyArea;
    private ProgressBar HPBar;

    public float maxHP = 500;
    public float HP;
    public bool isDead = false;

    [Export] public AnimatedSprite2D bird;

    private List<enemy> enemies;
    private float damageFreq = 2; // how often golem gets damaged by enemies in his vicinity

    public override void _Ready()
	{
        enemies = new List<enemy>();
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Animation = "Idle";
        animatedSprite2D.Play();

        AttackProjectile = GetNode<Area2D>("AttackProjectile");
        enemiesInRange = new Godot.Collections.Array<Node2D>();

        HitByEnemyArea = GetNode<Area2D>("HitByEnemyArea");

        HPBar= GetNode<ProgressBar>("HPBar");
        HPBar.MaxValue = maxHP;
        HP = maxHP;
        Debug.Print("ready");
        HPBar.Value = HP;

        bird.Play("Idle");
        //Debug.Print("play idle");

        CheckDistanceToPlayer();
        ThrowProjectile();
        DoDamage(); // recursive function to damage golem if enemies are up in his graw

    }

    public async void SetGolemLevel(int level)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200)); // await so this is called after ready func

        Debug.Print("Golem level:" + level);

        if (level==1)
        {
            ThrowFreq = 5000;
            damage = 1;
            maxHP = 400;
            HP = maxHP;
            HPBar.MaxValue = maxHP;
            HPBar.Value = HP;
            animatedSprite2D.Modulate = new Color(1, 1, 1, 1);
        }
        if (level == 2)
        {
            ThrowFreq = 4000;
            damage = 3;
            maxHP = 1100;
            HP = maxHP;
            HPBar.MaxValue = maxHP;
            HPBar.Value = HP;
            animatedSprite2D.Modulate = new Color(.5f, .5f, 1, 1);
        }
        if (level == 3)
        {
            ThrowFreq = 2000;
            damage = 9;
            maxHP = 3000;
            HPBar.MaxValue = maxHP;
            HP = maxHP;
            HPBar.Value = HP;
            animatedSprite2D.Modulate = new Color(1, .5f, .5f, 1);
        }
    }


	public override void _Process(double delta)
	{
        if (mode=="walk")
        {
            if (dist > walkToDistance)
            {
                Vector2 direction = GlobalTransform.Origin.DirectionTo(Globals.ps.GlobalPosition);
                velocity = direction * moveSpeed * poisonSpeed;

                if (direction.X < 0) // if facing left
                {
                    animatedSprite2D.FlipH = true;
                    if (bird != null)
                    {
                        bird.FlipH = true;
                        bird.Position = new Vector2(34, bird.Position.Y);
                    }
                }
                else
                {
                    animatedSprite2D.FlipH = false;
                    if (bird != null)
                    {
                        bird.FlipH = false;
                        bird.Position = new Vector2(-64, bird.Position.Y);
                    }
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
                velocity = direction * moveSpeed * poisonSpeed*3;

                if (direction.X < 0) // if facing left
                {
                    animatedSprite2D.FlipH = true;
                }
                else
                    animatedSprite2D.FlipH = false;

                Position += velocity * (float)delta;
            }
        }

        // sync bird frames with golem
        if (bird!=null)
        {
            if (animatedSprite2D.Animation == "Idle" && bird.Animation == "Idle" && bird != null)
            {
                bird.Frame = animatedSprite2D.Frame;
                //Debug.Print("frame:" + animatedSprite2D.Frame + " bird:" + bird.Frame);
            }
        }


    }

    private async void CheckDistanceToPlayer()
    {
        if (IsInstanceValid(Globals.ps) && IsInstanceValid(this))
            dist = GlobalPosition.DistanceTo(Globals.ps.GlobalPosition);
        if (mode=="stand" || mode=="run")
        {
            if (dist > minDistance && dist<=runDistance)
            {
                mode = "walk";
                animatedSprite2D.Animation = "Walk";
                animatedSprite2D.Play();
                animatedSprite2D.SpeedScale = 2;
                //Debug.Print("mode = walk");
                bird.Play("FlyUp");
                //Debug.Print("FlyUp");
                //Debug.Print("bird mode: walk");
            }
        }

        if (dist > runDistance && mode!="dead" && mode=="walk")
        {
            mode = "run";
            animatedSprite2D.Animation = "Walk";
            animatedSprite2D.Play();
            animatedSprite2D.SpeedScale = 5;
            //Debug.Print("mode = run");
            bird.Play("FlyUp");
            //Debug.Print("FlyUp");
        }

        if (dist < minDistance && mode!="throw")
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

    public void OnBodyEntered(Node2D body) // enemy runs into golem small col - both enemy and golem take damage
    {
        //Debug.Print("OnBodyEntered: " + col.Name);
        var enemiesClose = HitByEnemyArea.GetOverlappingBodies(); // col mask will only detect enemies on layer 2

        enemy en;
        AgroGolem ag;
        int dmg;

        // if type is enemy
        if (body.GetType() == typeof(enemy))
        {
            en = (enemy)body;
            take_damage(en.damage);

            // dmaage enemy
            en.take_damage(damage);

            // add enemy to list so it can repeatably do damage
            enemies.Add(en);
            }
    }

    private void CheckIfDead()
    {
        if (HP < 1 && mode != "dead")
        {
            mode = "dead";
            animatedSprite2D.Animation = "Death";
            animatedSprite2D.Play();
            bird.Play("FlyUp");
            //Debug.Print("FlyUp");
            // play golem death sound
            sndGolemDeathp.Play();
            HPBar.Visible = false;
            DeleteGolem();
            Globals.golemAlive = false;
        }
    }
    public void OnBodyExited(Node2D body)
    {
        if (body.GetType() == typeof(enemy))
        {
            enemy en = (enemy)body;
            enemies.Remove(en);
        }
    }

    private async void DoDamage() // damage friendly golem and enemy who ran into him
    {
        await Task.Delay(TimeSpan.FromMilliseconds(damageFreq*1000));
        if (enemies.Count > 0)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemy en = enemies[i];
                take_damage(en.damage);

                // dmaage enemy
                en.take_damage(damage);
            }
        }
        DoDamage();// call itself
    }

    public void OnAttackProjectileAreaEntered(Node2D col) // detects when enemies in shooting range (used in throw)
    {
        enemiesInRange.Add(col);
    }

    public void OnAttackProjectileAreaExited(Node2D col) // removes enemies when they leave shooting range
    {
        enemiesInRange.Remove(col);
    }


    private async void DeleteGolem()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(6000));
        QueueFree();
    }

    private async void ThrowProjectile()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(ThrowFreq + GD.RandRange(0, 400))); // TODO: this will change with golem upgrade attack speed
        if (mode == "stand")
        {
            Debug.Print("check Throw");
            throwing = false; // reset 
                              // pick random enemy
                              //List <Node2D> 


            if (enemiesInRange!=null)
            {
                Debug.Print("not null");
                if (enemiesInRange.Count > 0)
                {
                    Debug.Print(">0");

                    mode = "throw";
                    // play golem attack anim
                    animatedSprite2D.Animation = "Throw";
                    animatedSprite2D.Play();
                    targetEnemy = enemiesInRange[0];
                    targetEnemyPos = targetEnemy.GlobalPosition;
                    if (bird != null)
                        if (bird.Animation == "Idle")
                            bird.Play("FlyUp");


                    if (targetEnemy.GlobalPosition.X < Position.X) // if facing left
                    {
                        animatedSprite2D.FlipH = true;
                        bird.FlipH = true;
                        bird.Position = new Vector2(34, bird.Position.Y);
                    }
                    else
                    {
                        animatedSprite2D.FlipH = false;
                        bird.FlipH = false;
                        bird.Position = new Vector2(-64, bird.Position.Y);
                    }

                    //Debug.Print("throw");
                    //Debug.Print("TargetEnemy:" + targetEnemy + " pos:" + targetEnemy.Position);
                }
            }
            
        }
        ThrowProjectile();
    }

    public void BirdAnimDone()
    {
        //Debug.Print("Bird anim done:" + bird.Animation);
        if (bird.Animation=="FlyUp")
        {
            CheckLand();
        }
        else
            if (bird.Animation == "Land")
            {
                if (animatedSprite2D.Animation == "Idle")
                    bird.Animation = "Idle";
                else
                    bird.Animation = "FlyUp";
            }
            else
                if (bird.Animation == "Hover")
                {
                    CheckLand();
                }
    }

    private async void CheckLand()
    {
        //Debug.Print("CheckLand:" + mode+ " bird.Animation="+ bird.Animation);
        if (IsInstanceValid(bird))
        {
            if (mode == "stand")
            {
                bird.Play("Land");
                Debug.Print("Land");
            }
            else
            {
                bird.Play("Hover");
                await Task.Delay(TimeSpan.FromMilliseconds(350));
                CheckLand();
            }
        }

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
    }

    public void FrameChanged() // triggered by signal
    {
        if (animatedSprite2D.Frame == 15 && throwing==false && animatedSprite2D.Animation == "Throw")
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
            //Debug.Print("targetEnemy:" + targetEnemy.GetParent<RigidBody2D>().Name);


            if (IsInstanceValid(targetEnemy))
            {
                bScript.damage = damage;
            }

            bScript.range = 1200;
            bScript.SetTarget(targetEnemyPos,true);
        }

        if (animatedSprite2D.Animation == "Walk")
        {
            if (animatedSprite2D.Frame == 2 || animatedSprite2D.Frame == 15)
            {
                Globals.PlayRandomizedSound(sndFootStep);
            }
        }

    }

    public void take_damage(float dmg)
    {
        if (mode != "dead")
        {
            HP -= dmg;
            DamageBlink();

            HPBar.Value = HP;
            CheckIfDead();
        }
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

}
