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
            QueueFree();
        }
    }

    public void _OnBodyEntered(Node body)
    {
        QueueFree();
        ExplodeBullet();
        if (body.HasMethod("take_damage"))
        {
            body.Call("take_damage",damage);
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
