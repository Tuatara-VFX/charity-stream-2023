using Unity.Entities;
using Unity.Mathematics;

namespace Tuatara
{
	public partial struct RandomSystem : ISystem
	{
		public struct Singleton : IComponentData
		{
			public Random Random;
		}

		public void OnCreate(ref SystemState state)
		{
			state.EntityManager.AddComponentData(state.SystemHandle, new Singleton
			{
				Random = Random.CreateFromIndex((uint)System.Environment.TickCount)
			});
		}
	}
}