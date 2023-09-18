using Game.Components.Battle;
using Game.Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems
{
    sealed class BattleSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<StartupData> _startupData = default;
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<ShotComponent, AntipersonnelShot>> _antipersonnelShotFilter = default;

        readonly EcsPoolInject<BattleUnitStats> _statsPool = default;
        readonly EcsPoolInject<UnitIsDead> _deadPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            CalculateAntipersonnelDamage();
        }

        private void CalculateAntipersonnelDamage()
        {
            foreach (var item in _antipersonnelShotFilter.Value)
            {
                var component = _antipersonnelShotFilter.Pools.Inc1.Get(item);

                int armor;

                if (component.Assaulter.Unpack(out var assaulterWorld, out var assaulter) &&
                    component.Defender.Unpack(out var defenderWorld, out var defender))
                {
                    armor = _statsPool.Value.Get(defender).Armor;
                }
                else
                {
                    continue;
                }

                if (_deadPool.Value.Has(defender) || _deadPool.Value.Has(assaulter))
                    continue;

                var result = _startupData.Value.WeaponService.CulculateHit(DamageType.Antipersonnel, armor);

                if (result.IsPenetrate)
                {
                    _deadPool.Value.Add(defender);
                }

                _world.Value.DelEntity(item);
            }
        }
    }
}