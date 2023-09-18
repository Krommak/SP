using Game.Data.Settings.Weapon;
using Game.Data;
using System.Collections.Generic;

namespace Game.Services
{
    public sealed class WeaponService
    {
        private readonly Dictionary<DamageType, BaseWeaponSettings> _weapons;
        private System.Random _random;

        public WeaponService(AntipersonnelWeaponSettings antipersonnel, ArmorPiercingWeaponSettings armorPiercing, ArtilleryWeaponSettings artillery)
        {
            _weapons = new Dictionary<DamageType, BaseWeaponSettings>();
            _weapons.Add(DamageType.Antipersonnel, antipersonnel);
            _weapons.Add(DamageType.Armor_Piercing, armorPiercing);
            _weapons.Add(DamageType.Artillery, artillery);

            _random = new System.Random();
        }

        public HitResult CulculateHit(DamageType type, int armor)
        {
            return _weapons[type].CalculateResult(_random, armor);
        }
    }

    public sealed class HitResult
    {
        public readonly bool IsMissing;
        public readonly bool IsPenetrate;

        public HitResult(bool isMissing, bool isPenetrate)
        {
            IsMissing = isMissing;
            IsPenetrate = isPenetrate;
        }
    }
}