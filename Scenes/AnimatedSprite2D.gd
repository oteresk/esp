extends AnimatedSprite2D

@onready var anim = $"."

# Called when the node enters the scene tree for the first time.
func _ready():
	anim.play("Explode")
	
func on_finished():
	get_parent().queue_free()
	
