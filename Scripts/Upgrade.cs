using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;


public partial class Upgrade : CanvasLayer
{
    [Export] public Texture2D[] texRarities;
    [Export] public Texture2D[] texElements;
    [Export] public TextureRect btnUpgrade;
    [Export] public TextureRect texElement;
    [Export] public Label lblType;
    [Export] public Label lblSubType;
    [Export] public Label lblDescription;
    [Export] public Label lblUpgrade;
    [Export] public Label lblRarity;
    [Export] public Control ctlUpgrade;

    static private Upgrade curUpgrade;
    private Callable DeleteThis;

    // enums
    public enum WEAPONTYPE
	{
		Slash, Projectile, Cross, Orbit, Random
	}
    public enum WEAPONELEMENT
    {
        Energy, Fire, Poison, Ice, Leech
    }
    public enum RARITYTYPE
    {
        Common, Rare, Epic, Legendary, New
    }
    public enum UPGRADETYPE
    {
        Dmg, AoE, Speed, None
    }
    public WEAPONTYPE wType;
    public WEAPONELEMENT eType;
    public RARITYTYPE rarity;
    public UPGRADETYPE uType;

    private string description = "";
    public float rarityMult=0;
    private Node2D selectedAttack;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        RandomizeUpgrade();
    }

    public override void _Input(InputEvent @event)
    {
        
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                //Debug.Print("Button: " + this);
                if (curUpgrade == this)
                {
                    Globals.UnPauseGame();
                    SelectUpgrade();
                    PrintAllUpgrades();
                    ShrinkSelected();
                    curUpgrade = null;
                }
                else
                {
                    Globals.UnPauseGame();
                    ShrinkNotSelected();
                }
            }
        }
    }

    private void PrintAllUpgrades()
    {
        if (Globals.ps.atkSlashEnergy.Count>0)
        {
            GD.PrintRich("[color=green]"+"[b]"+"atkSlashEnergy " + "[/b]"+ "AoE:" + Globals.ps.atkSlashEnergy[0].GetAOELevel() + " spd:" + Globals.ps.atkSlashEnergy[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkSlashEnergy[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkSlashIce.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkSlashIce " + "[/b]" + "AoE:" + Globals.ps.atkSlashIce[0].GetAOELevel() + " spd:" + Globals.ps.atkSlashIce[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkSlashIce[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkSlashLeeches.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkSlashLeeches " + "[/b]" + "AoE:" + Globals.ps.atkSlashLeeches[0].GetAOELevel() + " spd:" + Globals.ps.atkSlashLeeches[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkSlashLeeches[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkProjectileEnergy.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkProjectileEnergy " + "[/b]" + "AoE:" + Globals.ps.atkProjectileEnergy[0].GetAOELevel() + " spd:" + Globals.ps.atkProjectileEnergy[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkProjectileEnergy[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkProjectileLeeches.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkProjectileLeeches " + "[/b]" + "AoE:" + Globals.ps.atkProjectileLeeches[0].GetAOELevel() + " spd:" + Globals.ps.atkProjectileLeeches[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkProjectileLeeches[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkProjectilePoison.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkProjectilePoison " + "[/b]" + "AoE:" + Globals.ps.atkProjectilePoison[0].GetAOELevel() + " spd:" + Globals.ps.atkProjectilePoison[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkProjectilePoison[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkOrbitEnergy.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkOrbitEnergy " + "[/b]" + "AoE:" + Globals.ps.atkOrbitEnergy[0].GetAOELevel() + " spd:" + Globals.ps.atkOrbitEnergy[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkOrbitEnergy[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkOrbitFire.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkOrbitFire " + "[/b]" + "AoE:" + Globals.ps.atkOrbitFire[0].GetAOELevel() + " spd:" + Globals.ps.atkOrbitFire[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkOrbitFire[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkOrbitPoison.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkOrbitPoison " + "[/b]" + "AoE:" + Globals.ps.atkOrbitPoison[0].GetAOELevel() + " spd:" + Globals.ps.atkOrbitPoison[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkOrbitPoison[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkCrossFire.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkCrossFire " + "[/b]" + "AoE:" + Globals.ps.atkCrossFire[0].GetAOELevel() + " spd:" + Globals.ps.atkCrossFire[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkCrossFire[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkCrossIce.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkCrossIce " + "[/b]" + "AoE:" + Globals.ps.atkCrossIce[0].GetAOELevel() + " spd:" + Globals.ps.atkCrossIce[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkCrossIce[0].GetDmgLevel() + "[/color]");
        }
        if (Globals.ps.atkCrossLeeches.Count > 0)
        {
            GD.PrintRich("[color=green]" + "[b]" + "atkCrossLeeches " + "[/b]" + "AoE:" + Globals.ps.atkCrossLeeches[0].GetAOELevel() + " spd:" + Globals.ps.atkCrossLeeches[0].GetAttackSpeedLevel() + " dmg:" + Globals.ps.atkCrossLeeches[0].GetDmgLevel() + "[/color]");
        }

    }
    private async void ShrinkNotSelected()
    {
        float animTime = .5f;
        Tween tween = GetTree().CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(ctlUpgrade, "scale", new Vector2(0,0), animTime);
        tween.Parallel().TweenProperty(btnUpgrade, "modulate:a", 0f, animTime);

        await Task.Delay(TimeSpan.FromMilliseconds(animTime * 1000));
        if (IsInstanceValid(this)) // don't access disposed nodes
            this.QueueFree();
    }
    private async void ShrinkSelected()
    {
        float animTime = .7f;
        Tween tween = GetTree().CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        //tween.TweenProperty(ctlUpgrade, "scale", new Vector2(0, 0), animTime);
        tween.Parallel().TweenProperty(btnUpgrade, "modulate:a", 0f, animTime);

        await Task.Delay(TimeSpan.FromMilliseconds(animTime*1000));
        if (IsInstanceValid(this))
            this.QueueFree();
    }


    public void OnMouseEntered()
    {
        //Debug.Print("Entered");
        curUpgrade = this;
        Tween tween = GetTree().CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(ctlUpgrade, "scale", new Vector2(1.1f, 1.1f), .4f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Circ);
    }
    public void OnMouseExited()
    {
        //Debug.Print("Exited");
        Tween tween = GetTree().CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(ctlUpgrade, "scale", new Vector2(1, 1), .4f).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Circ);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.U)) // energy
        {
            RandomizeUpgrade();
        }
    }

    public void RandomizeUpgrade()
    {
        // set weapon type (1-3 - unlocked in Globals)
        int r;
        do
        {
            r = GD.RandRange(1, 4);
        } while (Globals.weaponTypeUnlocked[r - 1] == false);
        /*
        if (r==3)
            r = 1; // testing
        //r = 2;
        */
        //r = 3;

        switch (r)
        {
            case 1:
                wType = WEAPONTYPE.Slash;
                break;
            case 2:
                wType = WEAPONTYPE.Projectile;
                break;
            case 3:
                wType = WEAPONTYPE.Cross;
                break;
            case 4:
                wType = WEAPONTYPE.Orbit;
                break;
        }

        // set rarity
        r = GD.RandRange(1, 100);
        switch (r)
        {
            case < 82:
                rarity = RARITYTYPE.Common;
                rarityMult = 1;
                break;
            case < 94:
                rarity = RARITYTYPE.Rare;
                rarityMult = 2;
                break;
            case < 100:
                rarity = RARITYTYPE.Epic;
                rarityMult = 4;
                break;
            case 100:
                rarity = RARITYTYPE.Legendary;
                rarityMult = 8;
                break;
        }

        // choose random element
        ChooseElement:
            r = GD.RandRange(1, 3);

        // if fire is locked
        if (wType == WEAPONTYPE.Cross && r==1 && Globals.weaponTypeUnlocked[11] == false) // only allow fire if unlocked
            goto ChooseElement;
        if (wType == WEAPONTYPE.Orbit && r == 1 && Globals.weaponTypeUnlocked[11] == false) // only allow fire if unlocked
            goto ChooseElement;


        description = "";

        // *** Swipe ***
        if (wType == WEAPONTYPE.Slash)
        {
            switch (r)
            {
                case 1:
                    eType = WEAPONELEMENT.Energy;
                    if (Globals.ps.atkSlashEnergy.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkSlashEnergy[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkSlashEnergy[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkSlashEnergy[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkSlashEnergy[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkSlashEnergy[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkSlashEnergy[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkSlashEnergy[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else
                    {
                        description = "Creates a slash of energy.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 2:
                    eType = WEAPONELEMENT.Ice;
                    if (Globals.ps.atkSlashIce.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkSlashIce[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkSlashIce[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkSlashIce[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkSlashIce[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkSlashIce[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkSlashIce[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkSlashIce[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "Creates a slash of Ice shards that freezes the enemy.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 3:
                    eType = WEAPONELEMENT.Leech;
                    if (Globals.ps.atkSlashLeeches.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkSlashLeeches[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkSlashLeeches[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkSlashLeeches[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkSlashLeeches[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkSlashLeeches[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkSlashLeeches[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkSlashLeeches[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "Creates a slash of leeches that drains the enemy health and give some of it to you.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
            }
        }

        // *** Projectile ***
        if (wType == WEAPONTYPE.Projectile)
        {
            switch (r)
            {
                case 1:
                    eType = WEAPONELEMENT.Energy;
                    if (Globals.ps.atkProjectileEnergy.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkProjectileEnergy[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkProjectileEnergy[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkProjectileEnergy[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkProjectileEnergy[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkProjectileEnergy[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkProjectileEnergy[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkProjectileEnergy[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else
                    {
                        description = "targets a close enemy and sends a homing projectile right at it.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 2:
                    eType = WEAPONELEMENT.Poison;
                    if (Globals.ps.atkProjectilePoison.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkProjectilePoison[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkProjectilePoison[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkProjectilePoison[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkProjectilePoison[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkProjectilePoison[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkProjectilePoison[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkProjectilePoison[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "targets a close enemy and sends a homing poison projectile right at it. The poison does damage over time.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 3:
                    eType = WEAPONELEMENT.Leech;
                    if (Globals.ps.atkProjectileLeeches.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkProjectileLeeches[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkProjectileLeeches[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkProjectileLeeches[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkProjectileLeeches[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkProjectileLeeches[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkProjectileLeeches[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkProjectileLeeches[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "targets a close enemy and sends leeches right at it. The leeches drain the enemy health and give some of it to you.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
            }
        }

    // *********** Cross *************
        if (wType == WEAPONTYPE.Cross)
        {
            switch (r)
            {
                case 1:
                    eType = WEAPONELEMENT.Fire;
                    if (Globals.ps.atkCrossFire.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkCrossFire[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkCrossFire[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkCrossFire[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkCrossFire[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkCrossFire[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkCrossFire[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkCrossFire[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else
                    {
                        description = "Shoots fire in 4 directions.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 2:
                    eType = WEAPONELEMENT.Ice;
                    if (Globals.ps.atkCrossIce.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkCrossIce[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkCrossIce[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkCrossIce[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkCrossIce[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkCrossIce[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkCrossIce[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkCrossIce[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "Shoots ice in 4 directions. Ice will freeze enemies.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 3:
                    eType = WEAPONELEMENT.Leech;
                    if (Globals.ps.atkCrossLeeches.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkCrossLeeches[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkCrossLeeches[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkCrossLeeches[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkCrossLeeches[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkCrossLeeches[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkCrossLeeches[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkCrossLeeches[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "Shoots leeches in 4 directions. The leeches drain the enemy health and give some of it to you.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
            }
        }

        // *********** orbit *************
        if (wType == WEAPONTYPE.Orbit)
        {
            //Debug.Print("Orbit Upgrade: en:"+ Globals.ps.atkOrbitEnergy.Count + " fire:"+ Globals.ps.atkOrbitFire.Count+" po: " + Globals.ps.atkOrbitPoison.Count);
            switch (r)
            {
                case 1:
                    eType = WEAPONELEMENT.Fire;
                    if (Globals.ps.atkOrbitFire.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkOrbitFire[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkOrbitFire[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkOrbitFire[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkOrbitFire[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkOrbitFire[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkOrbitFire[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkOrbitFire[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else
                    {
                        description = "A flame orbits around you.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 2:
                    eType = WEAPONELEMENT.Energy;
                    if (Globals.ps.atkOrbitEnergy.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkOrbitEnergy[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkOrbitEnergy[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkOrbitEnergy[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkOrbitEnergy[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkOrbitEnergy[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkOrbitEnergy[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkOrbitEnergy[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "A ball of energy orbits around you.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
                case 3:
                    eType = WEAPONELEMENT.Poison;
                    if (Globals.ps.atkOrbitPoison.Count > 0) //not new
                    {
                        selectedAttack = Globals.ps.atkOrbitPoison[0];
                        // set upgrade type (DMG, AOE, Speed)
                        int rand = GD.RandRange(1, 3);
                        switch (rand)
                        {
                            case 1:
                                uType = UPGRADETYPE.Dmg;
                                description = "DMG: " + Globals.ps.atkOrbitPoison[0].GetDmgLevel().ToString() + " -> " + (Globals.ps.atkOrbitPoison[0].GetDmgLevel() + 1 * rarityMult).ToString();
                                break;
                            case 2:
                                uType = UPGRADETYPE.AoE;
                                description = "AoE: " + Globals.ps.atkOrbitPoison[0].GetAOELevel().ToString() + " -> " + (Globals.ps.atkOrbitPoison[0].GetAOELevel() + 1 * rarityMult).ToString();
                                break;
                            case 3:
                                uType = UPGRADETYPE.Speed;
                                description = "Speed: " + Globals.ps.atkOrbitPoison[0].GetAttackSpeedLevel().ToString() + " -> " + (Globals.ps.atkOrbitPoison[0].GetAttackSpeedLevel() + 1 * rarityMult).ToString();
                                break;
                        }
                    }
                    else // new
                    {
                        description = "A glob of poison orbits around you.";
                        rarity = RARITYTYPE.New;
                        uType = UPGRADETYPE.None;
                    }
                    break;
            }
        }




        SetRarity(rarity);
        SetElement(eType);
        if (uType != UPGRADETYPE.None)
        {
            lblUpgrade.Text = uType.ToString();
        }
        else
        {
            lblUpgrade.Text = "New";
            lblRarity.Text = "";
        }
            
        lblType.Text = wType.ToString();
        lblDescription.Text = description;
    }

    private void SelectUpgrade()
    {
        if (lblUpgrade.Text == "New")
        {
            Debug.Print("New: " + wType.ToString() + eType.ToString());
            Globals.ps.AddAttack(wType.ToString(), eType.ToString());
        }
        else // upgrade
        {
            Debug.Print("Upgrade: " + wType.ToString() + eType.ToString()+rarity.ToString());
            if (selectedAttack.HasMethod("AddUpgrade")) 
            {
                selectedAttack.Call("AddUpgrade", uType.ToString(),rarityMult);
            }
        }
    }

    public void SetRarity(RARITYTYPE rType)
    {
        //Debug.Print("Rarity: " + rType.ToString());
        lblRarity.Text = rType.ToString();

        ShaderMaterial upgradeMat = (ShaderMaterial)btnUpgrade.Material;

        switch (rType)
        {
            case RARITYTYPE.Common:
                btnUpgrade.Texture = texRarities[0];
                lblDescription.Modulate = new Color(.6f, .26f, 1, 1);
                lblRarity.Modulate= new Color(.6f, .26f, 1, 1);
                lblUpgrade.Modulate = new Color(0, .86f, .87f, 1);
                if (upgradeMat != null)
                {
                    upgradeMat.SetShaderParameter("block_size", 0);
                }

                break;
            case RARITYTYPE.Rare:
                btnUpgrade.Texture = texRarities[1];
                lblRarity.Modulate = new Color(0, .86f, .87f, 1);
                lblDescription.Modulate = new Color(0, .86f, .87f, 1);
                lblUpgrade.Modulate = new Color(0, .86f, .87f, 1);
                if (upgradeMat != null)
                {
                    Debug.Print("Rare not null: "+ lblUpgrade.Text);
                    upgradeMat.SetShaderParameter("block_size", 8);
                    upgradeMat.SetShaderParameter("starting_colour", new Color(0, .71f, .69f, 1));
                    upgradeMat.SetShaderParameter("ending_colour", new Color(0, .71f, .69f, 1));
                    upgradeMat.SetShaderParameter("min_line_width", 3);
                    upgradeMat.SetShaderParameter("max_line_width", 22);
                }
                break;
            case RARITYTYPE.Epic:
                btnUpgrade.Texture = texRarities[2];
                lblRarity.Modulate = new Color(.27f, 1, .53f, 1);
                lblDescription.Modulate = new Color(.27f, 1, .53f, 1);
                lblUpgrade.Modulate = new Color(0, .86f, .87f, 1);
                if (upgradeMat != null)
                {
                    upgradeMat.SetShaderParameter("block_size", 8);
                    upgradeMat.SetShaderParameter("starting_colour", new Color(0, .99f, .4f, 1));
                    upgradeMat.SetShaderParameter("ending_colour", new Color(0, .99f, .4f, 1));
                    upgradeMat.SetShaderParameter("min_line_width", 3);
                    upgradeMat.SetShaderParameter("max_line_width", 22);
                }
                break;
            case RARITYTYPE.Legendary:
                btnUpgrade.Texture = texRarities[3];
                lblRarity.Modulate = new Color(1, .45f, .8f, 1);
                lblDescription.Modulate = new Color(1, .45f, .8f, 1);
                lblUpgrade.Modulate = new Color(0, .86f, .87f, 1);
                if (upgradeMat != null)
                {
                    upgradeMat.SetShaderParameter("block_size", 8);
                    upgradeMat.SetShaderParameter("starting_colour", new Color(1, 0f, .18f, 1));
                    upgradeMat.SetShaderParameter("ending_colour", new Color(1, 0f, .18f, 1));
                    upgradeMat.SetShaderParameter("min_line_width", 3);
                    upgradeMat.SetShaderParameter("max_line_width", 22);
                }
                break;
            case RARITYTYPE.New:
                btnUpgrade.Texture = texRarities[4];
                lblDescription.Modulate = new Color(.8f, .8f, .3f, 2);
                lblUpgrade.Modulate = new Color(.8f, .8f, .3f, 2);
                if (upgradeMat != null)
                {
                    upgradeMat.SetShaderParameter("block_size", 8);
                    upgradeMat.SetShaderParameter("starting_colour", new Color(1, 1, 1, 1));
                    upgradeMat.SetShaderParameter("ending_colour", new Color(1, 1, 1, 1));
                    upgradeMat.SetShaderParameter("min_line_width", 0);
                    upgradeMat.SetShaderParameter("max_line_width", 3);
                }
                break;
        }
    }

    public void SetElement(WEAPONELEMENT eT)
    {
        lblSubType.Text = eT.ToString();
        switch (eT)
        {
            case WEAPONELEMENT.Energy:
                texElement.Texture = texElements[0];
                break;
            case WEAPONELEMENT.Fire:
                texElement.Texture = texElements[1];
                break;
            case WEAPONELEMENT.Poison:
                texElement.Texture = texElements[2];
                break;
            case WEAPONELEMENT.Ice:
                texElement.Texture = texElements[3];
                break;
            case WEAPONELEMENT.Leech:
                texElement.Texture = texElements[4];
                break;
        }
    }

    public string GetName() // wType + eType + uType
    {
        string n;
        n = wType.ToString() + eType.ToString() + uType.ToString();

        return n;
    }


}
