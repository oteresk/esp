using Godot;
using System;

public partial class AttackRange : Area2D
{
    public enum BulletSource { Player, Tower }

    [Export] public BulletSource bSource;

    private player ps;

    public override void _PhysicsProcess(double delta)
    {
        var enemiesInRange = GetOverlappingBodies();
        if (enemiesInRange.Count > 0)
        {
            var targetEnemy = (Node2D)enemiesInRange[0];
            LookAt(targetEnemy.GlobalPosition);
        }
    }

    public void Shoot()
    {
        var bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet2.tscn");
        var newBullet = (Area2D)bulletScene.Instantiate();
        newBullet.GlobalPosition = GetNode<Node2D>("ShootingPoint").GlobalPosition;
        newBullet.GlobalRotation = GetNode<Node2D>("ShootingPoint").GlobalRotation;
        var bullets = GetNode("/root/World/Bullets");
        bullets.AddChild(newBullet);

        switch (bSource)
        {
            case BulletSource.Player:
                newBullet.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale = new Vector2(0.3f, 0.3f);
                newBullet.Modulate = new Color(0.2f, 0.2f, 1, 1);
                // play player attack anim
                ps = (player)Globals.pl;
                ps.PlayAttackAnim();
                break;
            case BulletSource.Tower:
                newBullet.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale = new Vector2(1, 1);
                newBullet.Modulate = new Color(1, 0.3f, 0.8f, 1);
                break;
        }
    }

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            var enemiesInRange = GetOverlappingBodies();
            if (enemiesInRange.Count > 0)
            {
                Shoot();
            }
        }
    }

}
