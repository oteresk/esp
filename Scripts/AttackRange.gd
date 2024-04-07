extends Area2D

enum BulletSource { Player , Tower }
@export var bSource: BulletSource

func _physics_process(_delta):
	var enemies_in_range = get_overlapping_bodies()
	if enemies_in_range.size() > 0:
		var target_enemy = enemies_in_range[0]
		look_at(target_enemy.global_position)

func shoot():
	const BULLET = preload("res://Scenes/bullet2.tscn")
	var new_bullet = BULLET.instantiate()
	new_bullet.global_position = %ShootingPoint.global_position
	new_bullet.global_rotation = %ShootingPoint.global_rotation
	#print("adding bullet child")
	var bullets=get_node("/root/World/Bullets")
	bullets.add_child(new_bullet)
	# scale tower bullets bigger
	match bSource:
		BulletSource.Player:
			new_bullet.get_node("AnimatedSprite2D").scale.x=.3
			new_bullet.get_node("AnimatedSprite2D").scale.y=.3
			new_bullet.modulate=Color(.2,.2,1,1)

		BulletSource.Tower:
			new_bullet.get_node("AnimatedSprite2D").scale.x=1
			new_bullet.get_node("AnimatedSprite2D").scale.y=1
			new_bullet.modulate=Color(1,.3,.8,1)
			

func _on_bullet_timer_timeout():
	var enemies_in_range = get_overlapping_bodies()
	if enemies_in_range.size() > 0:
		shoot()
		#print("shooting")
