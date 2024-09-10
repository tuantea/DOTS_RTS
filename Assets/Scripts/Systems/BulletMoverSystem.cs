using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;

partial struct BulletMoverSystem : ISystem {


    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<Bullet> bullet,
            RefRO<Target> target,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<Bullet>,
                RefRO<Target>>().WithEntityAccess()) {
            if (target.ValueRO.targetEntity == Entity.Null) {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);

            float3 targetPosion = targetLocalTransform.TransformPoint(shootVictim.hitLocalPositon);

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetPosion);

            float3 moveDirection = targetPosion - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.speed * SystemAPI.Time.DeltaTime;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosion);

            if (distanceAfterSq > distanceBeforeSq) {
                // Overshot
                localTransform.ValueRW.Position = targetPosion;
            }

            float destroyDistanceSq = .2f;
            if (math.distancesq(localTransform.ValueRO.Position, targetPosion) < destroyDistanceSq) {
                // Close enough to damage target
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }


}
