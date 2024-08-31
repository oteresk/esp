using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class GolemFactoryProgress : ProgressBar
{
	
	public int manaGiven;
	public int manaCost=5;
	public float curProgress; // progress of processing the current mana
	public float totalProgress;
	[Export] public Timer tmrProgress;
	private double updateFreq = .2f;
	[Export] public ProgressBar curProgBar;
	private float waitTime=5;
	private bool factoryRunning;
	private ResourceDiscovery curRD;
	public bool golemComplete=false;

    public override void _Ready()
	{
        //get resourceDicoveries of partent
        curRD=(ResourceDiscovery)this.GetNode("..");

		curProgBar.MaxValue = manaCost;

        tmrProgress.WaitTime=waitTime;
		// give first mana
		if (ResourceDiscoveries.mana > 0)
		{
            ResourceDiscoveries.mana -= 1;
			ResourceDiscoveries.UpdateResourceGUI();

			factoryRunning = true;

            manaGiven++;
            this.MaxValue = manaCost;
            this.Value = manaGiven;
            UpdateProgress();

        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	async void UpdateProgress() // runs everfy second
	{
		await Task.Delay(TimeSpan.FromMilliseconds(updateFreq*1000));
		if (factoryRunning == true && IsInstanceValid(tmrProgress))
		{
			curProgress = ((float)(1 - (tmrProgress.TimeLeft / tmrProgress.WaitTime)));
			Debug.Print("Progress: " + curProgress + "waittime:" + tmrProgress.WaitTime + " timeleft:" + tmrProgress.TimeLeft+" manaCost:"+manaCost+" manaGiven:"+manaGiven);
			curProgBar.Value = (manaGiven - 1) + curProgress;
            this.Value = manaGiven;
            UpdateProgress();
		}
	}


	public void OnTimeOver()
	{
		totalProgress+=waitTime;
		curProgress=1;
        //Debug.Print("ManaGiven:"+manaGiven+"Progress: " + curProgress+"waittime:"+ tmrProgress.WaitTime+" timeleft:"+ tmrProgress.TimeLeft);
        curProgBar.Value = (manaGiven - 1) + curProgress;
        // give next mana
        if (manaGiven==manaCost && golemComplete==false)
		{
            Globals.golemAlive = true;
            Debug.Print("Golem is alive!");
            golemComplete = true;
			// create golem
            PackedScene golemScene = (PackedScene)ResourceLoader.Load("res://Scenes/FriendlyGolem.tscn");
            var FriendlyGolem = golemScene.Instantiate();
            Node2D world = (Node2D)GetNode(Globals.NodeWorld);
            world.CallDeferred("add_child", FriendlyGolem);
			Node2D fG = (Node2D)FriendlyGolem;
			fG.Position = curRD.GlobalPosition+new Vector2(-30,250);
            Globals.golem = fG;
			Golem golem=(Golem)fG;
			golem.SetGolemLevel(Globals.golemLevel);

			// create minimap golem
			Node miniMap = GetNode(Globals.NodeMiniMap);
            MiniMap mm=(MiniMap)miniMap;
			mm.CreateGolemIcons();
			mm.DisplayGolem();

            curProgBar.Visible = false;
			this.Visible = false;
			factoryRunning = false;
			curRD.factoryisOn = false;
            curRD.ToggleSwitch(false);
            curRD.CompleteFactory();
			curRD.switchEnabled = false;
        }
		else
		if (manaGiven<manaCost)
		{
			if (ResourceDiscoveries.mana > 0 && curRD.factoryisOn)
			{
				if (factoryRunning==false)
				{
					factoryRunning = true;
					curRD.StartFactory();
					UpdateProgress();
                }
				ResourceDiscoveries.mana -= 1;
				ResourceDiscoveries.UpdateResourceGUI();
				manaGiven++;

				totalProgress = manaGiven + curProgress;
                curProgBar.Value = (manaGiven - 1) + curProgress;
                this.Value = manaGiven;

				//Debug.Print("Time over - manaCost:" + manaCost + " manaGiven:" + manaGiven + " totalProgress:" + totalProgress);
			}
			else
			{
                factoryRunning = false;
                curRD.PauseFactory();
            }
		}
		
		
	}


}
