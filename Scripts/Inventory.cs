using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public partial class Inventory : Control
{
    //[Export] public MarginContainer slots;
    [Export] public TextureRect[] relics;

    private bool backpackOpen = false;
    public override void _Ready()
	{
        DelayedStart();
    }

    public async void DelayedStart()
    {
        // wait a bit
        await Task.Delay(TimeSpan.FromMilliseconds(250));
        SetAllRelics();
    }

    private void SetAllRelics()
    {
        Debug.Print("Set relics");
		for (int i = 0; i < relics.Length; i++)
		{
            if (Globals.hasRelic[i])
            {
                relics[i].Visible = true;
                Debug.Print("true");
            }
            else
            {
                relics[i].Visible = false;
            }
        }
	}

}
