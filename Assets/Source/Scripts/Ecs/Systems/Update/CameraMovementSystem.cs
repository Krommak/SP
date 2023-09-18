using Game.Components.Camera;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Update
{
    sealed class CameraMovementSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsFilterInject<Inc<CameraTransform, CameraMovement>> _cameraFilter = default;

        readonly EcsPoolInject<CameraTransform> _cameraTransformPool = default;
        readonly EcsPoolInject<CameraMovement> _cameraMovementPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _cameraFilter.Value)
            {
                ref var transformComponent = ref _cameraTransformPool.Value.Get(item);
                var actualPos = transformComponent.Value.position;
                ref var movementComponent = ref _cameraMovementPool.Value.Get(item);

                var nextPos = actualPos + movementComponent.Direction * movementComponent.MovementSpeed;

                if(movementComponent.CheckPosition(nextPos))
                {
                    transformComponent.Value.position = nextPos;
                }
            }
        }
    }
}