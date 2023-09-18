using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Components.ChooseUnits
{
    internal struct ChooseUnitGroupOpened
    {
    }

    internal struct ChooseUnitGroupRect
    {
        internal float2 StartPos;
        internal float2 EndPos;

        internal Rect Rect { get; private set; }

        internal bool Contains(float2 position)
        {
            return Rect.Contains(position);
        }

        internal void UpdateRect()
        {
            Rect = new Rect(Mathf.Min(EndPos.x, StartPos.x),
                        Screen.height - Mathf.Max(EndPos.y, StartPos.y),
                        Mathf.Max(EndPos.x, StartPos.x) - Mathf.Min(EndPos.x, StartPos.x),
                        Mathf.Max(EndPos.y, StartPos.y) - Mathf.Min(EndPos.y, StartPos.y)
                        );
        }
    }

    internal struct CollectUnits
    {
    }
    
    internal struct ChooseUnitGroupClosed
    {
    }

    internal struct SetTargetForUnit
    {
        internal float2 Value;
    }

    internal struct FollowToEnemyComponent
    {
        internal EcsPackedEntityWithWorld Value; //Entity of target unit
    }
}