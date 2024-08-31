using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

public partial class StatUpgrades : CanvasLayer
{
	static public Label lblGold;
    static public TextureRect imgSelUpgrade;

    static public Texture2D imgEmptyUpgrade;

    static public Texture2D slotEmpty;
    static public Texture2D slotFull;
    static public int curUpgradeNum=-1;
    static public Label lblUpgradeName;
    static public Label lblCost;
    static public Label lblDescription;
    static public StatUpgrade sUpgrade;
    private bool btnBackEntered;
    private ColorRect black;

    public override void _Ready()
    {
		Node nod = Globals.rootNode.GetNode("Control/TextureRect/MCGold/HBGold/MCGold/Gold");
		lblGold = (Label)nod;

        nod = Globals.rootNode.GetNode("Control/TextureRect/MCUpgradeImage/UpgradeImage");
        imgSelUpgrade = (TextureRect)nod;

        nod = Globals.rootNode.GetNode("Control/TextureRect/MCUpgradeName/UpgradeName");
        lblUpgradeName = (Label)nod;

        nod = Globals.rootNode.GetNode("Control/TextureRect/MClblCost/lblCost");
        lblCost = (Label)nod;

        nod = Globals.rootNode.GetNode("Control/TextureRect/MCDescription/Description");
        lblDescription = (Label)nod;

        imgEmptyUpgrade = (Texture2D)GD.Load("res://Art/GUI/StatUpgrades/Border.png");

        slotEmpty = (Texture2D)GD.Load("res://Art/GUI/StatUpgrades/Block_Empty.png");
        slotFull = (Texture2D)GD.Load("res://Art/GUI/StatUpgrades/Block_Full.png");

        Node nodBlack = GetNode("Black");
        black = (ColorRect)nodBlack;

        ResetUpgrade();
    }

    static public void ResetUpgrade()
    {
        // reset selUpgrade
        imgSelUpgrade.Texture = imgEmptyUpgrade;
        lblUpgradeName.Text = "";
        lblCost.Text = "";
        lblDescription.Text = "";
    }

	public void UpdateAllSlots()
	{
        var mainLoop = Godot.Engine.GetMainLoop();
        var sceneTree = mainLoop as SceneTree;

        var statUpgrades = sceneTree.GetNodesInGroup("StatUpgrade");

        Debug.Print("Stat Upgrades: " + statUpgrades.Count);

        for (int i = 0; i < statUpgrades.Count; i++)
        {
            statUpgrades[i].Call("UpdateSlots");
        }

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
                Debug.Print("Click");
                if (btnBackEntered == true)
                {
                    Debug.Print("back");
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
        GetTree().ChangeSceneToFile("res://Scenes/Title.tscn");
    }
}
