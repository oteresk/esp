using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class Inventory : VBoxContainer
{
    [Export] public MarginContainer slots;
    [Export] public TextureRect[] relics;

    private bool backpackOpen = false;
    public override void _Ready()
	{
        DelayedStart();
        slots.Modulate=new Color(1,1,1,0);
		
    }

    public async void DelayedStart()
    {
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(250));
        SetAllRelics();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void OnClickBackpack()
	{
		Debug.Print("Click backpack");

		if (!backpackOpen) 
		{
			backpackOpen = true;
            ShowRelicOutlines();
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(slots, "modulate:a", 1, 2.1f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);
            SetAllRelics();
        }
		else
		{
			backpackOpen = false;
            HideRelicOutlines();
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(slots, "modulate:a", 0, 1.4f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        }
    }

    private void HideRelicOutlines()
    {
        for (int i = 0; i < relics.Count(); i++)
        {
            relics[i].UseParentMaterial = true;
        }
    }

    private void ShowRelicOutlines()
    {
        for (int i = 0; i < relics.Count(); i++)
        {
            relics[i].UseParentMaterial = false;
        }
    }


    private void SetAllRelics()
    { 
		for (int i = 0; i < relics.Length; i++)
		{
            if (Globals.hasRelic[i])
            {
                relics[i].Visible = true;
            }
            else
            {
                relics[i].Visible = false;
            }
        }
	}

}
