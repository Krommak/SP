using Game.Components.Shared;
using Game.Components.UI;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Systems.Update
{
    sealed class UpdateActionsSystem : IEcsInitSystem, IEcsRunSystem
    {
        #region Inject
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<UpdateUIBySelectedObject>> _updateUIFilter = default;
        readonly EcsFilterInject<Inc<SelectedActions>> _selectedActionsUIFilter = default;
        readonly EcsFilterInject<Inc<UnitIsSelected>> _selectedUnitsFilter = default;
        readonly EcsFilterInject<Inc<UpdateActions>> _updateActionsFilter = default;
        readonly EcsPoolInject<SelectedActions> _selectedActionsPool = default;
        readonly EcsPoolInject<UIActions> _actionsOfObjectsPool = default;
        readonly EcsPoolInject<UpdateActions> _updateActionsPool = default;
        #endregion

        public void Init(IEcsSystems systems)
        {
            foreach (var item in _updateActionsFilter.Value)
            {
                ref var component = ref _updateActionsPool.Value.Get(item);

                var transform = component.Transform;

                var increaseArmorButton = GameObject.Instantiate(component.ButtonPrefab, transform);
                increaseArmorButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var entity = _world.Value.NewEntity();
                    _world.Value.GetPool<IncreaseArmor>().Add(entity);
                });
                var image = increaseArmorButton.GetComponent<Image>();
                image.sprite = component.IncreaseIcon;
                image.color = component.ArmorColor;

                var increaseAttackSpeed = GameObject.Instantiate(component.ButtonPrefab, transform);
                increaseAttackSpeed.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var entity = _world.Value.NewEntity();
                    _world.Value.GetPool<IncreaseAttackSpeed>().Add(entity);
                });
                image = increaseAttackSpeed.GetComponent<Image>();
                image.sprite = component.IncreaseIcon;
                image.color = component.AttackSpeedColor;
                
                var increaseSpeed = GameObject.Instantiate(component.ButtonPrefab, transform);
                increaseSpeed.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var entity = _world.Value.NewEntity();
                    _world.Value.GetPool<IncreaseSpeed>().Add(entity);
                });
                image = increaseSpeed.GetComponent<Image>();
                image.sprite = component.IncreaseIcon;
                image.color = component.SpeedColor;

                var dencreaseArmorButton = GameObject.Instantiate(component.ButtonPrefab, transform);
                dencreaseArmorButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var entity = _world.Value.NewEntity();
                    _world.Value.GetPool<DecreaseArmor>().Add(entity);
                });
                image = dencreaseArmorButton.GetComponent<Image>();
                image.sprite = component.DecreaseIcon;
                image.color = component.ArmorColor;

                var dencreaseAttackSpeed = GameObject.Instantiate(component.ButtonPrefab, transform);
                dencreaseAttackSpeed.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var entity = _world.Value.NewEntity();
                    _world.Value.GetPool<DecreaseAttackSpeed>().Add(entity);
                });
                image = dencreaseAttackSpeed.GetComponent<Image>();
                image.sprite = component.DecreaseIcon;
                image.color = component.AttackSpeedColor;

                var dencreaseSpeed = GameObject.Instantiate(component.ButtonPrefab, transform);
                dencreaseSpeed.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var entity = _world.Value.NewEntity();
                    _world.Value.GetPool<DecreaseSpeed>().Add(entity);
                });
                image = dencreaseSpeed.GetComponent<Image>();
                image.sprite = component.DecreaseIcon;
                image.color = component.SpeedColor;
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var update in _updateUIFilter.Value)
            {
                foreach (var element in _selectedActionsUIFilter.Value)
                {
                    var uiViewComponent = _selectedActionsPool.Value.Get(element);

                    foreach (Transform child in uiViewComponent.Transform)
                    {
                        child.GetComponent<Button>().onClick.RemoveAllListeners();
                        GameObject.Destroy(child.gameObject);
                    }

                    int firstSelected = -1;

                    var counter = 0;
                    foreach (var selected in _selectedUnitsFilter.Value)
                    {
                        if (counter != 0) break;

                        firstSelected = selected;
                        counter++;
                    }

                    if (firstSelected == -1) continue;

                    var selectedPacked = _world.Value.PackEntityWithWorld(firstSelected);
                    var actions = _actionsOfObjectsPool.Value.Get(firstSelected).Value;

                    foreach (var action in actions)
                    {
                        var button = GameObject.Instantiate(uiViewComponent.ButtonPrefab, uiViewComponent.Transform);
                        button.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if(selectedPacked.Unpack(out var world, out int selectedEntity))
                            {
                                var timersPool = _world.Value.GetPool<TimerQueue>();

                                ref var timerQueue = ref timersPool.Has(selectedEntity) ? ref timersPool.Get(selectedEntity) : ref timersPool.Add(selectedEntity);

                                if (timerQueue.Value == null)
                                    timerQueue.Value = new Queue<TimerElement>();

                                var timer = new TimerElement();
                                timer.Timer = action.TimeForExecute;
                                timer.EndTimerAction = action;

                                timerQueue.Value.Enqueue(timer);
                            }
                        });
                    }
                }

                _world.Value.DelEntity(update);
            }
        }
    }

    public struct UpdateUIBySelectedObject
    {
    }
}