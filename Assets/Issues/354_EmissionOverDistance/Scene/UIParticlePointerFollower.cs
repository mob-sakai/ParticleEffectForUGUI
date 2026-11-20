using UnityEngine;

namespace Coffee.UIParticle.Issue354.Scene
{
    public class UIParticlePointerFollower : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
        }

        public bool SetScreenPosition(Vector2 screenPosition)
        {
            if (canvas == null || _rectTransform == null
                || !(_rectTransform.parent is RectTransform parentRectTransform))
            {
                return false;
            }

            var eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera;
            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay && eventCamera == null)
            {
                return false;
            }

            if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    parentRectTransform, screenPosition, eventCamera, out var worldPosition))
            {
                return false;
            }

            _rectTransform.position = worldPosition;
            return true;
        }
    }
}
