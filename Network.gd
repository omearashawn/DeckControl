extends Node
# Called when the node enters the scene tree for the first time.
var controller = null

func _ready():
	controller = get_node("/root/ControllerGlobal")
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	print(packPacket())
	pass
	
func _physics_process(delta):
	pass

func packPacket():
	var sticks = [controller.left_x, controller.left_y, controller.right_x, controller.right_y]
	return PackedByteArray(sticks)
		
	
