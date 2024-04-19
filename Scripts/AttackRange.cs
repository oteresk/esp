using Godot;
using System;
using System.Diagnostics;

// Projectile Attack
public partial class AttackRange : Area2D
{
    public enum BulletSource { Player, Tower }

    [Export] public BulletSource bSource;

    private player ps;
    public float damage;
    public float AOE;
    public float attackSpeed;
    private int dmgLevel = 2;
    private int AOELevel = 1;
    private int attchSpeedLevel = 1;
    private float dmgInc = 1.2f;
    private float AOEInc = 1.2f;
    private float attackSpeedInc = .9f;

    public override void _Ready()
    {
        UpdateAttributes();
    }

    private void UpdateAttributes()
    {
        damage = dmgLevel * dmgLevel * dmgInc;
    }

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
                BulletScript bScript = (BulletScript)newBullet;
                bScript.damage = GetDamage();
                bScript.bType = BulletScript.BulletType.Homing;
                bScript.range = 1200;

                //Debug.Print("dam: " + GetDamage());
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

    public float GetDamage()
    {
        UpdateAttributes();
        return damage;
    }

}
