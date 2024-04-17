using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class SlimeTrail : AnimatedSprite2D
{
    [Export] public DamageType damageType;
    [Export] public int damage;
    [Export] public int damageTime;
    private int life = 3;
    private int curLife = 0;

    private bool hitPlayer = false;

    private double speed = .1f; // magnetism start speed

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (hitPlayer) 
        {
            Vector2 pos = Globals.pl.GlobalPosition + new Vector2(0, -50);
            GlobalPosition = GlobalPosition.MoveToward(pos, (float)speed);
            speed += 3 * delta;

            if (GlobalPosition.DistanceTo(Globals.pl.Position)<1)
            {
                QueueFree();
            }

        }

    }

    public enum DamageType
    {
        Normal, Poison
    }

    public void OnTimeOver()
    {
        curLife++;
        if (curLife >= life)
        {
            life = 9999;
            FadeOut(3);
        }
    }

    public void OnBodyEntered(Node2D col) // hit player
    {
        // only take damage if trail is more than half faded
        if (this.Modulate.A > .5f)
        {
            if (col.Name == "Player" && Globals.playerAlive)
            {
                if (damageType == DamageType.Normal)
                {
                    Globals.DamagePlayer(damage);
                }
                else
                    if (damageType == DamageType.Poison)
                {
                    Globals.PoisonPlayer(damage, damageTime);
                }
                Tween tween = GetTree().CreateTween();
                tween.Parallel().TweenProperty(this, "modulate:a", 0f, 1.5f);
                tween.Parallel().TweenProperty(this, "scale", new Vector2(0, 0), 1.5f);
                hitPlayer = true;
            }
        }
    }

    private void DeleteThis()
    {
        if (!this.IsQueuedForDeletion())
            QueueFree();
    }
    async void FadeOut(float fadeTime)
    {
        //Debug.Print("fadetime:" + fadeTime);
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, fadeTime);
        await Task.Delay(TimeSpan.FromMilliseconds(fadeTime * 1000));
        if (!this.IsQueuedForDeletion())
            QueueFree();
    }

}
