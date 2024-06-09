using Godot;
using System;
using System.Threading.Tasks;
using static Poison;

public partial class Fire : Area2D
{
    public float flameTime;
    public float fireTime;
    public float damage;
    private bool canCatchFire = true;

    private float curTime;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    // called every second during poison
    public void TimerOver() // not really over, just a second has passed
    {
        curTime++;

        if (curTime >= fireTime) // see if fire is really over
        {
            FadeFire();
        }
    }


    public void OnAreaEntered(Area2D area)
	{
        if (canCatchFire)
        {
            if (area.IsInGroup("Enemies") && Globals.playerAlive) // Enemy goes through flame
            {
                // instantiate flame object on enemy
                var flameScene2 = (PackedScene)ResourceLoader.Load("res://Scenes/Flame.tscn");
                var newFlame2 = (AnimatedSprite2D)flameScene2.Instantiate();
                Node2D fScene2 = (Node2D)newFlame2;
                Node2D area2D = (Node2D)area;
                fScene2.Scale = area2D.Scale;

                newFlame2.Play();
                area.AddChild(newFlame2);
                var fScript = (Flame)newFlame2;
                fScript.fTarget = Flame.FlameTarget.Enemy;
                fScript.flameTime = flameTime;
                fScript.flameDamage = damage;
                fScript.slowDown = false;
                fScript.scaleMod = new Vector2(.3f, .3f);
                fScript.enemy = (Node2D)area;
            }
        }

    }

    public async void FadeFire()
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, 2.0f);

        await Task.Delay(TimeSpan.FromMilliseconds(1000));
        // disable catching fire
        canCatchFire = false;
        await Task.Delay(TimeSpan.FromMilliseconds(1000));


        if (IsInstanceValid(this))
        {
            tween.Stop();
            tween.Kill();
            QueueFree();
        }
    }


}
