using Game.Commands;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Update
{
    sealed class TimerSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<TimerQueue>> _timersFilter = default;

        readonly EcsFilterInject<Inc<FreeTimer>> _freeTimersFilter = default;

        readonly EcsPoolInject<TimerQueue> _timersPool = default;

        readonly EcsPoolInject<FreeTimer> _freeTimersPool = default;
        #endregion

        public void Run(IEcsSystems systems)
        {
            foreach (var item in _timersFilter.Value)
            {
                ref var timerQueue = ref _timersPool.Value.Get(item);
                var timer = timerQueue.Value.Peek();
                timer.Timer -= Time.deltaTime;

                if(timer.Timer <= 0)
                {
                    timerQueue.Value.Dequeue();
                    timer.EndTimerAction.Execute();

                    if (timerQueue.Value.Count == 0)
                        _timersPool.Value.Del(item);
                }
            }
            foreach (var item in _freeTimersFilter.Value)
            {
                ref var timer = ref _freeTimersPool.Value.Get(item).Value;
                timer.Timer -= Time.deltaTime;

                if (timer.Timer <= 0)
                {
                    timer.EndTimerAction.Execute();

                    _freeTimersPool.Value.Del(item);
                }
            }
        }
    }

    public struct FreeTimer
    {
        public TimerElement Value;
    }

    public struct TimerQueue
    {
        public Queue<TimerElement> Value;
    }

    public class TimerElement
    {
        public float Timer;
        public Command EndTimerAction;
    }
}