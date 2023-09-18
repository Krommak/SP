using Game.Commands;
using Game.Components.Battle;
using Game.Components.Shared;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Systems.Update
{
    sealed class AttackSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<IsUnit>, Exc<AttackState>> _unitsFilter = default;

        readonly EcsPoolInject<FreeTimer> _freeTimersPool = default;
        readonly EcsPoolInject<AttackState> _attacksPool = default;
        readonly EcsPoolInject<UnitStats> _statsPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            var battleUnitsPool = _runtimeData.Value.BattleWorld.GetPool<BattleViewComponent>();
            var battleAttacksPool = _runtimeData.Value.BattleWorld.GetPool<AttackState>();
            var myEnemies = _runtimeData.Value.UnitService.GetMyEnemies(_world.Value);
            var isDeadPool = _runtimeData.Value.BattleWorld.GetPool<UnitIsDead>();

            foreach (var item in _unitsFilter.Value)
            {
                var attackUnitViewComponent = _world.Value.GetPool<ViewComponent>().Get(item);
                
                var target = -1;
                var needRemoveEnemies = false;

                if(myEnemies.Count > _unitsFilter.Value.GetEntitiesCount())
                    needRemoveEnemies = true;

                foreach (var enemy in myEnemies)
                {
                    if (isDeadPool.Has(enemy)) continue;

                    var stats = _statsPool.Value.Get(item);
                    var targetUnitViewComponent = battleUnitsPool.Get(enemy);

                    if (CheckDistanceForFollow(targetUnitViewComponent.Transform, attackUnitViewComponent.Transform, stats.AttackDistance))
                    {
                        if (!attackUnitViewComponent.EntityInBattleWorld.Unpack(out var battleWorld, out var unpackAssaulter))
                            continue;

                        var battleUnitsStatsPool = _runtimeData.Value.BattleWorld.GetPool<BattleUnitStats>();

                        var timer = 1 / stats.AttackSpeed;

                        var data = new AttackData(attackUnitViewComponent.EntityInBattleWorld,
                            _runtimeData.Value.BattleWorld.PackEntityWithWorld(enemy),
                            timer, stats.DamageType);

                        var attackCommand = new AddAttackComponentByTimerEnd(data);

                        ref var timerComponent = 
                            ref _freeTimersPool.Value.Has(item) 
                            ? ref _freeTimersPool.Value.Get(item) 
                            : ref _freeTimersPool.Value.Add(item);

                        timerComponent.Value = new TimerElement()
                        {
                            EndTimerAction = attackCommand,
                            Timer = timer
                        };

                        target = enemy;

                        ref var attack = ref _attacksPool.Value.Add(item);
                        attack.TargetUnit = targetUnitViewComponent.Transform;

                        ref var battleAttack = ref battleAttacksPool.Add(unpackAssaulter);
                        battleAttack.TargetUnit = targetUnitViewComponent.Transform;

                        break;
                    }
                }

                if (target != -1 && needRemoveEnemies)
                {
                    myEnemies.Remove(target);
                }
            }
        }

        private bool CheckDistanceForFollow(Transform movingUnit, Transform targetUnit, float attackDistance)
        {
            return Vector3.Distance(movingUnit.position, targetUnit.position) < attackDistance;
        }
    }
}