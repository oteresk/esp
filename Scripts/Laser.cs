using Godot;
using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading.Tasks;

public partial class Laser : AnimatedSprite2D
{
    [Export] public AnimatedSprite2D startParticle;
    [Export] public CpuParticles2D endParticle;
    public override void _Ready()
	{
        Visible = false;
		AimAtPlayer(10);
        endParticle.Scale = new Vector2(.5f, .5f);
        endParticle.Spread = 0;
        startParticle.Play();
    }

	private async void AimAtPlayer(float delay)
	{
        await Task.Delay(TimeSpan.FromMilliseconds(delay));
        LookAt(Globals.ps.GlobalPosition+new Vector2(0,-30));
        RotationDegrees = RotationDegrees + 180;
        AimAtPlayer(10);
    }

    public void Shoot()
    {
        Visible=true;
        Play();
        startParticle.Visible = true;
        endParticle.Emitting = true;
        //Debug.Print("Shoot");
    }

    public void OnAnimationFinished()
    {
        Visible = false;
        startParticle.Visible = false;
        endParticle.Emitting = false;
    }
		

}
