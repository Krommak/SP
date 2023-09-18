using Game.Components.Battle;
using Game.Components.ChooseUnits;
using Game.Components.Shared;
using Game.Components.Unit;
using Game.Data;
using Game.Services;
using Leopotam.EcsLite;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Commands
{
    public abstract class Command
    {
        protected ICommandData data;
        public float TimeForExecute;

        public Command(ICommandData data)
        {
            this.data = data;
            TimeForExecute = data.TimeForExecute;
        }

        public abstract void Execute();
    }

    public interface ICommandData
    {
        public float TimeForExecute { get; set; }
    }

    public class SpawnUnitCommand : Command
    {
        private string _unitName;
        private EcsWorld _world;
        private EcsWorld _battleWorld;
        private UnitService _unitService;
        private SpawningDataService _spawningDataService;

        public SpawnUnitCommand(SpawnUnitCommandData data) : base(data)
        {
            _world = data.World;
            _battleWorld = data.BattleWorld;
            _unitService = data.RuntimeData.UnitService;
            _spawningDataService = data.RuntimeData.SpawningDataService;
            _unitName = data.UnitName;
        }

        public override void Execute()
        {
            if (_spawningDataService.TryGetSpawningData(_world, _unitName, out var data) && data is UnitData unitData)
            {
                var unitGO = GameObject.Instantiate(unitData.Prefab, unitData.SpawnPosition, Quaternion.identity);
                var newUnit = _world.NewEntity();
                var newBattleUnit = _battleWorld.NewEntity();

                //components for player
                ref var view = ref _world.GetPool<ViewComponent>().Add(newUnit);
                view.Transform = unitGO.transform;
                view.EntityInBattleWorld = _battleWorld.PackEntityWithWorld(newBattleUnit);

                ref var navigation = ref _world.GetPool<UnitNavMeshAgentComponent>().Add(newUnit);
                navigation.Value = unitGO.GetComponent<NavMeshAgent>();

                ref var selectedCircle = ref _world.GetPool<SelectedCircle>().Add(newUnit);
                var selCircle = unitGO.transform.Find("SelectedCircle").gameObject;
                selectedCircle.Value = selCircle;
                selectedCircle.Value.SetActive(false);

                ref var actionsComponent = ref _world.GetPool<UIActions>().Add(newUnit);
                var actions = new List<Command>();

                // Реализовать добавление команд для юнитов

                actionsComponent.Value = actions;

                _world.GetPool<IsUnit>().Add(newUnit);

                ref var setTargetComponent = ref _world.GetPool<SetTargetForUnit>().Add(newUnit);
                var targetPosition = unitGO.transform.position + unitGO.transform.forward * 7.5f;
                setTargetComponent.Value = new float2(targetPosition.x, targetPosition.z);

                ref var stats = ref _world.GetPool<UnitStats>().Add(newUnit);
                stats.Armor = unitData.Armor;
                stats.AttackSpeed = unitData.AttackSpeed;
                stats.MovementSpeed = unitData.MovementSpeed;
                stats.AttackDistance = unitData.AttackDistance;
                stats.DamageType = unitData.DamageType;

                navigation.Value.speed = stats.MovementSpeed;

                //components for battle
                ref var battleView = ref _battleWorld.GetPool<BattleViewComponent>().Add(newBattleUnit);
                battleView.Transform = unitGO.transform;
                var model = unitGO.transform.Find("Model").gameObject;
                battleView.Model = model;
                battleView.SelectedCircle = model;
                var tower = unitGO.transform.Find("Model/Weapon/Tower");
                battleView.Tower = tower;
                var dead = unitGO.transform.Find("DeadFX").gameObject;
                dead.SetActive(false);
                battleView.DeadEffect = dead;
                var shot = tower.Find("ShotFX").GetComponent<ParticleSystem>();
                battleView.ShotEffect = shot;
                battleView.EntityInPlayerWorld = _world.PackEntityWithWorld(newUnit);

                ref var battleStats = ref _battleWorld.GetPool<BattleUnitStats>().Add(newBattleUnit);
                battleStats.Armor = unitData.Armor;
                battleStats.AttackDistance = unitData.AttackDistance;
                battleStats.AttackSpeed = unitData.AttackSpeed;
                battleStats.DamageType = unitData.DamageType;

                _unitService.RegisterUnit(newBattleUnit, _world.PackEntityWithWorld(newUnit));
            }
            else
            {
                Debug.LogError($"Data {_unitName} not found");
            }
        }
    }

    
    public class SpawnUnitCommandData : ICommandData
    {
        public float TimeForExecute { get; set; }
        public string UnitName { get; set; }
        public RuntimeData RuntimeData { get; set; }
        public EcsWorld World { get; set; }
        public EcsWorld BattleWorld { get; set; }
    }

    public class AddAttackComponentByTimerEnd : Command
    {
        private AttackData _attackData;

        public AddAttackComponentByTimerEnd(AttackData data) : base(data)
        {
            _attackData = data;
        }

        public override void Execute()
        {
            if (_attackData.AssaulterIsAlive(out var assaulterWorld, out var assaulter)
                && _attackData.DefenderIsAlive(out var defenderWorld, out var defender))
            {
                var newAttack = assaulterWorld.NewEntity();

                var shots = assaulterWorld.GetPool<ShotComponent>();
                ref var shot = ref shots.Add(newAttack);
                shot.Assaulter = _attackData.PackedAssaulter;
                shot.Defender = _attackData.PackedDefender;

                switch (_attackData.DamageType)
                {
                    case DamageType.Armor_Piercing:
                        {
                            var attackType = assaulterWorld.GetPool<ArmorPiersingShot>();
                            attackType.Add(newAttack);
                            break;
                        }
                    case DamageType.Antipersonnel:
                        {
                            var attackType = assaulterWorld.GetPool<AntipersonnelShot>();
                            attackType.Add(newAttack);
                            break;
                        }
                    case DamageType.Artillery:
                        {
                            var attackType = assaulterWorld.GetPool<ArtilleryShot>();
                            attackType.Add(newAttack);
                            break;
                        }
                }

                var viewComponent = assaulterWorld.GetPool<BattleViewComponent>().Get(assaulter);
                if (viewComponent.EntityInPlayerWorld.Unpack(out var playerWorld, out var unpackedAssaulterInHisWorld))
                {
                    var playerWorldAttacks = playerWorld.GetPool<AttackState>();
                    var battleAttacks = assaulterWorld.GetPool<AttackState>();

                    if(playerWorldAttacks.Has(unpackedAssaulterInHisWorld))
                        playerWorldAttacks.Del(unpackedAssaulterInHisWorld);

                    if (battleAttacks.Has(assaulter))
                        battleAttacks.Del(assaulter);
                }
            }
            else if (_attackData.AssaulterIsAlive(out var assaulterWorld1, out var assaulter1)
                && !_attackData.DefenderIsAlive(out var defenderWorld1, out var defender1))
            {
                var viewComponent = assaulterWorld1.GetPool<BattleViewComponent>().Get(assaulter1);
                if (viewComponent.EntityInPlayerWorld.Unpack(out var playerWorld, out var unpackedAssaulterInHisWorld))
                {
                    assaulterWorld1.GetPool<AttackState>().Del(unpackedAssaulterInHisWorld);
                    playerWorld.GetPool<AttackState>().Del(unpackedAssaulterInHisWorld);
                }
            }
        }
    }

    public class AttackData : ICommandData
    {
        public EcsPackedEntityWithWorld PackedAssaulter;
        public EcsPackedEntityWithWorld PackedDefender;
        public DamageType DamageType;
        public float TimeForExecute { get; set; }

        public AttackData(EcsPackedEntityWithWorld packedAssaulter, EcsPackedEntityWithWorld packedDefender,
            float timeForExecute, DamageType damageType)
        {
            TimeForExecute = timeForExecute;
            PackedAssaulter = packedAssaulter;
            PackedDefender = packedDefender;
            DamageType = damageType;
        }

        public bool AssaulterIsAlive(out EcsWorld world, out int entity)
        {
            var result = PackedAssaulter.Unpack(out var unitWorld, out var unit);

            if (unitWorld.GetPool<UnitIsDead>().Has(unit))
                result = false;

            world = unitWorld;
            entity = unit;
            return result;
        }

        public bool DefenderIsAlive(out EcsWorld world, out int entity)
        {
            var result = PackedAssaulter.Unpack(out var unitWorld, out var unit);
            world = unitWorld;
            entity = unit;
            return result;
        }
    }

    public class FullDestroyUnit : Command
    {
        EcsPackedEntityWithWorld _battleUnit;
        RuntimeData _runtimeData;

        public FullDestroyUnit(DestroyUnitData data) : base(data)
        {
            _battleUnit = data.BattleUnit;
            _runtimeData = data.RuntimeData;
        }

        public override void Execute()
        {
            if(_battleUnit.Unpack(out var battleWorld, out var deadUnit))
            {
                var battleView = battleWorld.GetPool<BattleViewComponent>().Get(deadUnit);

                if(battleView.EntityInPlayerWorld.Unpack(out var unitWorld, out var unit))
                {
                    unitWorld.DelEntity(unit);
                }
                GameObject.Destroy(battleView.Transform.gameObject);
                battleWorld.DelEntity(deadUnit);

                _runtimeData.UnitService.RemoveUnit(deadUnit);
            }
        }
    }

    public class DestroyUnitData : ICommandData
    {
        public EcsPackedEntityWithWorld BattleUnit;
        public RuntimeData RuntimeData;

        public float TimeForExecute { get; set; }

        public DestroyUnitData(RuntimeData runtimeData, EcsPackedEntityWithWorld battleUnit, float timeForExecute)
        {
            TimeForExecute = timeForExecute;
            BattleUnit = battleUnit;
            RuntimeData = runtimeData;
        }
    }

    public class BuffUnitCommand : Command
    {
        public BuffUnitCommand(ICommandData data) : base(data)
        {
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
