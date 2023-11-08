using Unity.Entities;
using UnityEngine;

namespace Tuatara
{
	[DisallowMultipleComponent]
	public sealed class LifetimeAuthoring : MonoBehaviour
	{
		public float Duration = 1f;

		public class Baker : Baker<LifetimeAuthoring>
		{
			public override void Bake(LifetimeAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new Lifetime
				{
					Max = authoring.Duration
				});
			}
		}
	}
}