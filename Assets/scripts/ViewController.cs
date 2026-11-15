using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ViewController : MonoBehaviour
{
    public Button[] buttons; // 按钮数组
    public GameObject[] panels; // 面板数组
    private Image[] buttonBackgrounds; // 按钮的背景 Image 组件数组
    private int lastActiveButtonIndex = -1; // 上一次被点击的按钮索引

    void Start()
    {
        // 初始化按钮的背景 Image 组件数组
        buttonBackgrounds = new Image[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonBackgrounds[i] = buttons[i].GetComponent<Image>();
        }

        // 绑定按钮点击事件
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // 避免闭包问题
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        // 更新全局变量
        GlobalState.activeButtonIndex = buttonIndex;

        // 显示对应的面板
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == buttonIndex);
        }

        // 恢复上一个按钮的背景透明度
        if (lastActiveButtonIndex != -1)
        {
            SetButtonBackgroundAlpha(lastActiveButtonIndex, 0); // 恢复透明度为 0
        }

        // 更新当前按钮的背景透明度
        SetButtonBackgroundAlpha(buttonIndex, 128); // 设置透明度为 128

        // 更新上一次被点击的按钮索引
        lastActiveButtonIndex = buttonIndex;
    }

    void SetButtonBackgroundAlpha(int buttonIndex, byte alpha)
    {
        // 获取按钮的背景颜色
        Color backgroundColor = buttonBackgrounds[buttonIndex].color;

        // 修改颜色的透明度
        backgroundColor.a = alpha / 255f; // 将 alpha 值从 [0, 255] 映射到 [0, 1]

        // 更新按钮的背景颜色
        buttonBackgrounds[buttonIndex].color = backgroundColor;
    }
}
