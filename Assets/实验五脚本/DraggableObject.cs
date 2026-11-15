using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DraggableObject : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private RectTransform rectTransform;
    private RectTransform viewport;
    private RectTransform content;
    private Canvas canvas;

    private Vector2 startDragPosition;
    private Vector2 startMousePosition;

    private bool isDraggingEnabled = true; // 是否启用拖拽功能

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        content = transform.parent.GetComponent<RectTransform>(); // content 是父物体
        viewport = content.parent.GetComponent<RectTransform>(); // viewport 是 content 的父物体
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDraggingEnabled) return; // 如果拖拽功能禁用，则不处理点击事件

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            startDragPosition = rectTransform.anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position, canvas.worldCamera, out startMousePosition);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingEnabled) return; // 如果拖拽功能禁用，则不处理拖拽事件

        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 将鼠标位置转换为 content 的局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position, canvas.worldCamera, out Vector2 currentMousePosition);
        Vector2 delta = (currentMousePosition - startMousePosition) / content.localScale.x; // 考虑 content 的缩放

        // 计算新的位置
        Vector2 newPosition = startDragPosition + delta;

        // 限制物体在 content 范围内移动
        Vector2 contentSize = content.rect.size;
        Vector2 objectSize = rectTransform.rect.size * rectTransform.localScale;

        float minX = (-contentSize.x * 0.5f) + (objectSize.x * 0.5f);
        float minY = (-contentSize.y * 0.5f) + (objectSize.y * 0.5f);
        float maxX = (contentSize.x * 0.5f) - (objectSize.x * 0.5f);
        float maxY = (contentSize.y * 0.5f) - (objectSize.y * 0.5f);

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rectTransform.anchoredPosition = newPosition;
    }

    // 启用或禁用拖拽功能
    public void SetDraggingEnabled(bool enabled)
    {
        isDraggingEnabled = enabled;
    }
}