using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonTextColorChanger : MonoBehaviour
{
    // 按钮数组
    public Button[] buttons;

    // TMP文本数组
    public TextMeshProUGUI[] texts;

    // 记录当前被选中文本的索引
    private int _currentSelectedIndex = -1;

    // 默认文本颜色
    private Color _defaultColor = Color.black; // 你可以根据需要修改默认颜色

    void Start()
    {
        // 确保按钮和文本数组长度一致
        if (buttons.Length != texts.Length)
        {
            Debug.LogError("按钮和文本数组长度不一致！");
            return;
        }

        // 为每个按钮添加点击事件监听
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 捕获当前索引
            buttons[i].onClick.AddListener(() => ChangeTextColor(index));
        }
    }

    // 改变对应TMP文本的颜色
    void ChangeTextColor(int index)
    {
        // 如果当前有文本被选中，恢复其颜色
        if (_currentSelectedIndex != -1)
        {
            texts[_currentSelectedIndex].color = _defaultColor;
        }

        // 如果点击的是同一个按钮，取消选中状态
        if (_currentSelectedIndex == index)
        {
            _currentSelectedIndex = -1; // 取消选中
        }
        else
        {
            // 改变新点击的文本颜色为蓝色
            texts[index].color = Color.blue;
            _currentSelectedIndex = index; // 更新当前选中的索引
        }
    }
}