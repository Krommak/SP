using Game.Components.Battle;
using Game.Components.Unit;
using Game.Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Update
{
    sealed class UpdateUnitStats : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        readonly EcsWorldInject _worldInject = default;
        readonly EcsFilterInject<Inc<IncreaseArmor>> _increaseArmorFilterInject = default;
        readonly EcsFilterInject<Inc<DecreaseArmor>> _decreaseArmorFilterInject = default;
        readonly EcsFilterInject<Inc<IncreaseSpeed>> _increaseSpeedFilterInject = default;
        readonly EcsFilterInject<Inc<DecreaseSpeed>> _decreaseSpeedFilterInject = default;
        readonly EcsFilterInject<Inc<IncreaseAttackSpeed>> _increaseAttackSpeedFilterInject = default;
        readonly EcsFilterInject<Inc<DecreaseAttackSpeed>> _decreaseAttackSpeedFilterInject = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _increaseArmorFilterInject.Value)
            {
                _runtimeData.Value.SpawningDataService.TryChangeData(_worldInject.Value, "UnitData", new IncreaseUnitArmor());

                if(_runtimeData.Value.SpawningDataService.TryGetSpawningData(_worldInject.Value, "UnitData", out var spawningData) && spawningData is UnitData unitData)
                    UpdateAllUnits(unitData);
                
                _worldInject.Value.DelEntity(item);
            }
            foreach (var item in _decreaseArmorFilterInject.Value)
            {
                _runtimeData.Value.SpawningDataService.TryChangeData(_worldInject.Value, "UnitData", new DecreaseUnitArmor());

                if (_runtimeData.Value.SpawningDataService.TryGetSpawningData(_worldInject.Value, "UnitData", out var spawningData) && spawningData is UnitData unitData)
                    UpdateAllUnits(unitData);

                _worldInject.Value.DelEntity(item);
            }
            foreach (var item in _increaseSpeedFilterInject.Value)
            {
                _runtimeData.Value.SpawningDataService.TryChangeData(_worldInject.Value, "UnitData", new IncreaseUnitSpeed());

                if (_runtimeData.Value.SpawningDataService.TryGetSpawningData(_worldInject.Value, "UnitData", out var spawningData) && spawningData is UnitData unitData)
                    UpdateAllUnits(unitData);

                _worldInject.Value.DelEntity(item);
            }
            foreach (var item in _decreaseSpeedFilterInject.Value)
            {
                _runtimeData.Value.SpawningDataService.TryChangeData(_worldInject.Value, "UnitData", new DecreaseUnitSpeed());

                if (_runtimeData.Value.SpawningDataService.TryGetSpawningData(_worldInject.Value, "UnitData", out var spawningData) && spawningData is UnitData unitData)
                    UpdateAllUnits(unitData);

                _worldInject.Value.DelEntity(item);
            }
            foreach (var item in _increaseAttackSpeedFilterInject.Value)
            {
                _runtimeData.Value.SpawningDataService.TryChangeData(_worldInject.Value, "UnitData", new IncreaseUnitAttackSpeed());

                if (_runtimeData.Value.SpawningDataService.TryGetSpawningData(_worldInject.Value, "UnitData", out var spawningData) && spawningData is UnitData unitData)
                    UpdateAllUnits(unitData);

                _worldInject.Value.DelEntity(item);
            }
            foreach (var item in _decreaseAttackSpeedFilterInject.Value)
            {
                _runtimeData.Value.SpawningDataService.TryChangeData(_worldInject.Value, "UnitData", new DecreaseUnitAttackSpeed());

                if (_runtimeData.Value.SpawningDataService.TryGetSpawningData(_worldInject.Value, "UnitData", out var spawningData) && spawningData is UnitData unitData)
                    UpdateAllUnits(unitData);

                _worldInject.Value.DelEntity(item);
            }
        }

        private void UpdateAllUnits(UnitData data)
        {
            var myUnits = _runtimeData.Value.UnitService.GetMyUnits(_worldInject.Value);
            var battleWorld = _runtimeData.Value.BattleWorld;
            var battleStatsPool = battleWorld.GetPool<BattleUnitStats>();
            var thisWorldStatsPool = _worldInject.Value.GetPool<UnitStats>();

            foreach (var item in myUnits)
            {
                ref var stats = ref battleStatsPool.Get(item);
                stats.AttackDistance = data.AttackDistance;
                stats.AttackSpeed = data.AttackSpeed;
                stats.Armor = data.Armor;
                stats.DamageType = data.DamageType;

                if(_runtimeData.Value.UnitService.TryGetPeacefulUnitInt(item, out var peacefulUnit))
                {
                    ref var peacefulStats = ref thisWorldStatsPool.Get(peacefulUnit);
                    peacefulStats.AttackDistance = data.AttackDistance;
                    peacefulStats.AttackSpeed = data.AttackSpeed;
                    peacefulStats.MovementSpeed = data.MovementSpeed;
                    peacefulStats.DamageType = data.DamageType;
                    peacefulStats.Armor = data.Armor;
                }
            }
        }
    }
}