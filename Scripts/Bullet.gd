extends Area2D

var travel_dist = 0

func _ready():
	name = "bullet"
	
func _physics_process(delta):
	const SPEED = 1000
	const RANGE = 1200
	
	var direction = Vector2.RIGHT.rotated(rotation)
	# only move if bullet hasn't exploded
	if get_node("AnimatedSprite2D").visible:
		position += direction * SPEED * delta
		
	travel_dist += SPEED * delta
	
	if travel_dist > RANGE:
		queue_free()

func _on_body_entered(body):
	#queue_free()
	explode_bullet()
	if body.has_method("take_damage"):
		body.take_damage()

func explode_bullet():
	const BULLET = preload("res://Scenes/ExplodeBullet.tscn")
	var new_bullet = BULLET.instantiate()
	self.add_child(new_bullet)
	new_bullet.name="Explosion"
	get_node("AnimatedSprite2D").visible=false;

