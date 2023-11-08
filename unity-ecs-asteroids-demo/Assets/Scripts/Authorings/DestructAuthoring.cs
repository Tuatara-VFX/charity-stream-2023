using Unity.Entities;
using UnityEngine;

namespace Tuatara
{
	[DisallowMultipleComponent]
	public sealed class DestructiveAuthoring : MonoBehaviour
	{
		public bool IsDestructible;
		public bool DestroySelfOnContact;
		
		public class Baker : Baker<DestructiveAuthoring>
		{
			public override void Bake(DestructiveAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new Destructive
				{
					IsDestructible = authoring.IsDestructible,
					DestroySelfOnContact = authoring.DestroySelfOnContact,
				});
			}
		}
	}
}