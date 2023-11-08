using Unity.Entities;
using UnityEngine;

namespace Tuatara
{
	[DisallowMultipleComponent]
	public sealed class ParticleSpawnerAuthoring : MonoBehaviour
	{
		public GameObject Prefab;
		public float RateOverTime = 1;
		public int EmitOnDestroy = 0;
		public int MaxInstances = 10;
		public bool Recycle;

		public class Baker : Baker<ParticleSpawnerAuthoring>
		{
			public override void Bake(ParticleSpawnerAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddBuffer<Instance>(entity);
				AddComponent(entity,
					new ParticleSpawnerConfiguration
					{
						Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
						RateOverTime = authoring.RateOverTime == 0 ? 0 : 1 / authoring.RateOverTime,
						EmitCountOnDestroy = authoring.EmitOnDestroy,
						MaxInstances = authoring.MaxInstances,
						Recycle = authoring.Recycle
					});
			}
		}
	}
}