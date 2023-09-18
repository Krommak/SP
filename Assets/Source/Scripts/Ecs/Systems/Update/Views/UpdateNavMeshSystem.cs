using Game.Components.Navigation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Update
{
    sealed class UpdateNavMeshSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsWorldInject _world = default;
        readonly EcsFilterInject<Inc<UpdateNavMesh>> _updateFilter = default;
        readonly EcsFilterInject<Inc<NavMeshSurfaceComponent>> _surfacesFilter = default;

        readonly EcsPoolInject<NavMeshSurfaceComponent> _surfacesPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _updateFilter.Value)
            {
                foreach (var surface in _surfacesFilter.Value)
                {
                    var component = _surfacesPool.Value.Get(surface).Value;
                    component.BuildNavMesh();
                }

                _world.Value.DelEntity(item);
            }
        }
    }

    public struct UpdateNavMesh
    {
    }
}