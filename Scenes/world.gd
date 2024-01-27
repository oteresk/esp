extends Node2D

func spawn_mob():
	var new_mob = preload("res://Scenes/enemy_1.tscn").instantiate()
	%PathFollow2D.progress_ratio = randf()
	new_mob.global_position = %PathFollow2D.global_position
	add_child(new_mob)

# this is just a rudimentary way of spawning enemies; a more detailed wave-like spawning system will replace this
func _on_enemy_timer_timeout():
	spawn_mob()