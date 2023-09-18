using Game.Components.Battle;
using Game.Components.Unit;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.Systems
{
    sealed class BattleFXSystem : IEcsRunSystem
    {
        #region Inject
        readonly EcsFilterInject<Inc<BattleViewComponent, AttackState>> _assaulterFilterInject = default;
        readonly EcsFilterInject<Inc<BattleViewComponent>, Exc<AttackState>> _nonAssaulterFilterInject = default;
        #endregion
        
        public void Run(IEcsSystems systems)
        {
            var views = _assaulterFilterInject.Pools.Inc1;
            var attacks = _assaulterFilterInject.Pools.Inc2;

            foreach (var item in _assaulterFilterInject.Value)
            {
                var view = views.Get(item);
                var fx = view.ShotEffect;
                
                var tower = view.Tower;
                var target = attacks.Get(item).TargetUnit;
                
                if (target == null)
                    continue;

                var direction = Vector3.RotateTowards(tower.forward, (target.position - tower.position).normalized, Time.deltaTime*5, 0.0F);

                tower.rotation = Quaternion.LookRotation(direction);

                if (fx.isPlaying)
                    continue;

                fx.Play(true);
            }
            foreach (var item in _nonAssaulterFilterInject.Value)
            {
                var fx = views.Get(item).ShotEffect;
                
                if(fx.isPlaying)
                    fx.Stop(true);
            }
        }
    }
}