using UnityEngine;
using UnityEngine.UI;

public class PanelButtonController : MonoBehaviour
{
    [System.Serializable]
    public class PanelButtonPair
    {
        public Button button;
        public GameObject panel;
    }

    [Header("面板和按钮对应关系")]
    [Tooltip("将按钮和对应的面板拖拽到这里")]
    public PanelButtonPair[] panelButtonPairs;

    private GameObject currentlyActivePanel;

    void Start()
    {
        // 初始化所有面板为未激活状态
        foreach (var pair in panelButtonPairs)
        {
            if (pair.panel != null)
            {
                pair.panel.SetActive(false);
            }
        }

        // 为每个按钮添加点击事件
        for (int i = 0; i < panelButtonPairs.Length; i++)
        {
            if (panelButtonPairs[i].button != null && panelButtonPairs[i].panel != null)
            {
                int index = i; // 创建局部变量避免闭包问题
                panelButtonPairs[i].button.onClick.AddListener(() => TogglePanel(index));
            }
        }

        // 可选：默认激活第一个面板
        if (panelButtonPairs.Length > 0 && panelButtonPairs[0].panel != null)
        {
            panelButtonPairs[0].panel.SetActive(true);
            currentlyActivePanel = panelButtonPairs[0].panel;
        }
    }

    void TogglePanel(int panelIndex)
    {
        // 如果点击的面板已经是激活状态，可以选择不做任何操作
        // 或者如果你想点击已激活的面板时关闭它，可以取消下面注释
        /*
        if (currentlyActivePanel == panelButtonPairs[panelIndex].panel)
        {
            currentlyActivePanel.SetActive(false);
            currentlyActivePanel = null;
            return;
        }
        */

        // 隐藏当前激活的面板
        if (currentlyActivePanel != null)
        {
            currentlyActivePanel.SetActive(false);
        }

        // 显示新的面板
        panelButtonPairs[panelIndex].panel.SetActive(true);
        currentlyActivePanel = panelButtonPairs[panelIndex].panel;
    }
}