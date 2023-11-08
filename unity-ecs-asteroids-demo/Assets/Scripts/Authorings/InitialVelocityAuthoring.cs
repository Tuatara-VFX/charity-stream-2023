using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Tuatara
{
	[DisallowMultipleComponent]
	public sealed class InitialVelocityAuthoring : MonoBehaviour
	{
		public float3 Linear;
		public float3 Scale = new float3(1, 0, 1);
		[Range(0f, 1f)] public float DirectionRandomness;
		[Range(0f, 1f)] public float SpeedRandomness;

		public class Baker : Baker<InitialVelocityAuthoring>
		{
			public override void Bake(InitialVelocityAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new InitialVelocity
				{
					Linear = authoring.Linear,
					Scale = authoring.Scale,
					DirectionRandomness = authoring.DirectionRandomness,
					SpeedRandomness = authoring.SpeedRandomness,
				});
			}
		}
	}
}