using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Tuatara
{
	using static SystemAPI;

	public struct Destructive : IComponentData
	{
		public bool IsDestructible;
		public bool DestroySelfOnContact;
	}

	[UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
	public partial struct DestructionSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var ecb = GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
			state.Dependency = new DestructionTriggerEvents
			{
				CommandBuffer = ecb,
				DestructiveLookup = GetComponentLookup<Destructive>(true),
			}.Schedule(GetSingleton<SimulationSingleton>(), state.Dependency);
		}
	}

	[BurstCompile]
	public partial struct DestructionTriggerEvents : ITriggerEventsJob
	{
		public EntityCommandBuffer CommandBuffer;
		[ReadOnly] public ComponentLookup<Destructive> DestructiveLookup;

		public void Execute(TriggerEvent triggerEvent)
		{
			if (!DestructiveLookup.HasComponent(triggerEvent.EntityA) ||
			    !DestructiveLookup.HasComponent(triggerEvent.EntityB)) return;

			var destructiveA = DestructiveLookup[triggerEvent.EntityA];
			var destructiveB = DestructiveLookup[triggerEvent.EntityB];
			if (destructiveA.IsDestructible || destructiveA.DestroySelfOnContact)
			{
				CommandBuffer.DestroyEntity(triggerEvent.EntityA);
			}
			if (destructiveB.IsDestructible || destructiveB.DestroySelfOnContact)
			{
				CommandBuffer.DestroyEntity(triggerEvent.EntityB);
			}
		}
	}
}