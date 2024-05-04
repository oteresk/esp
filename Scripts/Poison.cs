using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Poison : AnimatedSprite2D
{
    public float poisonTime;
    private float curTime;
    public float poisonDamage;
    public enum PoisonTarget { Player, Enemy }
    public PoisonTarget pTarget;
    public Node2D enemy;
    public Vector2 enemyScale;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        DelayedStart();
    }

    async void DelayedStart()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        if (pTarget == PoisonTarget.Enemy)
        {
            if (IsInstanceValid(this))
            {
                Position = new Vector2(0, 0);
                // slow down enemy when poisoned
                enemy enScript = (enemy)enemy;
                enScript.poisonSpeed = .5f;
            }
        }
    }

	// called every second during poison
	public void PoisonOver() // not really over, just a second has passed
	{
        curTime++;

        //Debug.Print("poison time: " + (poisonTime - curTime));

        if (pTarget == PoisonTarget.Player)
        {
            Globals.DamagePlayer(poisonDamage);
        }
        else
        {
            enemy.Call("take_damage", poisonDamage);
        }



        if (curTime >= poisonTime) // see if poison is really over
        {
            if (pTarget == PoisonTarget.Player)
                Globals.PoisonEnded();
            else
            {
                // return enemy speed to normal when poison is over
                enemy enScript = (enemy)enemy;
                enScript.poisonSpeed = 1f;
            }


            QueueFree();
        }


    }
}
