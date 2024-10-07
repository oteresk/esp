using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class StatUpgrade : MarginContainer
{
	[Export] public TextureRect image;

    [Export] public TextureRect imgSlot1;
	[Export] public TextureRect imgSlot2;
	[Export] public TextureRect imgSlot3;
	[Export] public TextureRect imgSlot4;
	[Export] public TextureRect imgSlot5;
	[Export] public string upgradeName;
    public int slots;
    [Export] public string description;
    [Export] public int upgradeNum;

	private bool overButton = false;

    private AudioStreamPlayer statsMusic;

    public override void _Ready()
	{
        DelayedStart();
        Tween tween = GetTree().CreateTween();
        tween.Parallel().TweenProperty(image, "scale", new Vector2(.95f, .95f), .2f);

        Node nodMusic = GetNode("/root/StatUpgrades/statsMusic");
        statsMusic = (AudioStreamPlayer)nodMusic;
        
    }

	public async void DelayedStart()
    {
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(300));

        StatUpgrades.lblGold.Text=ResourceDiscoveries.gold.ToString();

        UpdateSlots();

        CheckUpgrade();

        CheckCanAfford();

        if (IsInstanceValid(statsMusic))
            statsMusic.Play();

        //Debug.Print("gold:"+ ResourceDiscoveries.gold.ToString());
    }

	public void OnMouseEntered()
	{
		overButton = true;
        Tween tween = GetTree().CreateTween();
        tween.Parallel().TweenProperty(image, "scale", new Vector2(1,1), .2f);
        //Debug.Print("entered: " + upgradeName);
	}

    public void OnMouseExited()
    {
        overButton = false;
        Tween tween = GetTree().CreateTween();
        tween.Parallel().TweenProperty(image, "scale", new Vector2(.95f, .95f), .2f);
        //Debug.Print("exited: " + upgradeName);
    }

    public override void _Input(InputEvent @event)
    {

        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (overButton == true)
                {
                    if (StatUpgrades.lblCost!=null)
                    {
                        if (Globals.statUpgradeLevel[upgradeNum] < Globals.MAXUPGRADES)
                        {
                            Debug.Print("Click: " + upgradeName);
                            StatUpgrades.lblCost.Text = "Cost: "+Globals.coststatUpgrade[upgradeNum, Globals.statUpgradeLevel[upgradeNum]].ToString();

                            StatUpgrades.imgSelUpgrade.Texture = image.Texture;
                            StatUpgrades.curUpgradeNum = upgradeNum;
                            StatUpgrades.lblUpgradeName.Text = upgradeName;
                            StatUpgrades.sUpgrade = this;
                            CheckUpgrade();
                        }
                    }
                }
            }
        }
    }


    public void UpdateSlots()
    {
        slots = Globals.statUpgradeLevel[upgradeNum];
        //Debug.Print("Slots:" + slots);
        if (IsInstanceValid(imgSlot1))
        {
            if (slots == 0)
            {
                imgSlot1.Texture = StatUpgrades.slotEmpty;
                imgSlot2.Texture = StatUpgrades.slotEmpty;
                imgSlot3.Texture = StatUpgrades.slotEmpty;
                imgSlot4.Texture = StatUpgrades.slotEmpty;
                imgSlot5.Texture = StatUpgrades.slotEmpty;
            }
            if (slots == 1)
            {
                imgSlot1.Texture = StatUpgrades.slotFull;
                imgSlot2.Texture = StatUpgrades.slotEmpty;
                imgSlot3.Texture = StatUpgrades.slotEmpty;
                imgSlot4.Texture = StatUpgrades.slotEmpty;
                imgSlot5.Texture = StatUpgrades.slotEmpty;
            }
            if (slots == 2)
            {
                imgSlot1.Texture = StatUpgrades.slotFull;
                imgSlot2.Texture = StatUpgrades.slotFull;
                imgSlot3.Texture = StatUpgrades.slotEmpty;
                imgSlot4.Texture = StatUpgrades.slotEmpty;
                imgSlot5.Texture = StatUpgrades.slotEmpty;
            }
            if (slots == 3)
            {
                imgSlot1.Texture = StatUpgrades.slotFull;
                imgSlot2.Texture = StatUpgrades.slotFull;
                imgSlot3.Texture = StatUpgrades.slotFull;
                imgSlot4.Texture = StatUpgrades.slotEmpty;
                imgSlot5.Texture = StatUpgrades.slotEmpty;
            }
            if (slots == 4)
            {
                imgSlot1.Texture = StatUpgrades.slotFull;
                imgSlot2.Texture = StatUpgrades.slotFull;
                imgSlot3.Texture = StatUpgrades.slotFull;
                imgSlot4.Texture = StatUpgrades.slotFull;
                imgSlot5.Texture = StatUpgrades.slotEmpty;
            }
            if (slots == 5)
            {
                imgSlot1.Texture = StatUpgrades.slotFull;
                imgSlot2.Texture = StatUpgrades.slotFull;
                imgSlot3.Texture = StatUpgrades.slotFull;
                imgSlot4.Texture = StatUpgrades.slotFull;
                imgSlot5.Texture = StatUpgrades.slotFull;
            }
        }
    }

    // if cost<=gold then enable upgrade button
    public bool CheckUpgrade()
    {
        bool canUpgrade = false;

        Debug.Print("upgradeNum: "+ upgradeNum+" Globals.statUpgradeLevel[upgradeNum]: " + Globals.statUpgradeLevel[upgradeNum]);


        // check if you can afford upgrade
        if (StatUpgrades.curUpgradeNum < 0)
            return false;

        if (Globals.statUpgradeLevel[upgradeNum] > Globals.MAXUPGRADES-1)
        {
            btnUpgrade.DisableButton();
            return false;
        }

        Debug.Print("upgradeNum:" + upgradeNum + " Globals.statUpgradeLevel[upgradeNum]:" + Globals.statUpgradeLevel[upgradeNum] + " Globals.MAXUPGRADES:" + Globals.MAXUPGRADES);

        if (Globals.coststatUpgrade[upgradeNum, Globals.statUpgradeLevel[upgradeNum]]<= ResourceDiscoveries.gold)
        {
            btnUpgrade.EnableButton();
            canUpgrade = true;

            // check if upgrade is maxed out

            if (Globals.statUpgradeLevel[upgradeNum] < Globals.MAXUPGRADES)
            {
                btnUpgrade.EnableButton();
                canUpgrade = true;
            }
            else
            {
                btnUpgrade.DisableButton();
                canUpgrade= false;
            }
        }
        else
        {
            btnUpgrade.DisableButton();
            canUpgrade = false;
        }

        return canUpgrade;
    }

    // changes upgrade image to gold if can afford or silver if can't
    public void CheckCanAfford()
    {
        if (Globals.statUpgradeLevel[upgradeNum] > Globals.MAXUPGRADES - 1)
        {
            //  unselected
            image.Modulate = new Color(1, 1, 1, .5f);
            return;
        }

        if (Globals.coststatUpgrade[upgradeNum, Globals.statUpgradeLevel[upgradeNum]] <= ResourceDiscoveries.gold)
        {
            // selected
            //image.Visible = true;
            if (IsInstanceValid(image))
                image.Modulate = new Color(1, 1, 1, 1f);
        }
        else
        {
            // , unselected
            if (IsInstanceValid(image))
                image.Modulate = new Color(1, 1, 1, .5f);
        }
    }


}
