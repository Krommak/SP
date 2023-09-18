using Game.Commands;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Components.Shared
{
    internal struct PlayerComponent
    {
        internal int PlayerEntity;
    }

    internal struct ViewComponent
    {
        internal EcsPackedEntityWithWorld EntityInBattleWorld;
        internal Transform Transform;
    }

    internal struct SelectedCircle
    {
        internal GameObject Value;
    }

    internal struct UIActions
    {
        internal List<Command> Value;
    }
}