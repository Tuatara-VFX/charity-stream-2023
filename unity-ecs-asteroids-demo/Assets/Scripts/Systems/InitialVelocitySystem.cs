using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Tuatara
{
	public struct InitialVelocity : IComponentData
	{
		public float3 Linear;
		public float3 Scale;
		public float DirectionRandomness;
		public float SpeedRandomness;
	}

	[RequireMatchingQueriesForUpdate]
	[UpdateInGroup(typeof(InitializationSystemGroup))]
	public partial class InitialVelocitySystem : SystemBase
	{
		protected override void OnUpdate()
		{
			var singleton = SystemAPI.GetSingleton<RandomSystem.Singleton>();
			Entities.WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>().ForEach(
				(Entity entity, EntityCommandBuffer ecb, ref PhysicsVelocity physicsVelocity,
					in InitialVelocity velocity, in LocalTransform localTransform) =>
				{
					var direction = math.normalizesafe(math.mul(math.slerp(quaternion.LookRotationSafe(velocity.Linear, math.up()),
						singleton.Random.NextQuaternionRotation(),
						velocity.DirectionRandomness), math.forward()) * velocity.Scale);
					
					var linear = direction * math.length(velocity.Linear) * velocity.Scale;

					linear *= 1 - singleton.Random.NextFloat(velocity.SpeedRandomness);
					
					physicsVelocity.Linear += localTransform.TransformDirection(linear);

					ecb.RemoveComponent<InitialVelocity>(entity);
				}).Run();
			SystemAPI.SetSingleton(singleton);
		}
	}
}