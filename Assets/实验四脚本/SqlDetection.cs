using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

public class SqlDetection : MonoBehaviour
{
    // 绑定UI组件
    public InputField consoleInputField;       // 控制台输入框
    public Text consolePathText;               // 控制台路径提示符
    public Text consoleOutputText;             // 控制台输出文本
    public ScrollRect consoleScrollRect;       // 控制台滚动视图
    public RectTransform consoleContent;       // 控制台 Content 的 RectTransform
    public Button consoleMinimizeButton;       // 控制台缩小按钮
    public Button consoleCloseButton;          // 控制台关闭按钮
    public GameObject consolePanel;            // 控制台面板的GameObject

    public InputField ruleInputField;          // 规则输入框
    public GameObject rulePanel;              // 规则输入框面板的GameObject

    public InputField attackInputField;       // 攻击语句输入框
    public Button startAttackButton;          // 开始攻击按钮
    public Button openConsoleButton;          // 打开控制台按钮
    public Button openRulePanelButton;        // 打开规则输入框按钮

    private string currentPath = "root@snort:~# "; // 当前路径

    // 预定义的规则模板（隐藏 msg 和 content）
    private string ruleTemplate = "alert tcp $EXTERNAL_NET any -> $HTTP_SERVERS $HTTP_PORTS (msg:\"[msg]\"; flow:to_server,established; content:\"[content]\"; nocase; sid:1000001; rev:1;)";

    void Start()
    {
        // 初始化控制台路径提示符
        consolePathText.text = currentPath;

        // 监听控制台输入框的回车事件
        consoleInputField.onEndEdit.AddListener(OnConsoleInputEndEdit);

        // 监听控制台缩小按钮点击事件
        consoleMinimizeButton.onClick.AddListener(OnConsoleMinimizeButtonClick);

        // 监听控制台关闭按钮点击事件
        consoleCloseButton.onClick.AddListener(OnConsoleCloseButtonClick);

        // 监听开始攻击按钮点击事件
        startAttackButton.onClick.AddListener(OnStartAttackButtonClick);

        // 监听打开控制台按钮点击事件
        openConsoleButton.onClick.AddListener(OnOpenConsoleButtonClick);

        // 监听打开规则输入框按钮点击事件
        openRulePanelButton.onClick.AddListener(OnOpenRulePanelButtonClick);

        // 初始化规则输入框内容
        InitializeRuleInputField();

        // 默认隐藏控制台和规则输入框面板
        consolePanel.SetActive(false);
        rulePanel.SetActive(false);
    }

