using Game.Services;
using UnityEngine;

namespace Game.Data.Settings.Weapon
{
    [CreateAssetMenu(fileName = "AntipersonnelSettings", menuName = "Game/Settings/AntipersonnelWeapon")]
    public class AntipersonnelWeaponSettings : BaseWeaponSettings
    {
        public override HitResult CalculateResult(System.Random random, int armor)
        {
            var isMissing = false;
            var isPenetrate = false;

            if(random.Next(_diceEdges) >= _accuracy)
            {
                isPenetrate = true;

                if(armor != 0)
                    isPenetrate = random.Next(_diceEdges * armor * _penetration) >= armor * _penetration;
            }
            else if(random.Next(_diceEdges) < _accuracy)
            {
                isMissing = true;
            }

            return new HitResult(isMissing, isPenetrate);
        }
    }
}
