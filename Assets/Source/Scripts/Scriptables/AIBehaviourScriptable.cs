using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "AIBehaviour", menuName = "Game/Data/AIBehaviour")]
    public class AIBehaviourScriptable : ScriptableObject
    {
        [SerializeField]
        public int CountOfUnitsForAttack;
    }
}