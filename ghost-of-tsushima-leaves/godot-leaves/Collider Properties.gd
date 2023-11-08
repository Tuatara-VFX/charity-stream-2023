@tool
extends GPUParticles3D

@export var colliderNode3D = Node3D

var velocity = Vector3(0, 0, 0)
var lastPosition = Vector3(0, 0, 0)

func _process(delta):
	# Send collider position
	process_material.set_shader_parameter("Collider_Position", colliderNode3D.position)
	# Send collider radius
	var nodeScale = colliderNode3D.scale
	process_material.set_shader_parameter("Collider_Radius", max(nodeScale.x, nodeScale.y, nodeScale.z) * 0.5)
	
	# Compute and send collider velocity
	if lastPosition != colliderNode3D.position:
		velocity = colliderNode3D.position - lastPosition
		velocity /= delta
		lastPosition = colliderNode3D.position
	else:
		velocity = Vector3(0, 0, 0)
	
	process_material.set_shader_parameter("Collider_Velocity", velocity)
	#pass
