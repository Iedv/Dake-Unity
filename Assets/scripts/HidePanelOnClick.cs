using UnityEngine;
using UnityEngine.UI;

public class HidePanelOnClick : MonoBehaviour
{
    // 引用到要隐藏的面板
    public GameObject panelToHide;

    // 引用到按钮
    public Button buttonToHidePanel;

    void Start()
    {
        // 确保面板和按钮都已经赋值
        if (panelToHide != null && buttonToHidePanel != null)
        {
            // 添加按钮点击事件监听
            buttonToHidePanel.onClick.AddListener(HidePanel);
        }
        else
        {
            Debug.LogError("Panel or Button is not assigned in the inspector.");
        }
    }

    // 隐藏面板的方法
    void HidePanel()
    {
        if (panelToHide != null)
        {
            panelToHide.SetActive(false);
        }
    }
}