using UnityEngine;

namespace Game.Components.Buildings
{
    internal struct IsBuilding
    {
    }

    internal struct CreateBuilding
    {
        internal int OwnerEntity;
        internal string BuildName;
        internal Vector3 SpawnPosition;
    }
}