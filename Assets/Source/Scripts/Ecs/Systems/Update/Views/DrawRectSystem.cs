using Game.Components.ChooseUnits;
using Game.MonoBehaviours;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Systems.Update
{
    sealed class DrawRectSystem : IEcsRunSystem
    {
        readonly GUIMono _GUIService;
        #region Inject
        readonly EcsFilterInject<Inc<ChooseUnitGroupRect, ChooseUnitGroupOpened>, Exc<ChooseUnitGroupClosed>> _rectsOpenedFilter = default;

        readonly EcsPoolInject<ChooseUnitGroupRect> _chooseRectsPool = default;
        #endregion

        public DrawRectSystem(GUIMono GUIService)
        {
            _GUIService = GUIService;
        }

        public void Run(IEcsSystems systems)
        {
            Rect rect = new Rect();

            foreach (var item in _rectsOpenedFilter.Value)
            {
                rect = _chooseRectsPool.Value.Get(item).Rect;
            }

            _GUIService.UpdateDrawRect(rect);
        }
    }
}