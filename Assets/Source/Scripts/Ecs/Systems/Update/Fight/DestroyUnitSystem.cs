using Game.Commands;
using Game.Components.Battle;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Update
{
    sealed class DestroyUnitSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<UnitIsDead, BattleViewComponent>, Exc<DestroyIsFinished>> _deadFilter = default;

        readonly EcsPoolInject<BattleViewComponent> _battleUnitsPool = default;
        readonly EcsPoolInject<FreeTimer> _timersPool = default;
        readonly EcsPoolInject<DestroyIsFinished> _finishedPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _deadFilter.Value)
            {
                var view = _battleUnitsPool.Value.Get(item);
                var newTimer = _world.Value.NewEntity();
                var timer = new TimerElement();
                view.Model.SetActive(false);
                view.SelectedCircle.SetActive(false);
                view.DeadEffect.SetActive(true);
                view.DeadEffect.transform.parent = null;

                if(view.EntityInPlayerWorld.Unpack(out var unitWorld, out var entity))
                {
                    unitWorld.GetPool<UnitIsDead>().Add(entity);
                }

                var data = new DestroyUnitData(_runtimeData.Value, _world.Value.PackEntityWithWorld(item), 3);
                timer.EndTimerAction = new FullDestroyUnit(data);
                timer.Timer = 3;

                ref var timerComponent = ref _timersPool.Value.Add(newTimer);
                timerComponent.Value = timer;

                _finishedPool.Value.Add(item);
            }
        }
    }

    public struct DestroyIsFinished
    { 
    }
}