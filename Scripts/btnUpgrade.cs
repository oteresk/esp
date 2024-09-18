using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class btnUpgrade : TextureButton
{
	static private Label lblUpgrade;
	static private TextureButton textureButtonUpgrade;
	public override void _Ready()
	{
		DelayedStart();
	}

	public async void DelayedStart()
	{
		// wait a bit
		await Task.Delay(TimeSpan.FromMilliseconds(200));

		Node upgradeNode = Globals.rootNode.GetNode("Control/TextureRect/MCbtnUpgrade/TextureButton/lblUpgrade");
		lblUpgrade = (Label)upgradeNode;
		lblUpgrade.Modulate = new Color(1, 1, 1, .5f);

		upgradeNode = Globals.rootNode.GetNode("Control/TextureRect/MCbtnUpgrade/TextureButton");
		textureButtonUpgrade = (TextureButton)upgradeNode;
		textureButtonUpgrade.Disabled = true;

		DisableButton();
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_accept") && HasFocus())
		{
			// EmitSignal(_pressed());
			ClickButton();
		}
	}

	static public void DisableButton()
	{
		lblUpgrade.Modulate = new Color(1, 1, 1, .5f);
		textureButtonUpgrade.Disabled = true;

		StatUpgrades.curUpgradeNum = -1;
	}

	static public void EnableButton()
	{
		Debug.Print("Enabled");
		lblUpgrade.Modulate = new Color(1, 1, .5f, 1);
		textureButtonUpgrade.Disabled = false;
	}

	public void ClickButton()
	{
		Debug.Print("Upgrade: " + StatUpgrades.curUpgradeNum);
		if (StatUpgrades.curUpgradeNum>=0)
		{
			if (ResourceDiscoveries.gold >= Globals.coststatUpgrade[StatUpgrades.curUpgradeNum, Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum]])
			{
				// minus gold
				ResourceDiscoveries.gold -= Globals.coststatUpgrade[StatUpgrades.curUpgradeNum, Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum]];

				// update gold label
				StatUpgrades.lblGold.Text = ResourceDiscoveries.gold.ToString();

				// increase upgrade level
				Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum]++;

				// save gold
				SaveLoad.SaveGame();

				// Update slots
				Node nd = Globals.rootNode.GetNode(".");
				StatUpgrades su = (StatUpgrades)nd;
				su.UpdateAllSlots();

				Debug.Print("curUpgradeNum:" + StatUpgrades.curUpgradeNum);

				// update cost label
				if (Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum] < Globals.MAXUPGRADES) // if upgrade not maxed out
				{
					StatUpgrades.lblCost.Text = Globals.coststatUpgrade[StatUpgrades.curUpgradeNum, Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum]].ToString();

					// check if can afford next upgrade
					if (!StatUpgrades.sUpgrade.CheckUpgrade())
					{
						Debug.Print("Disabled");
						btnUpgrade.DisableButton();
					}
					else
					{
						// check if upgrade is maxed out
						Debug.Print("Globals.statUpgradeLevel[upgradeNum]: " + Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum]);
						if (Globals.statUpgradeLevel[StatUpgrades.curUpgradeNum] < Globals.MAXUPGRADES)
						{
							Debug.Print("Enabled");
							btnUpgrade.EnableButton();
						}
						else
						{
							Debug.Print("Disabled");
							btnUpgrade.DisableButton();
							// reset selUpgrade
							StatUpgrades.ResetUpgrade();
						}
					}
				}
				else
				{
					StatUpgrades.lblCost.Text = "";
					btnUpgrade.DisableButton();
				}

				StatUpgrades.sUpgrade.CheckCanAfford();

				//SaveLoad.SaveGame();
			}
		}
		
	}

}
