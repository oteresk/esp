using Godot;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class ItemScript : Area2D
{
    [Export] public ItemType itemType;
    [Export] public float XP;
    [Export] public int frame;

    public bool collected = false;

    private Node2D Player;
    private double speed = -1; // magnetism start speed

    static private int items = 0; // counts the total number of items in use - for displaying item icons

    static private bool itemSpeedExists = false;
    static private bool itemAttackSpeedExists = false;
    static private bool itemDamageExists = false;
    static private bool itemAoEExists = false;
    static private bool itemShieldExists = false;

    Tween fadeTween;

    public float aliveTime = 10; // item will fade away after time

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        Player = (Node2D)GetNode(Globals.NodePlayer);

        Sprite2D sprItem = (Sprite2D)GetNode("Sprite2D");
        sprItem.Frame = frame;

        StartAliveTimer(aliveTime);

    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (collected) // gets called when gem is in player magnetic area
        {
            Vector2 pos = Player.GlobalPosition + new Vector2(0, -50);
            GlobalPosition = GlobalPosition.MoveToward(pos, (float)speed);
            speed += 3 * delta;
        }
    }

    public void OnAreaEntered(Area2D area)
	{
        if (area.IsInGroup("Players") && Globals.playerAlive) // picked up by player
        {
            Visible = false;



            if (itemType == ItemType.Gem)
            {
                Globals.AddXP(XP);
                // TODO: play sound for pickup gem
            }
            else
            {
                UsePotion(itemType);
            }
            
            //Debug.Print("Pickup collide");
        }
	}

    public enum ItemType
    {
        Gem, SpeedPotion, AttackSpeedPotion, DamagePotion, AoEPotion, ShieldPotion
    }

    public void OnVisibilityChanged() // if player picks up item
    {
        if(!IsVisibleInTree())
        {
            if (fadeTween!=null)
            {
                fadeTween.Stop();
                fadeTween.Kill();
            }
            if (IsInstanceValid(this))
            {
                QueueFree();
            }
        }
    }

    async void UsePotion(ItemType iType)
    {
        // start item effect
        player ps = (player)Player;

        if (Globals.playerAlive)
        {
            // remove poison
            Globals.RemoveAllPoison();

            items++;
            if (itemType == ItemType.SpeedPotion)
                ps.IncreaseSpeed(3);
            if (itemType == ItemType.AttackSpeedPotion)
                ps.SetAllAttackSpeed(.5f);
            if (itemType == ItemType.ShieldPotion)
                ps.EnableShield();

            // adjust item icon positions
            ps.AdjustItemIcons(items);

            // wait
            await Task.Delay(TimeSpan.FromMilliseconds(10000));

            items--;

            // disable item effect
            if (itemType == ItemType.SpeedPotion)
            {
                ps.ResetSpeed();
                itemSpeedExists = false;
            }
            if (itemType == ItemType.AttackSpeedPotion)
            {
                ps.ResetAllAttackSpeed();
                itemAttackSpeedExists = false;
            }
            if (itemType == ItemType.ShieldPotion)
            {
                ps.DisableShield();
                itemShieldExists = false;
            }

            // adjust item icon positions
            ps.AdjustItemIcons(items);
        }
    }
    public void CreateItem()
    {
        // randomly get temp potion 
        int rPotion = GD.RandRange(1, Globals.potionFreq);

        if (rPotion == 1)
        {
            // random potion type - 1=speed 2=AttackSpeed 3=Shield
            int rType = GD.RandRange(1, 3);
            if (rType == 1)
            {
                if (itemSpeedExists == false)
                {
                    itemType = ItemScript.ItemType.SpeedPotion;
                    itemSpeedExists = true;
                    frame = 5;
                    XP = 0;
                }
                else
                    CreateGem();
            }
            if (rType == 2)
            {
                if (itemAttackSpeedExists == false)
                {
                    itemType = ItemScript.ItemType.AttackSpeedPotion;
                    itemAttackSpeedExists = true;
                    frame = 4;
                    XP = 0;
                }
                else
                    CreateGem();
            }
            if (rType == 3)
            {
                if (itemShieldExists == false)
                {
                    itemType = ItemScript.ItemType.ShieldPotion;
                    itemShieldExists = true;
                    frame = 8;
                    XP = 0;
                }
                else
                    CreateGem();
            }
        }
        else
            CreateGem();
    }

    private void CreateGem()
    {
        itemType = ItemScript.ItemType.Gem;
        //XP = 1; // TODO: when we have multiple enemies, this will need to be a switch case for enemy tpe and amount of XP
        int rndXP = (int)GD.RandRange(1, XP);
        if (rndXP == 1)
        {
            frame = 0;
            XP = 1;
        }
        else
            if (rndXP < 5) 
            {
                frame = 1;
                XP = 5;
            }
            else
                if (rndXP < 10)
                {
                    frame = 2;
                    XP = 10;
                }


    }

    public async void Fade(float fadeTime)
    {
        if (IsInstanceValid(this))
        {
            fadeTween = GetTree().CreateTween();
            fadeTween.TweenProperty(this, "modulate:a", 0f, fadeTime);

            await Task.Delay(TimeSpan.FromMilliseconds(fadeTime*1000));
            if (IsInstanceValid(this))
            {
                fadeTween.Stop();
                fadeTween.Kill();
                QueueFree();
            }
        }
    }

    public async void StartAliveTimer(float aliveTime)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(aliveTime * 1000));

        Fade(2.2f);
    }
    


}
