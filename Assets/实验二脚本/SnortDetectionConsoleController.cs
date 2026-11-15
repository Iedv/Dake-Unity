using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;
using System.Collections;
public class SnortDetectionConsoleController : MonoBehaviour
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

    public Button nmapAttackButton;           // Nmap攻击按钮
    public Button openConsoleButton;          // 打开控制台按钮
    public Button openRulePanelButton;        // 打开规则输入框按钮

    private string currentPath = "root@snort:~# "; // 当前路径

    // 预定义的规则模板（隐藏 protocol、msg 和 flags）
    private string[] ruleTemplates = new string[]
    {
        "alert [protocol] $EXTERNAL_NET any -> $HOME_NET any (msg:\"[msg]\"; flags:[flags]; detection_filter:track by_src, count 5, seconds 10; sid:1000002; rev:1;)",
        "alert [protocol] $EXTERNAL_NET any -> $HOME_NET any (msg:\"[msg]\"; flags:[flags]; detection_filter:track by_src, count 5, seconds 10; sid:1000003; rev:1;)",
        "alert [protocol] $EXTERNAL_NET any -> $HOME_NET any (msg:\"[msg]\"; flags:[flags]; detection_filter:track by_src, count 5, seconds 10; sid:1000004; rev:1;)",
        "alert [protocol] $EXTERNAL_NET any -> $HOME_NET any (msg:\"[msg]\"; flags:[flags]; detection_filter:track by_src, count 5, seconds 10; sid:1000005; rev:1;)",
        "alert [protocol] $EXTERNAL_NET any -> $HOME_NET any (msg:\"[msg]\"; detection_filter:track by_src, count 10, seconds 30; sid:1000006; rev:1;)",
        "alert [protocol] $EXTERNAL_NET any -> $HOME_NET any (msg:\"[msg]\"; content:\"|00 00 00 00|\"; depth:4; offset:8; detection_filter:track by_src, count 3, seconds 10; sid:1000007; rev:1;)"
    };

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

        // 监听Nmap攻击按钮点击事件
        nmapAttackButton.onClick.AddListener(OnNmapAttackButtonClick);

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

    void OnNmapAttackButtonClick()
    {
        // 获取学生填写的规则
        string studentRules = ruleInputField.text;

        // 按行分割规则
        string[] rules = studentRules.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // 遍历每条规则并校验
        foreach (string rule in rules)
        {
            // 检查 protocol 和 flags 是否正确
            if (IsValidRule(rule))
            {
                StartCoroutine(SendStepTime("检测端口扫描攻击", "tcp S"));
                // 提取学生填写的 msg 内容
                int msgStart = rule.IndexOf("msg:\"") + 5;
                int msgEnd = rule.IndexOf("\"", msgStart);
                string msg = rule.Substring(msgStart, msgEnd - msgStart);

                // 获取当前时间
                string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // 生成报警信息
                string alertMessage = $"[**] [1:1000001:1] {msg} [**]\n" +
                                     $"[Classification: Attempted Information Leak] [Priority: 1]\n" +
                                     $"{currentTime} [Snort Alert]\n" +
                                     $"Protocol: TCP | Flags: S\n" +
                                     $"Source IP: 192.168.1.100 | Destination IP: 192.168.1.2\n";

                // 显示在控制台中
                consoleOutputText.text += alertMessage + "\n";
                UpdateConsoleContentSize();
                ScrollConsoleToBottom();
            }
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
        // 将规则模板加载到规则输入框中，并确保换行
        ruleInputField.text = string.Join(Environment.NewLine, ruleTemplates);

        // 启用多行输入和回车换行
        ruleInputField.lineType = InputField.LineType.MultiLineNewline;
    }

    bool IsValidRule(string rule)
    {
        // 检查 protocol 是否为 tcp 且 flags 是否为 S
        return rule.Contains("alert tcp") && rule.Contains("flags:S");
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