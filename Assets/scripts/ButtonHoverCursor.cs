using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))] // 确保脚本挂载的对象有Button组件
public class ButtonHoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D hoverCursor; // 鼠标悬停时的样式
    public Vector2 hotSpot = Vector2.zero; // 鼠标的热点（默认是左上角）

    private Texture2D defaultCursor; // 默认的鼠标样式
    private Vector2 defaultHotSpot; // 默认的热点
    private CursorMode cursorMode = CursorMode.Auto; // 鼠标模式

    void Start()
    {
        // 默认的鼠标样式是null
        defaultCursor = null;
        defaultHotSpot = Vector2.zero;

        // 动态添加EventTrigger组件（如果不存在）
        if (gameObject.GetComponent<EventTrigger>() == null)
        {
            gameObject.AddComponent<EventTrigger>();
        }
    }

    // 鼠标进入时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 改变鼠标样式
        if (hoverCursor != null)
        {
            Cursor.SetCursor(hoverCursor, hotSpot, cursorMode);
        }
    }

    // 鼠标退出时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        // 恢复默认的鼠标样式
        Cursor.SetCursor(defaultCursor, defaultHotSpot, cursorMode);
    }
}