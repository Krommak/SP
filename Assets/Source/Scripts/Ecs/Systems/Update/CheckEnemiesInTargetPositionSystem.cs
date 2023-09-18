using Game.Components.Battle;
using Game.Components.ChooseUnits;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Systems.Update
{
    sealed class CheckEnemiesInTargetPositionSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsCustomInject<RuntimeData> _runtimeData = default;
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<SetTargetForUnit>> _setTargetForUnitsFilter = default;

        readonly EcsPoolInject<SetTargetForUnit> _setTargetForUnitsPool = default;
        readonly EcsPoolInject<FollowToEnemyComponent> _followPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            var battleUnitsPool = _runtimeData.Value.BattleWorld.GetPool<BattleViewComponent>();

            var myEnemies = _runtimeData.Value.UnitService.GetMyEnemies(_world.Value);

            foreach (var item in _setTargetForUnitsFilter.Value)
            {
                var target = _setTargetForUnitsPool.Value.Get(item).Value;

                foreach (var unit in myEnemies)
                {
                    var unitComponent = battleUnitsPool.Get(unit);
                    if (unitComponent.EntityInPlayerWorld.Unpack(out var world, out int unpacked))
                    {
                        if (_world.Value == world) continue;
                    }

                    var tmp = new Vector2(Camera.main.WorldToScreenPoint(
                        new Vector3(target.x, 0, target.y)).x, 
                        Screen.height - Camera.main.WorldToScreenPoint(new Vector3(target.x, 0, target.y)).y);

                    var unitRect = CalculateUnitRect(unitComponent.Transform);
                    if (unitRect.Contains(tmp))
                    {
                        ref var component = 
                            ref _followPool.Value.Has(item) 
                            ? ref _followPool.Value.Get(item) 
                            : ref _followPool.Value.Add(item);
                        component.Value = _runtimeData.Value.BattleWorld.PackEntityWithWorld(unit);
                        break;
                    }
                }
            }
        }

        Rect CalculateUnitRect(Transform transform)
        {
            var renderer = transform.GetComponentInChildren<Renderer>();

            if (renderer == null)
                return new Rect();

            var view = renderer.bounds;

            var viewMin = new Vector3(view.min.x, transform.position.y, view.min.z);
            var viewMax = new Vector3(view.max.x, transform.position.y, view.max.z);

            var fMin = new Vector2(Camera.main.WorldToScreenPoint(viewMin).x, Screen.height - Camera.main.WorldToScreenPoint(viewMin).y);
            var fMax = new Vector2(Camera.main.WorldToScreenPoint(viewMax).x, Screen.height - Camera.main.WorldToScreenPoint(viewMax).y);

            var rectMin = new Vector2(fMin.x, math.min(fMin.y, fMax.y));
            var rectMax = new Vector2(fMax.x, math.max(fMin.y, fMax.y));

            var unitRect = new Rect();
            unitRect.min = rectMin;
            unitRect.max = rectMax;

            return unitRect;
        }
    }
}