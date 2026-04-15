using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 모바일 가상 조이스틱 UI.
/// 터치 드래그로 방향 입력을 제공한다.
/// </summary>
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 1f;

    public Vector2 Direction { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, canvas.worldCamera, out Vector2 localPoint);

        Vector2 normalized = localPoint / (background.sizeDelta * 0.5f);
        Direction = Vector2.ClampMagnitude(normalized, 1f);
        handle.anchoredPosition = Direction * (background.sizeDelta * 0.5f * handleRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
