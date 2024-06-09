using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static AttackRange;
using System.Xml.Linq;

public partial class Golem : RigidBody2D
{
    private AnimatedSprite2D animatedSprite2D;
    private Area2D AttackProjectile;
    private PackedScene bulletScene;
    public float checkDistanceFreq = 500; // in milliseconds
    public float ThrowFreq = 5000; // in milliseconds
    private float dist;
    private float minDistance = 700;
    private float walkToDistance = 400;
    public string mode = "stand";
    public float moveSpeed = 50f;
    public float poisonSpeed = 1f;
    Vector2 velocity;
    private float damage = 1;
    public bool throwing = false;
    private Node2D targetEnemy;
    private Vector2 targetEnemyPos;
    [Export] private AudioStreamPlayer2D sndFootStep;
    [Export] private AudioStreamPlayer2D sndGolemDeathp;
    private Area2D HitByEnemyArea;
    private ProgressBar HPBar;

    public float maxHP = 4000;
    public float HP;

    public override void _Ready()
	{
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Animation = "Idle";
        animatedSprite2D.Play();

        AttackProjectile = GetNode<Area2D>("AttackProjectile");

        HitByEnemyArea = GetNode<Area2D>("HitByEnemyArea");

        HPBar= GetNode<ProgressBar>("HPBar");
        HPBar.MaxValue = maxHP;
        HP = maxHP;
        HPBar.Value = HP;

        CheckDistanceToPlayer();
        ThrowProjectile();
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
                }
                else
                    animatedSprite2D.FlipH = false;

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

	}

    private async void CheckDistanceToPlayer()
    {
        dist = GlobalPosition.DistanceTo(Globals.ps.GlobalPosition);
        if (mode=="stand")
        {
            if (dist > minDistance)
            {
                mode = "walk";
                animatedSprite2D.Animation = "Walk";
                animatedSprite2D.Play();
                //Debug.Print("mode: walk");
            }
        }

        //Debug.Print("dist: " + dist);

        await Task.Delay(TimeSpan.FromMilliseconds(checkDistanceFreq));
        CheckDistanceToPlayer();
    }

    public void OnBodyEntered(Node2D col) // hit Golem
    {

        var enemiesInRange = HitByEnemyArea.GetOverlappingBodies(); // col mask will only detect enemies on layer 2
        enemy en;
        int dmg;

        for (int iter=0; iter<enemiesInRange.Count; iter++)
        {
            en = (enemy)enemiesInRange[iter];
            dmg = en.damage;
            HP -= dmg;

            // dmaage enemy
            en.take_damage(1);

            //Debug.Print("Hit Golem: " + dmg);
        }



        

        HPBar.Value = HP;

        if (HP < 1 && mode!="dead")
        {
            mode = "dead";
            animatedSprite2D.Animation = "Death";
            animatedSprite2D.Play();
            // play golem death sound
            sndGolemDeathp.Play();
        }
        

    }

        private async void ThrowProjectile()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(ThrowFreq));
        if (mode == "stand")
        {
            //Debug.Print("Throw");
            throwing = false; // reset 
            // pick random enemy
            var enemiesInRange = AttackProjectile.GetOverlappingBodies();
            if (enemiesInRange.Count > 0)
            {
                mode = "throw";
                // play golem attack anim
                animatedSprite2D.Animation = "Throw";
                animatedSprite2D.Play();
                targetEnemy=enemiesInRange[0];
                targetEnemyPos = targetEnemy.GlobalPosition;
                Debug.Print("TargetEnemy:" + targetEnemy + " pos:" + targetEnemy.Position);
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
            bScript.damage = damage;
            bScript.range = 1200;
            bScript.SetTarget(targetEnemyPos);
        }

        if (animatedSprite2D.Animation == "Walk")
        {
            if (animatedSprite2D.Frame == 2 || animatedSprite2D.Frame == 15)
            {
                sndFootStep.Play();
            }
        }

    }
}
