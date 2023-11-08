using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tuatara
{
	public struct Spaceship : IComponentData
	{
		public float MoveSpeed;
		public float RotateSpeed;

		public float2 Move;
		public bool Fire;
	}

	public partial class SpaceshipSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			HandleInput();
			Move();
			Fire();
		}

		private void HandleInput()
		{
			if (PlayerInput.all.Count == 0) return;
			var playerInput = PlayerInput.all[0];
			var move = playerInput.currentActionMap["Move"].ReadValue<Vector2>();
			var fire = playerInput.currentActionMap["Fire"].WasPerformedThisFrame();
			Entities.ForEach((ref Spaceship spaceship) =>
			{
				spaceship.Move = move;
				spaceship.Fire = fire;
			}).Run();
		}

		private void Move()
		{
			var dt = SystemAPI.Time.DeltaTime;
			Entities.ForEach((ref PhysicsVelocity physicsVelocity, ref LocalTransform localTransform,
				in Spaceship spaceship) =>
			{
				if (math.length(spaceship.Move) <= 0) return;

				physicsVelocity.Linear += localTransform.Forward() * (spaceship.Move.y * spaceship.MoveSpeed * dt);
				localTransform.Rotation = math.mul(localTransform.Rotation,
					quaternion.AxisAngle(new float3(0, 1, 0), spaceship.Move.x * spaceship.RotateSpeed * dt));
			}).Schedule();
		}

		private void Fire()
		{
			Entities.ForEach((ref ParticleSpawnerState myParticleSystem, in Spaceship spaceship) =>
			{
				if (!spaceship.Fire) return;
				myParticleSystem.EmitCount++;
			}).Schedule();
		}
	}
}