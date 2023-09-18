using System;
using UnityEngine;

namespace Game.Components.Camera
{
    [Serializable]
    public struct CameraTransform
    {
        public Transform Value;
    }

    [Serializable]
    public struct CameraMovement
    {
        [SerializeField]
        private Bounds MovementBounds;
        public Vector3 Direction;
        public float MovementSpeed;

        public bool CheckPosition(Vector3 position)
        {
            return MovementBounds.Contains(position);
        }
    }
}