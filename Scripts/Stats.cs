using Godot;
using System;

public partial class Stats : VBoxContainer
{    // Attack, Tower, Golem
    static public int[] statCostL1 = { 4,2,2,999 }; // cost in research points
    static public int[] statCostL2 = { 4,4,5,999 }; // cost in research points
    private bool btnAttackLevel = false;
    private bool btnTowerLevel = false;
    private bool btnGolemLevel = false;
    private bool buttonsEnabled = true;
    public override void _Ready()
	{
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public override void _Input(InputEvent @event)
    {
        if (buttonsEnabled)
        {
            if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                if (mouseEvent.ButtonIndex == MouseButton.Left)
                {
                    if (btnAttackLevel) // upgrade attack level
                    {
                        if (Globals.attackLevel == 1)
                        {
                            if (ResourceDiscoveries.research >= statCostL1[0])
                            {
                                ResourceDiscoveries.research -= statCostL1[0];
                                Globals.attackLevel++;
                                Globals.UpdateStatsGUI();
                                UpdateStats();
                                ResourceDiscoveries.UpdateResourceGUI();
                                Globals.SetAttackLevel();
                            }
                        }
                    }

                    if (btnTowerLevel) // upgrade tower level
                    {
                        if (Globals.towerLevel == 1)
                        {
                            if (ResourceDiscoveries.research >= statCostL1[1])
                            {
                                ResourceDiscoveries.research -= statCostL1[1];
                                Globals.towerLevel++;
                                Globals.UpdateStatsGUI();
                                UpdateStats();
                                ResourceDiscoveries.UpdateResourceGUI();
                                Globals.SetTowerLevel();
                            }
                        }
                        else
                            if (Globals.towerLevel == 2)
                            {
                                if (ResourceDiscoveries.research >= statCostL2[2])
                                {
                                    ResourceDiscoveries.research -= statCostL2[2];
                                    Globals.towerLevel++;
                                    Globals.UpdateStatsGUI();
                                    UpdateStats();
                                    ResourceDiscoveries.UpdateResourceGUI();
                                    Globals.SetTowerLevel();
                                }
                            }
                    }

                    if (btnGolemLevel) // upgrade golem level
                    {
                        if (Globals.golemLevel == 1)
                        {
                            if (ResourceDiscoveries.research >= statCostL1[2])
                            {
                                ResourceDiscoveries.research -= statCostL1[2];
                                Globals.golemLevel++;
                                // TODO: upgrade golems
                                //Globals.weaponTypeUnlocked[2] = true; // cross
                                Globals.UpdateStatsGUI();
                                UpdateStats();
                                ResourceDiscoveries.UpdateResourceGUI();
                            }
                        }
                    }

                }
            }
        }

    }                  
                        
                        
    public static void UpdateStats()
	{
        
		if (Globals.attackLevel==1)
		{
            if (ResourceDiscoveries.research >= statCostL1[0]) // Attack L1
            {
                Globals.lblAttack.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblAttack.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }
        if (Globals.attackLevel == 2)
        {
            if (ResourceDiscoveries.research >= statCostL2[0]) // Attack L2
            {
                Globals.lblAttack.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblAttack.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }


        if (Globals.towerLevel == 1)
        {
            if (ResourceDiscoveries.research >= statCostL1[1]) // Tower L1
            {
                Globals.lblTowerLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblTowerLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }
        if (Globals.towerLevel == 2)
        {
            if (ResourceDiscoveries.research >= statCostL2[2]) // Tower L2
            {
                Globals.lblTowerLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblTowerLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }
        if (Globals.towerLevel == 3)
        {
            if (ResourceDiscoveries.research >= statCostL2[3]) // Tower L3
            {
                Globals.lblTowerLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblTowerLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }


        if (Globals.golemLevel == 1)
        {
            if (ResourceDiscoveries.research >= statCostL1[2]) // Golem L1
            {
                Globals.lblGolemLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblGolemLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }
        if (Globals.golemLevel == 2)
        {
            if (ResourceDiscoveries.research >= statCostL2[2]) // Golem L2
            {
                Globals.lblGolemLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(0, 1, 0, 1);
            }
            else
                Globals.lblGolemLevel.GetParent().GetChild<TextureRect>(0).Modulate = new Color(1, 1, 1, 1);
        }

    }

    public void OnBtnAttackLevelEnter()
    {
        btnAttackLevel = true;
    }

    public void OnBtnAttackLevelExit()
    {
        btnAttackLevel = false;
    }

    public void OnBtnTowerLevelEnter()
    {
        btnTowerLevel = true;
    }

    public void OnBtnTowerLevelExit()
    {
        btnTowerLevel = false;
    }

    public void OnBtnGolemLevelEnter()
    {
        btnGolemLevel = true;
    }

    public void OnBtnGolemLevelExit()
    {
        btnGolemLevel = false;
    }

}
