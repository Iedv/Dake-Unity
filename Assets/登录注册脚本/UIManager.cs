using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 手动拖拽绑定的UI元素
    public GameObject loginPanel; // 登录界面
    public GameObject promptPanel; // 提示框
    public Text promptText; // 提示框的文本
    public GameObject[] navigationButtons; // 导航栏按钮（数组形式，支持多个按钮）
    public GameObject coursePanel; // 课程区面板

    public GameObject managePanel;

    private void Start()
    {
        // 初始化UI状态
        SetNavigationButtonsVisible(false); // 导航栏按钮默认隐藏
        coursePanel.SetActive(false); // 课程区默认隐藏
        promptPanel.SetActive(false); // 提示框默认隐藏

        managePanel.SetActive(false);
    }

    // 显示提示框
    public void ShowPrompt(string message)
    {
        promptText.text = message;
        promptPanel.SetActive(true);
    }

    // 登录成功后的UI更新
    public void OnLoginSuccess()
    {
        loginPanel.SetActive(false); // 隐藏登录界面
        SetNavigationButtonsVisible(true); // 显示导航栏按钮
        coursePanel.SetActive(true); // 显示课程区
    }

    public void LoginManager()
    {
        loginPanel.SetActive(false); // 隐藏登录界面
        managePanel.SetActive(true);
    }

    // 设置导航栏按钮的显示状态
    private void SetNavigationButtonsVisible(bool isVisible)
    {
        foreach (var button in navigationButtons)
        {
            button.SetActive(isVisible);
        }
    }
}