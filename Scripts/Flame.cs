using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Flame : AnimatedSprite2D
{
    public float flameTime;
    private float curTime;
    public float flameDamage;
    public enum FlameTarget { Player, Enemy }
    public FlameTarget fTarget;
    public Node2D enemy;
    public Vector2 enemyScale;
    public bool slowDown = true;
    public Vector2 scaleMod;
    public bool onFire = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scaleMod = new Vector2(1, 1);
        DelayedStart();
    }

    async void DelayedStart()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        if (fTarget == FlameTarget.Enemy)
        {
            if (IsInstanceValid(this))
            {
                Position = new Vector2(0, 0);
                Scale = Scale * scaleMod;
                // slow down enemy when poisoned
                if (slowDown)
                {
                    enemy enScript = (enemy)enemy;
                    enScript.poisonSpeed = .5f;
                }
            }
        }
    }

    // called every second during poison
    public void FlameOver() // not really over, just a second has passed
    {
        curTime++;

        //Debug.Print("poison time: " + (poisonTime - curTime));

        if (fTarget == FlameTarget.Player)
        {
            Globals.DamagePlayer(flameDamage);
        }
        else
        {
            enemy.Call("take_damage", flameDamage);
        }



        if (curTime >= flameTime) // see if poison is really over
        {
            if (fTarget == FlameTarget.Player)
                Globals.PoisonEnded();
            else
            {
                if (slowDown)
                {
                    // return enemy speed to normal when poison is over
                    enemy enScript = (enemy)enemy;
                    enScript.poisonSpeed = 1f;
                }
            }


            QueueFree();
        }


    }
}







