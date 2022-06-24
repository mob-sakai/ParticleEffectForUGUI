using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Target
    {
        Self,
        Parent,
        Custom,
    }

    private RectTransform rectTransform;
    private Canvas canvas;
    public Target m_Target;
    public Transform m_CustomTarget;
    public bool ex2;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var delta = (Vector3)GetLocalDelta(eventData.delta);

        switch (m_Target)
        {
            case Target.Self:
                rectTransform.localPosition += delta;
                break;
            case Target.Parent:
                rectTransform.parent.localPosition += delta;
                break;
            case Target.Custom:
                rectTransform.localPosition += delta;
                if (m_CustomTarget)
                {
                    if (ex2)
                        delta.Scale(canvas.rootCanvas.transform.localScale);
                    m_CustomTarget.localPosition += delta;
                }
                break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    private Vector2 GetLocalDelta(Vector2 evDelta)
    {
        switch (canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                {
                    var zero = transform.InverseTransformPoint(Vector2.zero);
                    var delta = transform.InverseTransformPoint(evDelta);
                    return delta - zero;
                }
            case RenderMode.ScreenSpaceCamera:
                {
                    Vector2 zero, delta;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Vector2.zero, canvas.worldCamera, out zero);
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, evDelta, canvas.worldCamera, out delta);
                    return delta - zero;
                }
            case RenderMode.WorldSpace:
                {
                    Vector3 zero, delta;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, Vector2.zero, canvas.worldCamera, out zero);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, evDelta, canvas.worldCamera, out delta);
                    return delta - zero;
                }
            default:
                throw new System.NotSupportedException();
        }
    }
}