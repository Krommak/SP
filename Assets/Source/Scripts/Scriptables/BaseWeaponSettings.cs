using Game.Services;
using UnityEngine;

namespace Game.Data.Settings.Weapon
{
    public abstract class BaseWeaponSettings : ScriptableObject
    {
        [SerializeField]
        protected int _diceEdges;
        [SerializeField, Range(1, 6)]
        protected int _accuracy;

        [SerializeField, Range(1, 6)]
        protected int _penetration;

        public abstract HitResult CalculateResult(System.Random random, int armor);
    }

}
