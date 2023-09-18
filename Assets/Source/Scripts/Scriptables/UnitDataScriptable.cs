using Game.Commands;
using Game.Services;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "Game/Data/Unit")]
    public class UnitDataScriptable : ScriptableObject
    {
        [SerializeField]
        GameObject _unitPrefab;
        [SerializeField]
        float _timeForSpawn;
        [SerializeField]
        int _armor;
        [SerializeField]
        int _damage;
        [SerializeField]
        int _attackSpeed;
        [SerializeField]
        float _movementSpeed;
        [SerializeField]
        float _attackDistance;
        [SerializeField]
        DamageType _damageType;

        public UnitData GetUnitData()
        {
            return new UnitData(_unitPrefab, _timeForSpawn, _armor, _damage,
                _attackSpeed, _movementSpeed, _attackDistance, _damageType);
        }
    }

    public class UnitData : ICommandData, ISpawningData
    {
        public GameObject Prefab;
        public Vector3 SpawnPosition;
        public int Armor;
        public int AttackSpeed;
        public float MovementSpeed;
        public float AttackDistance;
        public DamageType DamageType;

        public float TimeForExecute { get; set; }

        public UnitData(GameObject prefab, float timeForSpawn, int armor,
            int damage, int attackSpeed, float movementSpeed, float attackDistance, DamageType damageType)
        {
            Prefab = prefab;
            TimeForExecute = timeForSpawn;
            Armor = armor;
            AttackSpeed = attackSpeed;
            MovementSpeed = movementSpeed;
            AttackDistance = attackDistance;
            DamageType = damageType;
        }

        public void ChangeData(IChangeSpawningData change)
        {
            if(change is IncreaseUnitArmor)
            {
                Armor++;
            }
            else if (change is DecreaseUnitArmor && Armor > 0)
            {
                Armor--;
            }
            else if (change is IncreaseUnitSpeed)
            {
                MovementSpeed++;
            }
            else if (change is DecreaseUnitSpeed && MovementSpeed > 1)
            {
                MovementSpeed--;
            }
            else if (change is IncreaseUnitAttackSpeed)
            {
                AttackSpeed++;
            }
            else if (change is DecreaseUnitAttackSpeed && AttackSpeed > 1)
            {
                AttackSpeed--;
            }
        }
    }

    public class IncreaseUnitArmor : IChangeSpawningData
    {

    }
    public class DecreaseUnitArmor : IChangeSpawningData
    {

    }
    public class IncreaseUnitSpeed : IChangeSpawningData
    {

    }
    public class DecreaseUnitSpeed : IChangeSpawningData
    {

    }
    public class IncreaseUnitAttackSpeed : IChangeSpawningData
    {

    }
    public class DecreaseUnitAttackSpeed : IChangeSpawningData
    {

    }

    public enum DamageType
    {
        Armor_Piercing,
        Antipersonnel,
        Artillery
    }
}