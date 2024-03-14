using Godot;
using System;

public partial class resourceGUI : CanvasLayer
{
    [Export] public Node nodeGUI { get; set; }

    [Export] public Label lblGold;
    [Export] public Label lblIron;
    [Export] public Label lblMana;
    [Export] public Label lblWood;
    [Export] public Label lblTimer;

    public override void _Ready()
	{
        lblTimer.Text = "00:00";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

}
