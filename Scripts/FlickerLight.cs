using Godot;
using System;

public partial class FlickerLight : PointLight2D
{
    [Export] public Timer tmr;
    [Export] public eMode mode;
    [Export] public float speed;
    [Export] public PointLight2D light;
    private float curGlow;
    private int dir = -1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        tmr.Start();
        curGlow = light.Energy;
    }


    private void OnTimerTimeout()
    {

        if (mode == eMode.Flash)
        {
            // show or hide light
            if (Visible == false)
                Visible = true;
            else
                Visible = false;
        }


        if (mode == eMode.Glow)
        {
            if (dir == -1)
            {
                light.Energy -= speed;
                if (light.Energy <= speed)
                    dir = 1;
            }
            else
            {
                light.Energy += speed;
                if (light.Energy >= 8 - speed)
                    dir = -1;
            }
        }
    }


    public enum eMode
    {
        Flash,
        Glow
    }

}

