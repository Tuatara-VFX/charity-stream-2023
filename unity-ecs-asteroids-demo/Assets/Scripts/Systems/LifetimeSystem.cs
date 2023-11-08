using Unity.Entities;

namespace Tuatara
{
	public struct Lifetime : IComponentData
	{
		public float Current;
		public float Max;
	}
	public partial class LifetimeSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>().ForEach(
				(Entity entity, EntityCommandBuffer ecb, ref Lifetime lifetime) =>
				{
					lifetime.Current += SystemAPI.Time.DeltaTime;
					if (lifetime.Current < lifetime.Max) return;
					ecb.DestroyEntity(entity);
				}).Schedule();
		}
	}
}