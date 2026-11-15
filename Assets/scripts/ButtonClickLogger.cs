using UnityEngine;
using UnityEngine.UI;

public class ButtonClickLogger : MonoBehaviour
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
        Debug.Log("Button was clicked!");
    }
}