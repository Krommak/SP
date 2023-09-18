using Game.Data;
using Game.Data.Settings.Weapon;
using Game.MonoBehaviours;
using Game.Services;
using Game.Worlds;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField]
        StartupData _startupData;
        RuntimeData _runtimeData;

        Dictionary<WorldType, GameWorld> _worlds = new Dictionary<WorldType, GameWorld>();

        void Start()
        {
            _runtimeData = new RuntimeData();

            foreach (var item in (WorldType[])Enum.GetValues(typeof(WorldType)))
            {
                var worldNew = new EcsWorld();

                var type = Type.GetType($"Game.Worlds.{item}");
                var world = (GameWorld)Activator.CreateInstance(type);
                world.Init(worldNew)
                    .FillSystems(GetComponent<GUIMono>())
                    .Inject(_startupData, _runtimeData)
                    .InitSystems();
                _worlds.Add(item, world);

                if(item == WorldType.BattleWorld)
                {
                    _runtimeData.BattleWorld = worldNew;
                }
            }
        }

        void Update()
        {
            foreach (var world in _worlds)
            {
                world.Value.UpdateSystems();
            }
        }

        void FixedUpdate()
        {
            foreach (var world in _worlds)
            {
                world.Value.FixedUpdateSystems();
            }
        }

        void LateUpdate()
        {
            foreach (var world in _worlds)
            {
                world.Value.LateUpdateSystems();
            }
        }

        void OnDestroy()
        {
            foreach (var world in _worlds)
            {
                world.Value.Destroy();
            }
        }
    }

    internal enum WorldType
    {
        PlayerWorld,
        AIWorld,
        BattleWorld
    }

    public class RuntimeData
    {
        public EcsWorld BattleWorld;
        private UnitService _unitService;
        private SpawningDataService _spawningDataService;

        public UnitService UnitService
        {
            get
            {
                if (_unitService == null)
                    _unitService = new UnitService();

                return _unitService;
            }
            set
            {
            }
        }

        public SpawningDataService SpawningDataService
        {
            get
            {
                if (_spawningDataService == null)
                    _spawningDataService = new SpawningDataService();

                return _spawningDataService;
            }
            set
            {
            }
        }
    }

    [Serializable]
    public class StartupData
    {
        [SerializeField]
        public AIBehaviourScriptable AIBehaviour;

        [SerializeField]
        private AntipersonnelWeaponSettings _antipersonnelWeaponSettings;
        [SerializeField]
        private ArmorPiercingWeaponSettings _armorPiercingWeaponSettings;
        [SerializeField]
        private ArtilleryWeaponSettings _artilleryWeaponSettings;

        private WeaponService _weaponService;

        public WeaponService WeaponService 
        { 
            get
            {
                if (_weaponService == null)
                    _weaponService = new WeaponService(_antipersonnelWeaponSettings, _armorPiercingWeaponSettings, _artilleryWeaponSettings);

                return _weaponService;
            }
            set
            {
            }
        }

        [SerializeField]
        private List<Vector3> _basePositions = new List<Vector3>
        {
            new Vector3(-40, 0.0f, -40),
            new Vector3(40, 0.0f, 40)
        };

        private int baseNum = 0;

        public Vector3 BasePosition
        {
            get
            {
                if(baseNum > _basePositions.Count)
                {
                    return new Vector3();
                }
                var value = _basePositions[baseNum];
                baseNum++;
                return value;
            }
        }
    }
}