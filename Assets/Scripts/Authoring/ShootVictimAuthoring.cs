using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ShootVictimAuthoring : MonoBehaviour {

    public Transform hitPositionTransform; 

    public class Baker : Baker<ShootVictimAuthoring> {
        public override void Bake(ShootVictimAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootVictim {
                hitLocalPositon = authoring.hitPositionTransform.localPosition, 
            });
        }
    }
}

public struct ShootVictim : IComponentData {
    public float3 hitLocalPositon;
}