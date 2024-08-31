using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class GolemProjectile : Area2D
{
    private float travelDist = 0;
    public float damage;
    public BulletType bType = BulletType.Homing;
    public Vector2 direction;
    public float range;
    const float SPEED = 1000;
    public string element;
    public float poisonTime;
    public float freezeTime;
    public float fireTime = 6; // how long the big fire lasts in world
    public float flameTime; // how long the flame on enemy lasts

    public Vector2 targetPos;
    public float dist;
    private float startDist;
    private bool targetSet = false;
    private float yOff;
    private float acceleration;
    private float gravity = .01f;
    private float projectileSpeed = 390; //higher=faster
    private bool isFriendly = true;

    Tween tween;
    Tween boulderTween;

    [Export] public Sprite2D shadow;
    [Export] public AnimatedSprite2D projectile;
    [Export] public AudioStreamPlayer2D sndWhoosh;
    [Export] public AudioStreamPlayer2D sndBoulderLandh;
    [Export] public AnimatedSprite2D explosion;
    [Export] public CollisionShape2D colShape;

    public enum BulletType
    {
        Homing, Straight, Flame
    }

    public override void _Ready()
    {
        Name = "Bullet";
        colShape.Disabled = true;
        /*
        AnimatedSprite2D aS2D = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (aS2D != null)
            aS2D.Play();
        */

        sndWhoosh.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        /*
        //LookAt(targetEnemy.GlobalPosition);
        //Debug.Print("Target enemy: " +targetEnemy+" "+ targetEnemy.GlobalPosition);
        //const float RANGE = 1200;

        if (bType == BulletType.Homing)
            direction = Vector2.Right.Rotated(Rotation);

        if (bType != BulletType.Flame)
        {
            AnimatedSprite2D aS2D = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
            if (aS2D != null) // regular animated projectile
            {
                if (GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D").Visible)
                    Position += direction * SPEED * (float)delta;
            }
            else
            { // golem projectile
                if (GetNodeOrNull<Sprite2D>("Shadow").Visible)
                {
                    Position += direction * SPEED * (float)delta;
                }
            }


            travelDist += SPEED * (float)delta;

            if (travelDist > range)
            {
                QueueFree();
            }
        }
        */


        dist= targetPos.DistanceTo(GlobalPosition);
        if (dist < .3f && projectile.Visible ==true)
            ExplodeBullet(0);

    }

    
    public void SetTarget(Vector2 tPos, bool isFriend)
    {
        isFriendly= isFriend;
        targetSet = true;
        targetPos = tPos;
        startDist = targetPos.DistanceTo(GlobalPosition);

        acceleration = 3.7f; // boulder up acceleration

        tween = GetTree().CreateTween();
        tween.Parallel().TweenProperty(this, "global_position", targetPos, startDist/projectileSpeed);
        tween.SetPauseMode(Tween.TweenPauseMode.Stop);

        boulderTween=projectile.GetTree().CreateTween();

        boulderTween.TweenProperty(projectile, "position",new Vector2(0,-400), startDist / projectileSpeed / 2).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
        boulderTween.TweenProperty(projectile, "position", new Vector2(0, 0), startDist / projectileSpeed / 2).SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
        boulderTween.SetPauseMode(Tween.TweenPauseMode.Stop);


        ExplodeBullet(startDist / projectileSpeed);

        //Debug.Print("Target Pos: " + targetPos+ " startDist:" + startDist);
    }


    private async void ExplodeBullet(float waitTime)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(waitTime*1000+20));

        if (IsInstanceValid(projectile))
            projectile.Visible = false;
        shadow.Visible = false;

        sndBoulderLandh.Play();

        explosion.Visible = true;
        explosion.Play();

        colShape.Visible = true;
        colShape.Disabled = false;
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        colShape.Visible = false;
        colShape.Disabled = true;

        tween.Kill();
        boulderTween.Kill();

        await Task.Delay(TimeSpan.FromMilliseconds(2000));

        if (IsInstanceValid(this))
            QueueFree();

    }

    public void _OnBodyEntered(Node2D body)
    {
        //Debug.Print("_OnBodyEntered body.Name:"+ body.Name+ " Globals.playerAlive:"+ Globals.playerAlive+ " Globals.ps.canBeDamaged:"+ Globals.ps.canBeDamaged);
        if (isFriendly) //damage enemies
        {
            //Debug.Print("friendly");
            if (body.HasMethod("take_damage"))
            {
                //Debug.Print("golem projectile hit: " + body.Name);
                if (body.Name == "AgroGolem")
                    body.Call("take_damage", damage * 10); // damage is * 10 if it's against agro golem
            }
        }
        else // damage player
        {
            if (body.Name == "Player" && Globals.playerAlive)
            {
                //Debug.Print("dmg: " + damage);
                Globals.DamagePlayer(damage);
            }
        }

        if (body.GetParent().Name== "FriendlyGolem" && !isFriendly) // check if hit friendly golem, but not if projectile was friendly
        { 
            Debug.Print("hurt friendly");
            body.GetParent<RigidBody2D>().Call("take_damage", damage); // damage is * 2 if it's against friendly golem
        }


    }


}
