using Godot;
using System;
using System.Diagnostics;

public partial class Poison : AnimatedSprite2D
{
    private player ps;
    public float poisonTime;
    private float curTime;
    public float poisonDamage;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        
	}

	// called every second during poison
	public void PoisonOver()
	{
        curTime++;

        Debug.Print("poison time: " + (poisonTime - curTime));

        Globals.DamagePlayer(poisonDamage);

        if (curTime >= poisonTime)
        {
            Globals.PoisonEnded();
            QueueFree();
        }


    }
}