    void OnConsoleInputEndEdit(string input)
    {
        // 检测是否按下回车键
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 显示输入的命令（带路径提示符）
            consoleOutputText.text += currentPath + input + "\n";

            // 执行命令并显示结果
            string result = ExecuteCommand(input);
            consoleOutputText.text += result + "\n";

            // 清空输入框
            consoleInputField.text = "";

            // 让输入框重新聚焦
            consoleInputField.ActivateInputField();

            // 更新 Content 大小
            UpdateConsoleContentSize();

            // 滚动到底部
            ScrollConsoleToBottom();
        }
    }

    string ExecuteCommand(string command)
    {
        // 不管输入什么命令，都提示“不可用”
        return "Command not available in this experiment.";
    }

    void UpdateConsoleContentSize()
    {
        // 获取输出文本的 Preferred Height
        float preferredHeight = consoleOutputText.preferredHeight;

        // 设置 Content 的高度
        consoleContent.sizeDelta = new Vector2(consoleContent.sizeDelta.x, preferredHeight);

        // 强制更新 Canvas
        Canvas.ForceUpdateCanvases();
    }

    void ScrollConsoleToBottom()
    {
        // 将滚动条滚动到底部
        consoleScrollRect.verticalNormalizedPosition = 0f;
    }

    void OnConsoleMinimizeButtonClick()
    {
        // 隐藏控制台面板
        consolePanel.SetActive(false);
    }

    void OnConsoleCloseButtonClick()
    {
        // 清空命令行内容
        consoleOutputText.text = "";
        consoleInputField.text = "";

        // 隐藏控制台面板
        consolePanel.SetActive(false);
    }

    void OnStartAttackButtonClick()
    {
        // 获取学生填写的规则
        string studentRule = ruleInputField.text;

        // 获取学生构造的攻击语句
        string attackStatement = attackInputField.text;

        // 检查规则是否有效
        if (IsValidRule(studentRule, attackStatement))
        {
            StartCoroutine(SendStepTime("检测sql注入攻击", "sql detection"));
            // 提取学生填写的 msg 内容
            int msgStart = studentRule.IndexOf("msg:\"") + 5;
            int msgEnd = studentRule.IndexOf("\"", msgStart);
            string msg = studentRule.Substring(msgStart, msgEnd - msgStart);

            // 获取当前时间
            string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 生成报警信息
            string alertMessage = $"[**] [1:1000001:1] {msg} [**]\n" +
                                 $"[Classification: SQL Injection Attempt] [Priority: 1]\n" +
                                 $"{currentTime} [Snort Alert]\n" +
                                 $"Protocol: TCP | Attack Statement: {attackStatement}\n" +
                                 $"Source IP: 192.168.1.10 | Destination IP: 192.168.1.2\n";

            // 显示在控制台中
            consoleOutputText.text += alertMessage + "\n";
            UpdateConsoleContentSize();
            ScrollConsoleToBottom();
        }
    }

    void OnOpenConsoleButtonClick()
    {
        // 如果规则面板处于激活状态，则先隐藏规则面板
        if (rulePanel.activeSelf)
        {
            rulePanel.SetActive(false);
        }

        // 显示控制台面板
        consolePanel.SetActive(true);
    }

    void OnOpenRulePanelButtonClick()
    {
        // 如果控制台面板处于激活状态，则先隐藏控制台面板
        if (consolePanel.activeSelf)
        {
            consolePanel.SetActive(false);
        }

        // 显示规则输入框面板
        rulePanel.SetActive(true);
    }

    void InitializeRuleInputField()
    {
        // 将规则模板加载到规则输入框中
        ruleInputField.text = ruleTemplate;

        // 启用多行输入和回车换行
        ruleInputField.lineType = InputField.LineType.MultiLineNewline;
    }

    bool IsValidRule(string rule, string attackStatement)
    {
        // 检查规则是否包含 msg 和至少一个 content 字段
        if (!rule.Contains("msg:\"") || !rule.Contains("content:\""))
        {
            return false;
        }

        // 提取所有的 content 字段
        List<string> contentKeywords = new List<string>();
        int contentStartIndex = 0;
        while (true)
        {
            contentStartIndex = rule.IndexOf("content:\"", contentStartIndex);
            if (contentStartIndex == -1) break;

            contentStartIndex += 9; // 跳过 "content:\""
            int contentEndIndex = rule.IndexOf("\"", contentStartIndex);
            string keyword = rule.Substring(contentStartIndex, contentEndIndex - contentStartIndex);
            contentKeywords.Add(keyword);

            contentStartIndex = contentEndIndex + 1;
        }

        // 检查攻击语句是否包含任意一个 content 关键词
        foreach (string keyword in contentKeywords)
        {
            if (attackStatement.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator SendStepTime(string experimentId, string stepName)
    {
        // 构造JSON数据
        string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\",\"experimentId\":\"" + experimentId + "\",\"stepName\":\"" + stepName + "\"}";
        Debug.Log("Sending JSON: " + jsonData);

        // 创建UnityWebRequest对象
        using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/record-step-time", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 发送请求并等待响应
            yield return request.SendWebRequest();

            // 处理响应
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status + ", Message: " + response.message);
            }
        }
    }

    [System.Serializable]
    private class Response
    {
        public string status;
        public string message;
    }

}