using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MapViewportController : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    [Header("References")]
    public RectTransform viewport;    // Viewport区域
    public RectTransform content;     // 可移动的内容区域

    [Header("Settings")]
    public float dragSpeed = 1f;      // 拖拽速度
    public float zoomSpeed = 0.1f;   // 缩放速度
    public Vector2 zoomRange = new Vector2(0.5f, 3f); // 缩放范围

    private Vector2 lastMousePosition;
    private bool isDragging;

    void Start()
    {
        // 确保布局更新
        Canvas.ForceUpdateCanvases();
        ClampContentPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            lastMousePosition = Input.mousePosition;
            isDragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Middle) return;

        Vector2 currentMousePosition = Input.mousePosition;
        Vector2 delta = (currentMousePosition - lastMousePosition) * dragSpeed;

        // 移动内容
        content.anchoredPosition -= delta / content.localScale.x;

        // 限制移动范围
        ClampContentPosition();

        lastMousePosition = currentMousePosition;
    }

    public void OnScroll(PointerEventData eventData)
    {
        // 获取父级Canvas
        Canvas canvas = viewport.GetComponentInParent<Canvas>();

        // 计算缩放中心点（鼠标位置对应的内容局部坐标）
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            viewport,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint);

        // 计算新的缩放比例
        float newScale = Mathf.Clamp(
            content.localScale.x + eventData.scrollDelta.y * zoomSpeed,
            zoomRange.x,
            zoomRange.y);

        // 计算缩放前后的尺寸变化
        Vector2 scaleChange = new Vector2(
            content.rect.width * (newScale - content.localScale.x),
            content.rect.height * (newScale - content.localScale.y));

        // 调整位置保持缩放中心点不变
        content.localScale = Vector3.one * newScale;
        content.anchoredPosition += (localPoint - content.anchoredPosition) *
                                   (scaleChange / content.sizeDelta);

        // 限制移动范围
        ClampContentPosition();
    }

    private void ClampContentPosition()
    {
        // 获取父级偏移
        RectTransform parentRect = viewport.parent.GetComponent<RectTransform>();
        Vector2 parentOffset = parentRect != null ? parentRect.rect.size : Vector2.zero;

        Vector2 contentSize = content.rect.size * content.localScale.x;
        Vector2 viewportSize = viewport.rect.size;

        // 计算边界限制
        float minX = (viewportSize.x - contentSize.x) - parentOffset.x;
        float minY = (viewportSize.y - contentSize.y) - parentOffset.y;
        float maxX = parentOffset.x;
        float maxY = parentOffset.y;

        // 应用限制
        content.anchoredPosition = new Vector2(
            Mathf.Clamp(content.anchoredPosition.x, minX, maxX),
            Mathf.Clamp(content.anchoredPosition.y, minY, maxY)
        );
    }

    public void OnApplicationFocus(bool hasFocus)
    {
        isDragging = false;
    }
}