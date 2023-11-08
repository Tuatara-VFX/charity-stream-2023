using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Tuatara
{
	public struct ParticleSpawnerConfiguration : IComponentData
	{
		public Entity Prefab;
		public float RateOverTime;
		public int EmitCountOnDestroy;
		public int MaxInstances;
		public bool Recycle;
	}

	public struct ParticleSpawnerState : ICleanupComponentData
	{
		public Entity Prefab;

		public float NextInstantiateTime;
		public int EmitCount;
		public int EmitCountOnDestroy;
		public int LastEmitCount;

		public float3 LastPosition;
		public quaternion LastRotation;
	}

	public struct Instance : IBufferElementData
	{
		public Entity Value;
	}

	[UpdateInGroup(typeof(InitializationSystemGroup))]
	public partial class ParticleSpawnerSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			SetupParticleSystemState();
			ClearDestroyed();
			HandleSpawning();
			CreateParticlesOnDestroy();
		}

		private void ClearDestroyed()
		{
			Entities.ForEach((ref DynamicBuffer<Instance> particleInstances) =>
			{
				for (var i = particleInstances.Length - 1; i >= 0; i--)
				{
					if (SystemAPI.Exists(particleInstances[i].Value)) continue;
					particleInstances.RemoveAt(i);
				}
			}).Schedule();
		}

		private void HandleSpawning()
		{
			var dt = SystemAPI.Time.DeltaTime;
			var time = (float)SystemAPI.Time.ElapsedTime;
			Entities.WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>().ForEach(
				(Entity entity, EntityCommandBuffer ecb, ref ParticleSpawnerState spawner,
					ref DynamicBuffer<Instance> instances, in ParticleSpawnerConfiguration config,
					in LocalToWorld transform) =>
				{
					if (config.RateOverTime > 0 && time >= spawner.NextInstantiateTime)
					{
						spawner.EmitCount += 1 + (int)math.round(dt / config.RateOverTime);
						spawner.NextInstantiateTime = time + config.RateOverTime;
					}

					var belowMaxParticle = instances.Length < config.MaxInstances && !config.Recycle;
					if (!belowMaxParticle)
					{
						spawner.NextInstantiateTime = time + config.RateOverTime;
					}

					var emitCountDelta = math.abs(spawner.EmitCount - spawner.LastEmitCount);
					if (emitCountDelta > 0 && (belowMaxParticle || config.Recycle))
					{
						if (instances.Length >= config.MaxInstances)
						{
							ecb.DestroyEntity(instances[0].Value);
							instances.RemoveAt(0);
						}

						for (var i = 0; i < emitCountDelta && instances.Length < config.MaxInstances; i++)
						{
							ecb.AppendToBuffer(entity, new Instance
							{
								Value = CreateParticle(ecb, config.Prefab, transform.Position, transform.Rotation)
							});
						}
					}

					spawner.LastPosition = transform.Position;
					spawner.LastRotation = transform.Rotation;
					spawner.LastEmitCount = spawner.EmitCount;
				}).Schedule();
		}

		private void SetupParticleSystemState()
		{
			Entities.WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>()
				.WithAbsent<ParticleSpawnerState>().ForEach(
					(Entity entity, EntityCommandBuffer ecb, in ParticleSpawnerConfiguration config) =>
					{
						ecb.AddComponent(entity, new ParticleSpawnerState
						{
							Prefab = config.Prefab,
							EmitCountOnDestroy = config.EmitCountOnDestroy,
							NextInstantiateTime = -1,
						});
					}).Schedule();
		}

		private void CreateParticlesOnDestroy()
		{
			Entities.WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>()
				.WithAbsent<ParticleSpawnerConfiguration>().ForEach(
					(Entity entity, EntityCommandBuffer ecb, in ParticleSpawnerState state) =>
					{
						for (var i = 0; i < state.EmitCountOnDestroy; i++)
						{
							CreateParticle(ecb, state.Prefab, state.LastPosition, state.LastRotation);
						}

						ecb.RemoveComponent<ParticleSpawnerState>(entity);
					}).Schedule();
		}

		private static Entity CreateParticle(in EntityCommandBuffer ecb, in Entity prefab, in float3 position,
			in quaternion rotation)
		{
			var particle = ecb.Instantiate(prefab);
			var particleTransform = new LocalTransform
			{
				Position = position,
				Rotation = rotation,
				Scale = 1
			};
			ecb.SetComponent(particle, particleTransform);
			return particle;
		}
	}
}