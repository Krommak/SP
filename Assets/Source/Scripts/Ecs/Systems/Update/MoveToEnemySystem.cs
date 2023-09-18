using Game.Components.Battle;
using Game.Components.ChooseUnits;
using Game.Components.Shared;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Systems.Update
{
    sealed class MoveToEnemySystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<FollowToEnemyComponent>> _followFilter = default;

        readonly EcsPoolInject<SetTargetForUnit> _setTargetForUnitsPool = default;
        readonly EcsPoolInject<FollowToEnemyComponent> _followPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            var battleUnitsFilter = _runtimeData.Value.BattleWorld.Filter<BattleViewComponent>().End();
            var battleUnitsPool = _runtimeData.Value.BattleWorld.GetPool<BattleViewComponent>();
            var attacksPool = _runtimeData.Value.BattleWorld.GetPool<AttackState>();

            foreach (var item in _followFilter.Value)
            {
                var targetEntity = _followPool.Value.Get(item).Value;

                if(targetEntity.Unpack(out var battleWorld, out var unpackedTarget))
                {
                    if (!battleUnitsPool.Has(unpackedTarget))
                    {
                        _followPool.Value.Del(item);
                        _setTargetForUnitsPool.Value.Del(item);
                        continue;
                    }

                    var targetUnitViewComponent = battleUnitsPool.Get(unpackedTarget);
                    var movingUnitViewComponent = _world.Value.GetPool<ViewComponent>().Get(item);
                    var attackDistance = _world.Value.GetPool<UnitStats>().Get(item).AttackDistance;

                    if (CheckDistanceForFollow(targetUnitViewComponent.Transform, movingUnitViewComponent.Transform, attackDistance))
                    {
                        ref var target = 
                            ref _setTargetForUnitsPool.Value.Has(item) 
                            ? ref _setTargetForUnitsPool.Value.Get(item) 
                            : ref _setTargetForUnitsPool.Value.Add(item);

                        var pos = movingUnitViewComponent.Transform.position;
                        target.Value = new float2(pos.x, pos.z);
                        continue;
                    }

                    ref var targetComponent = 
                        ref _setTargetForUnitsPool.Value.Has(item) 
                        ? ref _setTargetForUnitsPool.Value.Get(item) 
                        : ref _setTargetForUnitsPool.Value.Add(item);

                    var position = battleUnitsPool.Get(unpackedTarget).Transform.position;
                    targetComponent.Value = new float2(position.x, position.z);
                }
            }
        }

        private bool CheckDistanceForFollow(Transform movingUnit, Transform targetUnit, float attackDistance)
        {
            return Vector3.Distance(movingUnit.position, targetUnit.position) < attackDistance;
        }
    }
}