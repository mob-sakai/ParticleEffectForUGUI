using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UIElementDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Target m_Target;

    [SerializeField]
    private Transform m_CustomTarget;

    [FormerlySerializedAs("ex2")]
    [SerializeField]
    private bool m_UseCanvasScale;

    private Canvas _canvas;
    private RectTransform _rectTransform;

    private void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = (Vector3)GetLocalDelta(eventData.delta);

        switch (m_Target)
        {
            case Target.Self:
                _rectTransform.localPosition += delta;
                break;
            case Target.Parent:
                _rectTransform.parent.localPosition += delta;
                break;
            case Target.Custom:
                _rectTransform.localPosition += delta;
                if (m_CustomTarget)
                {
                    if (m_UseCanvasScale)
                    {
                        delta.Scale(_canvas.rootCanvas.transform.localScale);
                    }

                    m_CustomTarget.localPosition += delta;
                }

                break;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    private Vector2 GetLocalDelta(Vector2 evDelta)
    {
        switch (_canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            {
                var zero = transform.InverseTransformPoint(Vector2.zero);
                var delta = transform.InverseTransformPoint(evDelta);
                return delta - zero;
            }
            case RenderMode.ScreenSpaceCamera:
            case RenderMode.WorldSpace:
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Vector2.zero,
                    _canvas.worldCamera, out var zero);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, evDelta,
                    _canvas.worldCamera, out var delta);
                return delta - zero;
            }
            default:
                throw new NotSupportedException();
        }
    }

    private enum Target
    {
        Self,
        Parent,
        Custom
    }
}
