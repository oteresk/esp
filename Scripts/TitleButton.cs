using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class TitleButton : TextureButton
{
    Control steamManager;
	[Export] public Label lblButton;
    private bool overButton = false;
    private ColorRect black;

    private AudioStreamPlayer titleMusic;

    public override void _Ready()
    {
        Node nodMusic = GetNode("/root/Title/titleMusic");
        titleMusic = (AudioStreamPlayer)nodMusic;

        Node nodBlack = GetNode("/root/Title/Control/Black");
        black = (ColorRect)nodBlack;

        if (lblButton.Name == "lblSteamName")
        {
            steamManager = (Control)GetNode("/root/SteamManager");
            lblButton.Text = steamManager.Call("GetSteamName").ToString();
        }

        FadeIn();
    }

    public void Hover()
	{
        lblButton.Modulate = new Color(1, 1, .2f, 1);
        overButton = true;
    }

    public void UnHover()
    {
        lblButton.Modulate = new Color(1, 1, 1, 1);
        overButton = false;
    }

    public override void _Input(InputEvent @event)
    {

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (overButton == true)
                {
                        Fade();
                }
            }
        }
    }

    private async void Fade()
    {
        black.Visible = true;

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(black, "modulate:a", 1f, 2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(2000));


        Debug.Print("Load scene: "+ lblButton.Name);

        if (lblButton.Name == "lblPlay")
        {
            //GetTree().ChangeSceneToFile("res://Scenes/world.tscn");
            GetTree().ChangeSceneToPacked(Globals.WorldScene);
        }

        if (lblButton.Name == "lblUpgrades")
        {
            //GetTree().ChangeSceneToFile("res://Scenes/StatUpgrades.tscn");
            GetTree().ChangeSceneToPacked(Globals.StatUpgradesScene);
        }

        if (lblButton.Name == "lblOptions")
        {
            //GetTree().ChangeSceneToFile("res://Scenes/Options.tscn");
            GetTree().ChangeSceneToPacked(Globals.OptionsScene);
        }
        if (lblButton.Name == "lblQuit")
        {
            Debug.Print("Quit");
            GetTree().Quit();
        }
    }

    private async void FadeIn()
    {
        black.Visible = true;
        black.Modulate = new Color(1, 1, 1, 1);
        await Task.Delay(TimeSpan.FromMilliseconds(1900));

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(black, "modulate:a", 0f, 3f);

        // play music
        for (int i = 0; i < 10; i++) // wait to load volume settings
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
            if (Globals.settingsLoaded)
                break;
        }
        titleMusic.Play();
    }
}
