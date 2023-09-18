using UnityEngine;

namespace Game.MonoBehaviours
{
    public class GUIMono : MonoBehaviour
    {
        [SerializeField]
        private GUISkin _skin;
        private bool _drawRect;
        private Rect _rect;

        private void OnGUI()
        {
            if (_drawRect)
            {
                GUI.skin = _skin;
                GUI.depth = 99; 
                
                GUI.Box(_rect, "");
            }
        }

        public void UpdateDrawRect(Rect rect)
        {
            _drawRect = rect.size == Vector2.zero ? false : true;

            _rect = rect;
        }
    }
}
