using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WebLinkOpen : MonoBehaviour
{
    private void Start()
    {
        // 获取按钮组件
        Button button = GetComponent<Button>();

        // 检查按钮是否存在
        if (button != null)
        {
            // 绑定点击事件
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("No Button component found on this GameObject!");
        }
    }

    // 按钮点击事件的处理方法
    private void OnButtonClicked()
    {
        string url = "http://10.20.27.68:8081";
        OpenLinkInNewTab(url);
    }

    public void OpenLinkInNewTab(string url)
    {
        // 在 WebGL 中，这将在新标签页打开
        Application.OpenURL(url);
    }

}
