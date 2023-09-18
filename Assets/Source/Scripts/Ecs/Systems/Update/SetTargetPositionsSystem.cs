using Game.Components.Battle;
using Game.Components.ChooseUnits;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Update
{
    sealed class SetTargetPositionsSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsFilterInject<Inc<SetTargetForUnit>> _setTargetForUnitsFilter = default;
        readonly EcsFilterInject<Inc<UnitIsDead>> _deadUnitsFilter = default;

        readonly EcsPoolInject<SetTargetForUnit> _setTargetForUnitsPool = default;
        readonly EcsPoolInject<UnitNavMeshAgentComponent> _unitNavMeshAgentPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _setTargetForUnitsFilter.Value)
            {
                var target = _setTargetForUnitsPool.Value.Get(item).Value;

                if (!_unitNavMeshAgentPool.Value.Has(item)) continue;

                var agent = _unitNavMeshAgentPool.Value.Get(item).Value;
                agent.SetDestination(new UnityEngine.Vector3(target.x, 1f, target.y));

                _setTargetForUnitsPool.Value.Del(item);
            }
            foreach (var item in _deadUnitsFilter.Value)
            {
                if (!_unitNavMeshAgentPool.Value.Has(item)) continue;

                var agent = _unitNavMeshAgentPool.Value.Get(item).Value;

                if (agent.isStopped == false)
                    agent.isStopped = true;
            }
        }
    }
}