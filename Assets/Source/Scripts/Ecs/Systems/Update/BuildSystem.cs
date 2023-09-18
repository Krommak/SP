using Game.Components.Buildings;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Game.Data;
using System.Collections.Generic;
using Game.Components.Shared;
using Game.Commands;
using Game.Components.Navigation;

namespace Game.Systems.Update
{
    sealed class BuildSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;

        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<CreateBuilding>> _createFilter = default;
        readonly EcsPoolInject<CreateBuilding> _createPool = default;
        readonly EcsPoolInject<UIActions> _actionsPool = default;
        readonly EcsPoolInject<ViewComponent> _baseViewPool = default;
        readonly EcsPoolInject<SelectedCircle> _selectedCirclePool = default;
        readonly EcsPoolInject<IsBuilding> _isBuildingPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _createFilter.Value)
            {
                var component = _createPool.Value.Get(item);

                var data = Resources.Load<BuildingDataScriptable>(component.BuildName).GetData();

                CreateBuilding(component.OwnerEntity, data, component.SpawnPosition);

                _createPool.Value.Del(item);
            }
        }

        private void CreateBuilding(int playerEntity, BuildData data, Vector3 position)
        {
            var baseGo = GameObject.Instantiate(data.Prefab, position, Quaternion.identity);

            var baseEntity = _world.Value.NewEntity();

            ref var viewComponent = ref _baseViewPool.Value.Add(baseEntity);
            viewComponent.Transform = baseGo.transform;

            ref var actionsComponent = ref _actionsPool.Value.Add(baseEntity);
            var actions = new List<Command>();
            foreach (var unit in data.SpawningUnits)
            {
                _runtimeData.Value.SpawningDataService.RegisterNewData(_world.Value, unit);
                unit.SpawnPosition = position;
                actions.Add(
                    new SpawnUnitCommand(
                    new SpawnUnitCommandData()
                {
                    RuntimeData = _runtimeData.Value,
                    World = _world.Value,
                    BattleWorld = _runtimeData.Value.BattleWorld,
                    UnitName = unit.GetType().Name,
                    TimeForExecute = unit.TimeForExecute
                }));
            }
            actionsComponent.Value = actions;

            _isBuildingPool.Value.Add(baseEntity);

            ref var selectedCircle = ref _selectedCirclePool.Value.Add(baseEntity);
            selectedCircle.Value = baseGo.GetComponentInChildren<ParticleSystem>().gameObject;
            selectedCircle.Value.SetActive(false);

            var updateNavigation = _world.Value.NewEntity();
            _world.Value.GetPool<NavMeshSurfaceComponent>().Add(updateNavigation);
        }
    }
}