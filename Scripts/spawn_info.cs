using Godot;
using System;
using MonoCustomResourceRegistry;


[GlobalClass] public partial class spawn_info : Resource {

	[Export] public int time_start {get; set; }
	[Export] public int time_end {get; set; }
	[Export] public Resource Enemy {get; set; }
	[Export] public int enemy_num {get; set; }
	[Export] public int enemy_spawn_delay {get; set; }
	[Export] public int spawn_delay_counter {get; set; }

	public spawn_info()
	{
		time_start = 0;
		time_end = 0;
		Enemy = null;
		enemy_num = 0;
		enemy_spawn_delay = 0;
		spawn_delay_counter = 0;
	}

    
}