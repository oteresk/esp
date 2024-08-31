using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class resourceGUI : CanvasLayer
{
    [Export] public Node nodeGUI { get; set; }

    [Export] public Label lblGold;
    [Export] public Label lblIron;
    [Export] public Label lblMana;
    [Export] public Label lblWood;
    [Export] public Label lblResearch;
    [Export] public Label lblTimer;

    [Export] public TextureRect imgPlatform;
    public bool platformAvailable = false;
    private bool platOn = false;
    public override void _Ready()
	{
        lblTimer.Text = "00:00";
	}


    public async void FlashPlatform()
    {
        if (platOn)
            await Task.Delay(TimeSpan.FromMilliseconds(1000));
        else
            await Task.Delay(TimeSpan.FromMilliseconds(400));
        if (platOn)
        {
            imgPlatform.Visible = true;

            platOn = false;
        }
        else
        {
            imgPlatform.Visible = false;

            platOn = true;
        }

        if (platformAvailable)
            FlashPlatform();
        else
            imgPlatform.Visible = false;
    }

    public void OnPlatformButtonEntered()
    {
        Debug.Print("Platbut entered");
    }

    public void OnPlatformButtonExited()
    {
        Debug.Print("Platbut exited");
    }

}
