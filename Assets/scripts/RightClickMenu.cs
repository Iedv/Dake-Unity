using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickMenu : MonoBehaviour, IPointerClickHandler
{
    public GameObject popupPanel; // 需要呼出的面板或游戏物体

    private void Start()
    {
        // 初始化时隐藏面板
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    // 检测鼠标点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果是右键点击
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowPopup();
        }
    }

    // 显示面板
    private void ShowPopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }

    // 隐藏面板
    private void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 如果面板已经显示
        if (popupPanel != null && popupPanel.activeSelf)
        {
            // 检测鼠标点击
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                // 判断点击位置是否在面板内
                if (!IsPointerOverPopupPanel())
                {
                    HidePopup();
                }
            }
        }
    }

    // 判断鼠标是否在面板上
    private bool IsPointerOverPopupPanel()
    {
        // 检查鼠标是否在面板的 RectTransform 范围内
        if (RectTransformUtility.RectangleContainsScreenPoint(
            popupPanel.GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            return true;
        }

        // 检查鼠标是否在面板的子物体上（如按钮）
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(popupPanel.transform))
            {
                return true;
            }
        }

        return false;
    }
}