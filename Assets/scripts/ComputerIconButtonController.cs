using UnityEngine;
using UnityEngine.UI;
public class ComputerIconButtonController : MonoBehaviour
{
    // 绑定控制台面板
    public GameObject commandPanel; // 控制台面板的GameObject

    void Start()
    {
        // 监听按钮点击事件
        GetComponent<Button>().onClick.AddListener(OnComputerIconClick);
    }

    // 电脑图标按钮点击事件
    void OnComputerIconClick()
    {
        // 显示控制台面板
        commandPanel.SetActive(true);
    }
}