using AB_Utility.FromSceneToEntityConverter;
using Game.MonoBehaviours;
using Game.Systems;
using Game.Systems.Init;
using Game.Systems.Update;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Worlds
{
    public abstract class GameWorld
    {
        EcsWorld _world;
        protected EcsSystems _initSystems;
        protected EcsSystems _updateSystems;
        protected EcsSystems _fixedUpdateSystems;
        protected EcsSystems _lateUpdateSystems;

        public virtual GameWorld Init(EcsWorld world)
        {
            _world = world;

            _initSystems = new EcsSystems(world);
            _updateSystems = new EcsSystems(world);
            _fixedUpdateSystems = new EcsSystems(world);
            _lateUpdateSystems = new EcsSystems(world);

            return this;
        }

        public abstract GameWorld FillSystems(GUIMono GUIService);

        public virtual GameWorld Inject(StartupData startupData, RuntimeData runtimeData)
        {
            _initSystems.Inject(startupData, runtimeData);
            _updateSystems.Inject(startupData, runtimeData);
            _fixedUpdateSystems.Inject(startupData, runtimeData);
            _lateUpdateSystems.Inject(startupData, runtimeData);

            return this;
        }

        public virtual GameWorld InitSystems()
        {
            _initSystems.Init();
            _updateSystems.Init();
            _fixedUpdateSystems.Init();
            _lateUpdateSystems.Init();

            return this;
        }

        public virtual void UpdateSystems()
        {
            _updateSystems.Run();
        }

        public virtual void FixedUpdateSystems()
        {
            _fixedUpdateSystems.Run();
        }

        public virtual void LateUpdateSystems()
        {
            _lateUpdateSystems.Run();
        }

        public virtual void Destroy()
        {
            if (_initSystems != null)
            {
                _initSystems.Destroy();
                _initSystems = null;
            }
            if (_updateSystems != null)
            {
                _updateSystems.Destroy();
                _updateSystems = null;
            }
            if (_fixedUpdateSystems != null)
            {
                _fixedUpdateSystems.Destroy();
                _fixedUpdateSystems = null;
            }

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }

    }

    public class PlayerWorld : GameWorld
    {
        public override GameWorld FillSystems(GUIMono GUIService)
        {
            _initSystems
                .Add(new InitPlayerSystem())
                .ConvertScene();

            _updateSystems
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Add(new BuildSystem())
                .Add(new UpdateNavMeshSystem())
                .Add(new PlayerInputSystem(Camera.main))
                .Add(new DrawRectSystem(GUIService))
                .Add(new ChooseUnitsSystem())
                .Add(new DrawSelectedCircleSystem())
                .Add(new CheckEnemiesInTargetPositionSystem())
                .Add(new MoveToEnemySystem())
                .Add(new AttackSystem())
                .Add(new SetTargetPositionsSystem())
                .Add(new UpdateActionsSystem())
                .Add(new UpdateUnitStats())
                .Add(new TimerSystem());

            _lateUpdateSystems
                .Add(new CameraMovementSystem());

            return this;
        }
    }

    public class AIWorld : GameWorld
    {
        public override GameWorld FillSystems(GUIMono GUIService)
        {
            _initSystems
                .Add(new InitAISystem());

            _updateSystems
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Add(new BuildSystem())
                .Add(new UpdateNavMeshSystem())
                .Add(new AIBehaviourSystem())
                .Add(new CheckEnemiesInTargetPositionSystem())
                .Add(new MoveToEnemySystem())
                .Add(new AttackSystem())
                .Add(new SetTargetPositionsSystem())
                .Add(new TimerSystem());

            return this;
        }
    }

    public class BattleWorld : GameWorld
    {
        public override GameWorld FillSystems(GUIMono GUIService)
        {
            _updateSystems
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Add(new BattleSystem())
                .Add(new DestroyUnitSystem())
                .Add(new TimerSystem())
                .Add(new BattleFXSystem());

            return this;
        }
    }
}