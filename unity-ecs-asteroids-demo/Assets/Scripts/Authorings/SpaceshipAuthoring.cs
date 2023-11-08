using Unity.Entities;
using UnityEngine;

namespace Tuatara
{
	[DisallowMultipleComponent]
	public sealed class SpaceshipAuthoring : MonoBehaviour
	{
		public float MoveSpeed = 10;
		public float RotateSpeed = 5;

		public class Baker : Baker<SpaceshipAuthoring>
		{
			public override void Bake(SpaceshipAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new Spaceship
				{
					MoveSpeed = authoring.MoveSpeed, 
					RotateSpeed = authoring.RotateSpeed
				});
			}
		}
	}
}