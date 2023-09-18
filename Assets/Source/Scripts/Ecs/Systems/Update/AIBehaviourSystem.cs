using Game.Components.Battle;
using Game.Components.Buildings;
using Game.Components.ChooseUnits;
using Game.Components.Shared;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;

namespace Game.Systems.Update
{
    sealed class AIBehaviourSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsWorldInject _world = default;
        readonly EcsCustomInject<StartupData> _sturtupData = default;
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        readonly EcsFilterInject<Inc<IsUnit>> _units = default;
        readonly EcsFilterInject<Inc<IsBuilding, UIActions>, Exc<TimerQueue>> _building = default;
        readonly EcsPoolInject<UIActions> _actionsOfObjectsPool = default;
        readonly EcsPoolInject<FollowToEnemyComponent> _followPool = default;
        readonly EcsPoolInject<IsUnit> _unitsPool = default;
        #endregion

        AIState state = AIState.SpawnUnits;

        public void Run(IEcsSystems systems)
        {
            var unitsCount = _units.Value.GetEntitiesCount();

            if (unitsCount < _sturtupData.Value.AIBehaviour.CountOfUnitsForAttack)
            {
                state = AIState.SpawnUnits;
                foreach (var item in _building.Value)
                {
                    var actions = _actionsOfObjectsPool.Value.Get(item).Value;

                    for (int i = 0; i < _sturtupData.Value.AIBehaviour.CountOfUnitsForAttack; i++)
                    {
                        var timersPool = _world.Value.GetPool<TimerQueue>();

                        ref var timerQueue = ref timersPool.Has(item) ? ref timersPool.Get(item) : ref timersPool.Add(item);

                        if (timerQueue.Value == null)
                            timerQueue.Value = new Queue<TimerElement>();

                        var timer = new TimerElement();
                        timer.Timer = actions[0].TimeForExecute;
                        timer.EndTimerAction = actions[0];

                        timerQueue.Value.Enqueue(timer);
                    }
                }
            }
            else if(state != AIState.Attack)
            {
                var target = -1;
                var battleViewFilter = _runtimeData.Value.BattleWorld.Filter<BattleViewComponent>().End();
                var battleViewPool = _runtimeData.Value.BattleWorld.GetPool<BattleViewComponent>();

                foreach (var item in battleViewFilter)
                {
                    var battleViewComponent = battleViewPool.Get(item);
                    if(battleViewComponent.EntityInPlayerWorld.Unpack(out var world, out int unpackedEntity))
                    {
                        if (world == _world.Value) continue;
                    }

                    target = item;
                }

                if (target == -1) return;

                foreach (var item in _units.Value)
                {
                    ref var followComponent = ref _followPool.Value.Has(item) ? ref _followPool.Value.Get(item) : ref _followPool.Value.Add(item);
                    followComponent.Value = _runtimeData.Value.BattleWorld.PackEntityWithWorld(target);
                }
                state = AIState.Attack;
            }
        }

        private enum AIState
        {
            SpawnUnits,
            Attack
        }
    }
}