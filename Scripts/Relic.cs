using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ItemScript;
using static System.Net.Mime.MediaTypeNames;

public partial class Relic : Area2D
{
    [Export] public Texture2D[] imgRelics;
    [Export] public Sprite2D sprRelic;
    [Export] public Sprite2D sprShadow;
    [Export] public Sprite2D sprDiscovery;

    public bool collected = false;
    public bool pickedUp = false;
    private double speed = -1; // magnetism start speed

    [Export] public Area2D AreaDiscovery;
    [Export] public AudioStreamPlayer sndGetRelic;
    [Export] public AudioStreamPlayer sndDiscoverRelic;

    [Export] public GpuParticles2D dust;
    [Export] public GpuParticles2D stars;

    private double rad = .1f;
    private bool dir = true;
    private ShaderMaterial matDiscovery;

    private int relicNum;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        sprRelic = (Sprite2D)GetNode("sprRelic");
        sprRelic.Visible = false;

        sprDiscovery = (Sprite2D)GetNode("sprDiscovery");
        matDiscovery = (ShaderMaterial)sprDiscovery.Material;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (sprDiscovery.Visible) // alter shader
        {
            if (dir)
            {
                rad += (double).06f * delta;
            }
            else
            {
                rad -= (double).06f * delta;
            }
            if (rad > .4f)
                dir = false;

            if (rad < .1f)
                dir = true;
            matDiscovery.SetShaderParameter("radius", rad);
        }
	}


    public void SetRelic(int rNum)
    {
        Debug.Print("relic num: "+rNum);
        relicNum = rNum;
        sprRelic.Texture = imgRelics[rNum];
        sprShadow.Texture = imgRelics[rNum];
        Debug.Print("SetRelic: "+relicNum+" - "+imgRelics[relicNum].ResourcePath);
    }

    public string GetRelicName(int relicNum)
    {
		string rName= imgRelics[relicNum].ResourceName.ToString();

		return rName;
    }

    public void OnAreaEntered(Area2D area) // player enters relic
	{
        if (area.IsInGroup("Player") && Globals.playerAlive) // picked up by player
        {
            if (collected == false) // if just discovered relic
            {
                AreaDiscovery.SetDeferred("monitoring", false);
                stars.Emitting = true;
                dust.Emitting = true;
                sndDiscoverRelic.Play();
                Scale = new Vector2(0, 0);
                sprRelic.Visible = true;
                sprShadow.Visible = true;
                sprDiscovery.Visible = false;
                Debug.Print("discovered");
                Tween tween = GetTree().CreateTween();
                tween.TweenProperty(sprRelic, "position", new Vector2(0, -280), .6f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(sprRelic, "position", new Vector2(0, 0), 1.2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Bounce);
                tween.TweenCallback(Callable.From(EnableCollider));

                Tween t2 = GetTree().CreateTween();
                t2.Parallel().TweenProperty(this, "scale", new Vector2(.5f, .5f), 2.5f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
            }
            else // if magnetized and now colliding with player
            {
                // TODO: give player item
                if (pickedUp == false)
                {
                    pickedUp = true;
                    GivePlayerItem();
                }
            }
        }
    }

    private async void GivePlayerItem()
    {
        // play sound
        sndGetRelic.Play();
        Visible = false;

        Globals.hasRelic[relicNum]=true;
        // save
        SaveLoad.SaveGame();


            await Task.Delay(TimeSpan.FromMilliseconds(5000));
        QueueFree();
    }

    private void EnableCollider()
    {
        this.SetDeferred("monitorable", true);
        this.SetDeferred("monitoring", true);
    }

    public override void _PhysicsProcess(double delta)
    { 
        base._PhysicsProcess(delta);
        if (collected) // gets called when gem is in player magnetic area
        {
            Vector2 pos = Globals.ps.GlobalPosition + new Vector2(0, -50);
            GlobalPosition = GlobalPosition.MoveToward(pos, (float)speed);
            speed += 3 * delta;
        }
    }



}
