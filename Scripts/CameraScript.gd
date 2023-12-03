extends Camera2D

var speed=200

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	var inputVect=Input.get_vector("left","right","up","down")
	translate(inputVect*speed*delta)
