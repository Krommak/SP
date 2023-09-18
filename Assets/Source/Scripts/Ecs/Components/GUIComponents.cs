using System;
using UnityEngine;

namespace Game.Components.UI
{
    [Serializable]
    public struct SelectedInfo
    {
        [SerializeField]
        public Transform Transform;
        [SerializeField]
        public GameObject InfoPrefab;
        [SerializeField]
        public GameObject QueuePrefab;
    }

    [Serializable]
    public struct SelectedActions
    {
        [SerializeField]
        public Transform Transform;
        [SerializeField]
        public GameObject ButtonPrefab;
    }

    [Serializable]
    public struct UpdateActions
    {
        [SerializeField]
        public Transform Transform;
        [SerializeField]
        public GameObject ButtonPrefab;
        [SerializeField]
        public Sprite IncreaseIcon;
        [SerializeField]
        public Sprite DecreaseIcon;
        [SerializeField]
        public Color ArmorColor;
        [SerializeField]
        public Color SpeedColor;
        [SerializeField]
        public Color AttackSpeedColor;
    }
}