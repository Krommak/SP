using Game.Components.Shared;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Game.Systems.Update
{
    sealed class DrawSelectedCircleSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsFilterInject<Inc<SelectIsActive>> _selectedUnitsFilter = default;
        readonly EcsFilterInject<Inc<IsDeselected>> _deselectedUnitsFilter = default;

        readonly EcsPoolInject<SelectIsActive> _selectedActivePool = default;
        readonly EcsPoolInject<SelectedCircle> _selectedCirclesPool = default;
        readonly EcsPoolInject<IsDeselected> _deselectedCirclesPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _deselectedUnitsFilter.Value)
            {
                _selectedCirclesPool.Value.Get(item).Value.SetActive(false);
                _deselectedCirclesPool.Value.Del(item);
            }
            foreach (var item in _selectedUnitsFilter.Value)
            {
                _selectedCirclesPool.Value.Get(item).Value.SetActive(true);
                _selectedActivePool.Value.Del(item);
            }
        }
    }

    public struct SelectIsActive
    {

    }

    public struct IsDeselected
    {
    }

}