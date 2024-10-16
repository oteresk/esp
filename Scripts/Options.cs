using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class Options : Node2D
{
    private bool btnBackEntered;
    private ColorRect black;

    [Export] public Slider sldMusicVolume;
    [Export] public Slider sldSFXVolume;
    [Export] public OptionButton optResolution;
    [Export] public CheckButton chkFullscreen;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Node nodBlack = GetNode("Black");
        black = (ColorRect)nodBlack;

        // get current resolution
        string resString = DisplayServer.ScreenGetSize().X + "x" + DisplayServer.ScreenGetSize().Y;

        int sel = -1;
        for (int i = 0;i<optResolution.ItemCount;i++)
        {
            Debug.Print("opt " + i + ": " + optResolution.GetItemText(i));
            if (optResolution.GetItemText(i) == resString)
            {
                sel = i;
            }
        }

        optResolution.Selected= sel;

        // set fullscreen chk
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen)
        {
            Debug.Print("Exclusive fullscreen");
            chkFullscreen.ButtonPressed = true;
        }
        else
            chkFullscreen.ButtonPressed = false;

        // set volumes
        Debug.Print("Music vol: " + Globals.settings_MusicVolume);
        Debug.Print("SFX vol: " + Globals.settings_SFXVolume);

        DelayedStart();
    }

    private async void DelayedStart()
    {
        for (int i = 0; i < 10; i++) // wait to load volume settings
        {
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            if (Globals.settingsLoaded)
                break;
        }
        sldMusicVolume.Value = Mathf.DbToLinear(Globals.settings_MusicVolume);
        sldSFXVolume.Value = Mathf.DbToLinear(Globals.settings_SFXVolume);
        Globals.settingsLoaded = false;
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public void BackEntered()
    {
        btnBackEntered = true;
    }

    public void BackExited()
    {
        btnBackEntered = false;
    }

    public override void _Input(InputEvent @event)
    {

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //Debug.Print("Click");
                if (btnBackEntered == true)
                {
                    Debug.Print("back");
                    // save options

                    // volumes
                    Globals.settings_MusicVolume = (float)Mathf.LinearToDb(sldMusicVolume.Value);
                    Globals.settings_SFXVolume = (float)Mathf.LinearToDb(sldSFXVolume.Value);

                    SaveLoad.ClearSettings();
                    SaveLoad.SaveSettings();

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
        black.Visible = false;
        //GetTree().ChangeSceneToFile("res://Scenes/Title.tscn");
        GetTree().ChangeSceneToPacked(Globals.TitleScene);
    }

    public void _on_slider_music_volume_mouse_exited()
    {
        sldMusicVolume.ReleaseFocus();
    }

    public void _on_slider_music_volume_valueChanged(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Music"), (float)Mathf.LinearToDb(sldMusicVolume.Value));
    }

    public void _on_slider_sfx_volume_mouse_exited()
    {
        sldSFXVolume.ReleaseFocus();
    }

    public void _on_slider_sfx_volume_valueChanged(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("SFX"), (float)Mathf.LinearToDb(sldSFXVolume.Value));
    }
    public void ChangeResolution(int item)
    {
        Debug.Print("Resolution: " + optResolution.GetItemText(item));
        string full = optResolution.GetItemText(item);
        string[] separate = full.Split('x');
        Debug.Print(separate[0]+" x "+separate[1]);
        GetWindow().Size = new Vector2I(Int32.Parse(separate[0]), Int32.Parse(separate[1]));
        CenterWindow();
    }

    public void CenterWindow()
    {
        Vector2I screenCenter = DisplayServer.ScreenGetPosition() + DisplayServer.ScreenGetSize()/2;
        Vector2I windowSize = GetWindow().GetSizeWithDecorations();
        GetWindow().Position = screenCenter - windowSize/ 2;

        Debug.Print("center:"+screenCenter+" size: "+windowSize);
    }

    public void FullScreenPress(bool pressed)
    {
        if (pressed)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
        }
        else
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
    }

}
