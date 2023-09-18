using Game.Components.Buildings;
using Game.Components.ChooseUnits;
using Game.Components.Shared;
using Game.Components.Unit;
using Game.Worlds;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Systems.Update
{
    sealed class ChooseUnitsSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<ChooseUnitGroupRect, CollectUnits>> _rectsClosedFilter = default;
        readonly EcsFilterInject<Inc<ViewComponent, IsUnit>> _unitTransformFilter = default;
        readonly EcsFilterInject<Inc<ViewComponent, IsBuilding>> _buildTransformFilter = default;
        readonly EcsFilterInject<Inc<UnitIsSelected>> _selectedUnitsFilter = default;

        readonly EcsPoolInject<SelectIsActive> _selectedActivePool = default;
        readonly EcsPoolInject<ChooseUnitGroupRect> _chooseRectsPool = default;
        readonly EcsPoolInject<UnitIsSelected> _selectedUnitPool = default;
        readonly EcsPoolInject<IsDeselected> _deselectedUnitPool = default;
        readonly EcsPoolInject<ViewComponent> _unitTransformPool = default;
        readonly EcsPoolInject<UpdateUIBySelectedObject> _updateUIPool = default;
        #endregion

        void IEcsRunSystem.Run(IEcsSystems systems)
        {
            foreach (var item in _rectsClosedFilter.Value)
            {
                ClearSelected();

                var rect = _chooseRectsPool.Value.Get(item).Rect;

                bool unitAdded = false;
                
                foreach (var unit in _unitTransformFilter.Value)
                {
                    if(CheckObjectInRect(unit, rect))
                    {
                        unitAdded = true;
                        _selectedUnitPool.Value.Add(unit);
                        _selectedActivePool.Value.Add(unit);
                    }
                }

                if(!unitAdded)
                {
                    foreach (var build in _buildTransformFilter.Value)
                    {
                        if (CheckObjectInRect(build, rect))
                        {
                            _selectedUnitPool.Value.Add(build);
                            _selectedActivePool.Value.Add(build);
                        }
                    }
                }

                _world.Value.DelEntity(item);

                var updateUI = _world.Value.NewEntity();
                _updateUIPool.Value.Add(updateUI);
            }
        }

        void ClearSelected()
        {
            foreach (var item in _selectedUnitsFilter.Value)
            {
                _selectedUnitPool.Value.Del(item);
                _deselectedUnitPool.Value.Add(item);
            }
        }

        public bool CheckObjectInRect(int checkedEntity, Rect rect)
        {
            var transform = _unitTransformPool.Value.Get(checkedEntity).Transform;
            var pos = transform.position;

            Vector2 tmp = new Vector2(Camera.main.WorldToScreenPoint(pos).x, Screen.height - Camera.main.WorldToScreenPoint(pos).y);

            var unitRect = CalculateUnitRect(transform);

            return rect.Contains(tmp) || unitRect.Contains(rect.center);
        }

        Rect CalculateUnitRect(Transform transform)
        {
            var view = transform.GetComponentInChildren<Renderer>().bounds;

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