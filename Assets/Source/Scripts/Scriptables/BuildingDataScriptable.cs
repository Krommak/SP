using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Game/Data/Build")]
    public class BuildingDataScriptable : ScriptableObject
    {
        [SerializeField]
        GameObject _buildPrefab;
        [SerializeField]
        UnitDataScriptable[] _spawningUnits;

        public BuildData GetData()
        {
            var units = new List<UnitData>();

            foreach (var item in _spawningUnits)
            {
                units.Add(item.GetUnitData());
            }

            return new BuildData(_buildPrefab, units.ToArray());
        }
    }

    public class BuildData
    {
        public GameObject Prefab;
        public UnitData[] SpawningUnits;

        public BuildData(GameObject prefab, UnitData[] spawningUnits)
        {
            Prefab = prefab;
            SpawningUnits = spawningUnits;
        }
    }
}