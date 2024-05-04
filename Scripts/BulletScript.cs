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
    const float SPEED = 1000;
    public string element;
    public float poisonTime;
    public float freezeTime;

    public enum BulletType
    {
        Homing, Straight
    }

    public override void _Ready()
    {
        Name = "Bullet";
    }

    public override void _PhysicsProcess(double delta)
    {
        
        //const float RANGE = 1200;

        if (bType == BulletType.Homing)
           direction  = Vector2.Right.Rotated(Rotation);

        if (GetNode<AnimatedSprite2D>("AnimatedSprite2D").Visible)
            Position += direction * SPEED * (float)delta;

        travelDist += SPEED * (float)delta;

        if (travelDist > range)
        {
            //ExplodeBullet();
            QueueFree();
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
