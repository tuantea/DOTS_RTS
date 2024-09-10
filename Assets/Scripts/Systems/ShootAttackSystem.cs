using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct ShootAttackSystem : ISystem {

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target,
            RefRW<UnitMover> unitMover)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>,
                RefRW<UnitMover>>().WithDisabled<MoveOverride>()) {

            if (target.ValueRO.targetEntity == Entity.Null) {
                continue;
            }
            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            if (math.distance(targetLocalTransform.Position, localTransform.ValueRO.Position) > shootAttack.ValueRO.attackDistance) {
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            } else {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }
            float3 animDirection = targetLocalTransform.Position -localTransform.ValueRO.Position;
            animDirection = math.normalize(animDirection);

            quaternion targetRotation = quaternion.LookRotation(animDirection, math.up());
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation, targetRotation, unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0f) {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

           
            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            float3 bulletSpawnWorldPositon = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPositon));
            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;
            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity=target.ValueRO.targetEntity;
            shootAttack.ValueRW.onShoot.isTriggered = true;
            shootAttack.ValueRW.onShoot.shootFromPosition = bulletSpawnWorldPositon;



        }
    }

}