using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Services
{
    // Все добавления и удаления юнитов должны проходить через этот сервис
    public sealed class UnitService
    {
        private Dictionary<int, EcsPackedEntityWithWorld> _battleUnitsAndHisWorldEntity;

        public UnitService()
        {
            _battleUnitsAndHisWorldEntity = new Dictionary<int, EcsPackedEntityWithWorld>();
        }

        public void RegisterUnit(int battleUnit, EcsPackedEntityWithWorld worldUnit)
        {
            if (_battleUnitsAndHisWorldEntity.ContainsKey(battleUnit))
            {
                Debug.LogWarning("Multiple add unit in collection");
            }
            else
            {
                _battleUnitsAndHisWorldEntity.Add(battleUnit, worldUnit);
            }
        }

        public void RemoveUnit(int battleUnit)
        {
            if (!_battleUnitsAndHisWorldEntity.ContainsKey(battleUnit))
            {
                Debug.LogWarning("Unit is removed earlier");
            }
            else
            {
                _battleUnitsAndHisWorldEntity.Remove(battleUnit);
            }
        }

        public List<int> GetMyEnemies(EcsWorld world)
        {
            var res = new List<int>();

            foreach (var item in _battleUnitsAndHisWorldEntity)
            {
                if(item.Value.Unpack(out var unitWorld, out var unit) && unitWorld != world)
                {
                    res.Add(item.Key);
                }
            }

            return res;
        }

        public List<int> GetMyUnits(EcsWorld world)
        {
            var res = new List<int>();

            foreach (var item in _battleUnitsAndHisWorldEntity)
            {
                if (item.Value.Unpack(out var unitWorld, out var unit) && unitWorld == world)
                {
                    res.Add(item.Key);
                }
            }

            return res;
        }

        public bool TryGetPeacefulUnitInt(int entity, out int packedEntity)
        {
            if (_battleUnitsAndHisWorldEntity.ContainsKey(entity))
            {
                if (_battleUnitsAndHisWorldEntity[entity].Unpack(out var world, out var unpackedEntity))
                {
                    packedEntity = unpackedEntity;
                    return true;
                }
                else
                {
                    packedEntity = 0;
                    return false;
                }
            }
            else
            {
                packedEntity = 0;
                return false;
            }
        }
    }
}