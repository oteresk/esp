using Godot;
using System;
using System.Diagnostics;

public partial class BulletScript : Area2D
{
    private float travelDist = 0;
    public float damage;
    public BulletType bType = BulletType.Homing;
    public Vector2 direction;
    public float range;
    public float SPEED = 1000;
    public string element;
    public float poisonTime;
    public float freezeTime;
    public float fireTime=6; // how long the big fire lasts in world
    public float flameTime; // how long the flame on enemy lasts
    public Vector2 targetPos;

    public enum BulletType
    {
        Homing, Straight, Flame
    }

    public override void _Ready()
    {
        AnimatedSprite2D aS2D = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (aS2D!=null)
            aS2D.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        //const float RANGE = 1200;

        if (bType == BulletType.Homing)
           direction  = Vector2.Right.Rotated(Rotation);

        if (bType != BulletType.Flame)
        {
            AnimatedSprite2D aS2D = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
            if (aS2D!=null) // regular animated projectile
            {
                if (GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D").Visible)
                    Position += direction * SPEED * (float)delta;
            }
            else
            { // golem projectile
                if (GetNodeOrNull<Sprite2D>("Shadow").Visible)
                    Position += direction * SPEED * (float)delta;
            }


            travelDist += SPEED * (float)delta;

            if (travelDist > range)
            {
                //ExplodeBullet();
                QueueFree();
            }
        }
    }

    public void _OnBodyEntered(Node body)
    {
        if (body.HasMethod("take_damage"))
        {
            ExplodeBullet();
            body.Call("take_damage",damage);

            if (element=="poison") // if poison
            {
                // instantiate poison object on enemy
                var poisonScene = (PackedScene)ResourceLoader.Load("res://Scenes/poison.tscn");
                var newPoison = (AnimatedSprite2D)poisonScene.Instantiate();
                Node2D pScene = (Node2D)newPoison;
                Node2D body2D = (Node2D)body;
                pScene.Scale = body2D.Scale;

                newPoison.Play();
                body.AddChild(newPoison);
                var pScript = (Poison)newPoison;
                pScript.pTarget = Poison.PoisonTarget.Enemy;
                pScript.poisonTime = poisonTime;
                pScript.poisonDamage = damage;
                pScript.enemy=(Node2D)body;
            }

            if (element == "ice")
            {
                enemy en = (enemy)body;
                en.FreezeEnemy(freezeTime);
            }
            if (element == "fire")
            {
                // add flame to world landing spot
                var fireScene = (PackedScene)ResourceLoader.Load("res://Scenes/Fire.tscn");
                var newFire = (Area2D)fireScene.Instantiate();
                Node2D fScene = (Node2D)newFire;
                Node2D body2D = (Node2D)body;
                newFire.Position = body2D.Position;
                AnimatedSprite2D newFireAnim = (AnimatedSprite2D)newFire.GetNode("AnimatedSprite2D");
                newFireAnim.Play();
                newFire.Scale = new Vector2(3, 3);
                Fire fireScript = (Fire)newFire;
                fireScript.damage = damage;
                fireScript.flameTime = flameTime;
                fireScript.fireTime= fireTime;
                Node2D world = (Node2D)GetNode(Globals.NodeWorld);
                world.CallDeferred("add_child", newFire);


                // instantiate flame object on enemy
                if (!body.HasNode("Area2D/Flame")) // only create flame if one doesn't exist on enemy already
                {
                    var flameScene2 = (PackedScene)ResourceLoader.Load("res://Scenes/Flame.tscn");
                    var newFlame2 = (AnimatedSprite2D)flameScene2.Instantiate();
                    Node2D fScene2 = (Node2D)newFlame2;
                    fScene2.Scale = body2D.Scale;

                    newFlame2.Play();
                    body.AddChild(newFlame2);
                    var fScript = (Flame)newFlame2;
                    fScript.fTarget = Flame.FlameTarget.Enemy;
                    fScript.flameTime = flameTime;
                    fScript.flameDamage = damage;
                    fScript.slowDown = false;
                    fScript.scaleMod = new Vector2(.3f, .3f);
                    fScript.enemy = (Node2D)body;
                    fScript.onFire = true;

                }
            }


        }

    }

    public void ExplodeBullet()
    {
        var expBulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/ExplodeBullet.tscn");
        var newBullet = (Node2D)expBulletScene.Instantiate();
        GetParent().AddChild(newBullet);
        newBullet.Name = "Explosion";
        newBullet.Position = Position;
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Visible = false;
    }

}
