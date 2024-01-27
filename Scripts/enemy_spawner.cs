using Godot;
using System;
using System.Diagnostics;


public partial class enemy_spawner : Node2D {
	[Export] spawn_info[] spawns = new spawn_info[5];

	int time;
	public override void _Ready()
	{
		Area2D pl = (Area2D)GetTree().GetFirstNodeInGroup("players");
		time = 0;
	}

	/* public void _on_timer_timeout() {
		time += 1;
		var enemy_spawns = spawns;
		for(int i = 0; i < enemy_spawns.Length; i++) {
			if(time >= enemy_spawns[i].time_start && time < enemy_spawns[i].time_end) {
				if(enemy_spawns[i].spawn_delay_counter < enemy_spawns[i].enemy_spawn_delay) {
					enemy_spawns[i].spawn_delay_counter += 1;
				} else {
					enemy_spawns[i].spawn_delay_counter = 0;
					if(enemy_spawns[i].Enemy != null) {
						var new_enemy = enemy_spawns[i].Enemy;
						Debug.print(new_enemy);
						int counter = 0;
						while (counter < enemy_spawns[i].enemy_num) {
							var enemy_spawn = new_enemy.Instantiate();
							enemy_spawn.GlobalPosition = get_random_position();
							AddChild(enemy_spawn);
							counter += 1;
						}

					}
					
				}
			}
		}
	}

	public void get_random_position() {

	}*/
}

/*public partial class enemy_spawner : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}*/
