using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClassController : MonoBehaviour
{
    public Button[] buttons; // 按钮数组
    public GameObject[] panels; // 面板数组

    void Start()
    {
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
        ClassState.activeClassIndex = buttonIndex;

        // 显示对应的面板
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == buttonIndex);
        }
    }
}
