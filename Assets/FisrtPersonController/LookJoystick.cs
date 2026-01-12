using UnityEngine;
using UnityEngine.EventSystems;

public class LookJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 60f;
    [SerializeField] private float deadZone = 0.05f;
    [SerializeField] private bool invertY = false;

    public Vector2 Direction { get; private set; }

    private Vector2 pointerDownPos;

    private void Reset()
    {
        background = GetComponent<RectTransform>();
        if (transform.childCount > 0)
            handle = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pointerDownPos
        );

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 delta = localPoint - pointerDownPos;
        delta = Vector2.ClampMagnitude(delta, handleRange);

        handle.anchoredPosition = delta;

        Vector2 normalized = delta / handleRange;

        if (normalized.magnitude < deadZone)
            normalized = Vector2.zero;

        if (invertY)
            normalized.y *= -1f;

        Direction = normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }
}
