using Game.Components.Buildings;
using Game.Components.Shared;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Init
{
    sealed class InitPlayerSystem : IEcsInitSystem
    {
        readonly EcsCustomInject<StartupData> _sturtupData = default;

        #region Inject
        readonly EcsWorldInject _world = default;

        readonly EcsPoolInject<PlayerComponent> _playersPool = default;
        readonly EcsPoolInject<CreateBuilding> _createPool = default;
        #endregion

        public void Init(IEcsSystems systems)
        {
            var newPlayer = _world.Value.NewEntity();
            ref var component = ref _playersPool.Value.Add(newPlayer);
            component.PlayerEntity = newPlayer;

            var createBase = _world.Value.NewEntity();
            ref var createComponent = ref _createPool.Value.Add(createBase);
            createComponent.SpawnPosition = _sturtupData.Value.BasePosition;
            createComponent.OwnerEntity = newPlayer;
            createComponent.BuildName = "BaseBuild";
        }
    }
}