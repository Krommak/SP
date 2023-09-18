using Game.Services;
using UnityEngine;

namespace Game.Data.Settings.Weapon
{
    [CreateAssetMenu(fileName = "ArtilleryWeaponSettings", menuName = "Game/Settings/ArtilleryWeaponSettings")]
    public class ArtilleryWeaponSettings : BaseWeaponSettings
    {
        public override HitResult CalculateResult(System.Random random, int armor)
        {
            throw new System.NotImplementedException();
        }
    }
}
