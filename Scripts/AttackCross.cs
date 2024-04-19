using Godot;
using System;
using System.Diagnostics;

public partial class AttackCross : Area2D
{
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

    public float GetDamage()
    {
        UpdateAttributes();
        return damage;
    }
    private void UpdateAttributes()
    {
        damage = dmgLevel * dmgLevel * dmgInc;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // play player attack anim
        ps = (player)Globals.pl;
        ps.PlayAttackAnim();

        var bulletScene = (PackedScene)ResourceLoader.Load("res://Scenes/bullet2.tscn");
        var bullet = new Godot.Collections.Array { (Area2D)bulletScene.Instantiate(), (Area2D)bulletScene.Instantiate(), (Area2D)bulletScene.Instantiate(), (Area2D)bulletScene.Instantiate() };
        //Debug.Print("Bullet count:" + bullet.Count);
        Area2D a2D;
        BulletScript bScript;
        var bullets = GetNode("/root/World/Bullets");

        for (int iter=0; iter<bullet.Count; iter++)
        {
            a2D=(Area2D)bullet[iter];

            bScript = (BulletScript)bullet[iter];
            bScript.damage = GetDamage();
            bScript.bType = BulletScript.BulletType.Straight;
            bScript.range = 600;

            a2D.GlobalPosition = GetNode<Node2D>("ShootingPoint").GlobalPosition;
            a2D.GlobalRotation = GetNode<Node2D>("ShootingPoint").GlobalRotation;
            bullets.AddChild(a2D);
            a2D.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Scale = new Vector2(0.3f, 0.3f);
            a2D.Modulate = new Color(0.2f, 0.2f, 1, 1);
            // set direction
            if (iter==0) // up
            {
                a2D.LookAt(Vector2.Up);
                bScript.direction = new Vector2(0, -1);
            }
            if (iter == 1) // down
            {
                a2D.LookAt(Vector2.Down);
                bScript.direction = new Vector2(0, 1);
            }
            if (iter == 2) // left
            {
                a2D.LookAt(Vector2.Left);
                bScript.direction = new Vector2(-1, 0);
            }
            if (iter == 3) // right
            {
                a2D.LookAt(Vector2.Right);
                bScript.direction = new Vector2(1, 0);
            }

        }

    }

}
