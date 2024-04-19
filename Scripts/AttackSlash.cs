using Godot;
using System;

public partial class AttackSlash : Area2D
{
    private player ps;
    public float damage;
    public float AOE;
    public float attackSpeed;
    private int dmgLevel = 1;
    private int AOELevel = 1;
    private int attchSpeedLevel = 1;
    private float dmgInc = 1.2f;
    private float AOEInc = 1.2f;
    private float attackSpeedInc = .9f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        UpdateAttributes();
	}

    private void UpdateAttributes()
    {
        damage = dmgLevel * dmgLevel * dmgInc;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{



	}

    public void Shoot()
    {
        ps = (player)Globals.pl;      
        // play player attack anim
        ps.PlayAttackAnim();
        AnimatedSprite2D aSpr = (AnimatedSprite2D)GetNode("Slash");
        aSpr.Play("default");
        // set propper flip acording to player
        aSpr.FlipH= ps.animatedSprite2D.FlipH;

        // set collisionShape flip with player
        if (aSpr.FlipH==false)
        {
            GetNode<CollisionShape2D>("SlashArea2D/CollisionShape2D").Position= new Vector2(80,-9);
        }
        else
        {
            GetNode<CollisionShape2D>("SlashArea2D/CollisionShape2D").Position = new Vector2(-80,-9);
        }
    }

    public void OnBulletTimerTimeout()
    {
        if (Globals.playerAlive)
        {
            Shoot();
        }
    }

    public float GetDamage()
    {
        UpdateAttributes();
        return damage;
    }

}
