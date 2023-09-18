using Game.Components.Camera;
using Game.Components.ChooseUnits;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Systems.Update
{
    sealed class PlayerInputSystem : IEcsRunSystem
    {
        bool _rectOpened = false;

        #region Inject
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<ChooseUnitGroupRect>, Exc<ChooseUnitGroupOpened, ChooseUnitGroupClosed>> _rectsEmptyFilter = default;
        readonly EcsFilterInject<Inc<ChooseUnitGroupRect, ChooseUnitGroupOpened>, Exc<ChooseUnitGroupClosed>> _rectsOpenedFilter = default;
        readonly EcsFilterInject<Inc<UnitIsSelected>> _selectedUnitsFilter = default;
        readonly EcsFilterInject<Inc<CameraMovement>> _cameraFilter = default;

        readonly EcsPoolInject<ChooseUnitGroupRect> _chooseRectsPool = default;
        readonly EcsPoolInject<ChooseUnitGroupOpened> _chooseOpenedPool = default;
        readonly EcsPoolInject<ChooseUnitGroupClosed> _chooseClosedPool = default;
        readonly EcsPoolInject<SetTargetForUnit> _setTargetForUnitsPool = default;
        readonly EcsPoolInject<CollectUnits> _collectPool = default;
        readonly EcsPoolInject<CameraMovement> _cameraMovementPool = default;
        #endregion

        Camera _camera;

        public PlayerInputSystem(Camera camera)
        {
            _camera = camera;
        }

        public void Run(IEcsSystems systems)
        {
            SelectInput();
            CameraInput();
        }

        private void SelectInput()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (_rectOpened)
                    CloseRect();

                return;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _rectOpened = true;
                var entity = -1;
                foreach (var item in _rectsEmptyFilter.Value)
                {
                    entity = item;
                }
                if (entity == -1)
                {
                    entity = _world.Value.NewEntity();
                    _chooseRectsPool.Value.Add(entity);
                }

                _chooseOpenedPool.Value.Add(entity);

                ref var rectComponent = ref _chooseRectsPool.Value.Get(entity);

                rectComponent.StartPos = new float2(Input.mousePosition.x, Input.mousePosition.y);
                rectComponent.EndPos = rectComponent.StartPos;
                rectComponent.UpdateRect();
            }
            if (Input.GetKey(KeyCode.Mouse0))
            {
                var entity = -1;
                foreach (var item in _rectsOpenedFilter.Value)
                {
                    entity = item;
                }

                ref var rectComponent = ref _chooseRectsPool.Value.Get(entity);

                rectComponent.EndPos = new float2(Input.mousePosition.x, Input.mousePosition.y);
                rectComponent.UpdateRect();
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                CloseRect();
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                var mousePos = Input.mousePosition;
                mousePos.z = 1;
                Ray castPoint = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if (Physics.Raycast(castPoint, out hit, Camera.main.farClipPlane))
                {
                    foreach (var item in _selectedUnitsFilter.Value)
                    {
                        ref var target = 
                            ref _setTargetForUnitsPool.Value.Has(item) 
                            ? ref _setTargetForUnitsPool.Value.Get(item) 
                            : ref _setTargetForUnitsPool.Value.Add(item);
                        target.Value = new float2(hit.point.x, hit.point.z);
                    }
                }
            }
        }

        private void CameraInput()
        {
            var direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction = Vector3.Lerp(direction, Vector3.forward, 0.5f).normalized;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction = Vector3.Lerp(direction, Vector3.left, 0.5f).normalized;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction = Vector3.Lerp(direction, Vector3.back, 0.5f).normalized;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction = Vector3.Lerp(direction, Vector3.right, 0.5f).normalized;
            }
            if(Input.mousePosition.y >= _camera.pixelHeight-15 
                || Input.mousePosition.x >=_camera.pixelWidth-15
                || Input.mousePosition.x <= 15
                || Input.mousePosition.y <= 15)
            {
                direction = new Vector3(Input.mousePosition.x - _camera.pixelWidth/2, 0, Input.mousePosition.y - _camera.pixelHeight / 2).normalized;
            }
            foreach (var item in _cameraFilter.Value)
            {
                ref var component = ref _cameraMovementPool.Value.Get(item);
                component.Direction = direction;
            }
        }

        private void CloseRect()
        {
            _rectOpened = false;
            var entity = -1;
            foreach (var item in _rectsOpenedFilter.Value)
            {
                entity = item;
            }

            ref var rectComponent = ref _chooseRectsPool.Value.Get(entity);

            rectComponent.EndPos = new float2(Input.mousePosition.x, Input.mousePosition.y);
            rectComponent.UpdateRect();

            _chooseClosedPool.Value.Add(entity);
            _collectPool.Value.Add(entity);
        }
    }
}