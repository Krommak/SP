using Game.Services;
using UnityEngine;

namespace Game.Data.Settings.Weapon
{
    [CreateAssetMenu(fileName = "ArmorPiercingSettings", menuName = "Game/Settings/ArmorPiercingSettings")]
    public class ArmorPiercingWeaponSettings : BaseWeaponSettings
    {
        public override HitResult CalculateResult(System.Random random, int armor)
        {
            throw new System.NotImplementedException();
        }
    }
}
