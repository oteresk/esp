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
    [Export] public Control confirm;

    public override void _Ready()
    {
        confirm.Visible = false;

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

        FadeIn();
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

        var statUpgrade = sceneTree.GetNodesInGroup("StatUpgrade");

        Debug.Print("Stat Upgrades: " + statUpgrade.Count);

        for (int i = 0; i < statUpgrade.Count; i++)
        {
            statUpgrade[i].Call("UpdateSlots");
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
        //GetTree().ChangeSceneToFile("res://Scenes/Title.tscn");
        GetTree().ChangeSceneToPacked(Globals.TitleScene);
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

    }


    public void ShowConfirm()
    {
        confirm.Visible = true;
        Globals.rootNode.GetTree().Paused = true;
    }

    public void ConfirmNo()
    {
        confirm.Visible = false;
        Globals.rootNode.GetTree().Paused = false;
    }

    public void ConfirmYes()
    {
        confirm.Visible = false;
        Globals.rootNode.GetTree().Paused = false;
        Globals.instance.ResetStats();
    }


}
