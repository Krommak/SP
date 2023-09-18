using Game.Data;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Components.Battle
{
    internal struct BattleViewComponent
    {
        internal EcsPackedEntityWithWorld EntityInPlayerWorld;
        internal Transform Transform;
        internal Transform Tower;
        internal GameObject Model;
        internal GameObject SelectedCircle;
        internal GameObject DeadEffect;
        internal ParticleSystem ShotEffect;
    }

    internal struct BattleUnitStats
    {
        internal int Armor;
        internal int AttackSpeed;
        internal float AttackDistance;
        internal DamageType DamageType;
    }

    internal struct UnitStats
    {
        internal int Armor;
        internal int AttackSpeed;
        internal float AttackDistance;
        internal float MovementSpeed;
        internal DamageType DamageType;
    }

    internal struct UnitIsDead
    {
    }

    internal struct ShotComponent
    {
        internal EcsPackedEntityWithWorld Assaulter { get; set; }
        internal EcsPackedEntityWithWorld Defender { get; set; }
    }

    internal struct ArtilleryShot
    {
    }

    internal struct AntipersonnelShot
    {
    }

    internal struct ArmorPiersingShot
    {
    }
}