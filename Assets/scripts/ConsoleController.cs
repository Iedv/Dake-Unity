using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Networking;
using System.Collections;

public class ConsoleController : MonoBehaviour
{
    // ��UI���
    public InputField inputField;       // �����
    public Text pathText;               // ·����ʾ��
    public Text outputText;             // ����ı�
    public ScrollRect scrollRect;       // ������ͼ
    public RectTransform content;       // Content �� RectTransform
    public Button minimizeButton;       // ��С��ť
    public Button closeButton;          // �رհ�ť
    public GameObject commandPanel;     // ����̨����GameObject

    private string currentPath = "root@kali:~# "; // ��ǰ·��

    void Start()
    {
        // ��ʼ��·����ʾ��
        pathText.text = currentPath;

        // ���������Ļس��¼�
        inputField.onEndEdit.AddListener(OnInputEndEdit);

        // ������С��ť����¼�
        minimizeButton.onClick.AddListener(OnMinimizeButtonClick);

        // �����رհ�ť����¼�
        closeButton.onClick.AddListener(OnCloseButtonClick);

        // �������۽�
        inputField.ActivateInputField();
    }

    void OnInputEndEdit(string input)
    {
        // ����Ƿ��»س���
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // ��ʾ����������·����ʾ����
            outputText.text += currentPath + input + "\n";

            // ִ�������ʾ���
            string result = ExecuteCommand(input);
            outputText.text += result + "\n";

            // ��������
            inputField.text = "";

            // ����������¾۽�
            inputField.ActivateInputField();

            // ���� Content ��С
            UpdateContentSize();

            // �������ײ�
            ScrollToBottom();
        }
    }

    string ExecuteCommand(string command)
    {
        // ȥ������Ŀո�
        command = Regex.Replace(command, @"\s+", " ").Trim();

        // �ָ�����Ͳ���
        string[] parts = command.Split(' ');
        string cmd = parts[0];
        string[] args = parts.Length > 1 ? parts[1..] : new string[0];

        // ��ȡ��ǰʱ��
        string currentTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        // ���������ִ��
        switch (cmd)
        {
            case "ifconfig":
            case "ip":
                if (args.Length > 0 && (args[0] == "addr" || args[0] == "addr show"))
                {
                    StartCoroutine(SendStepTime("ģ��˿�ɨ�蹥��","ip addr"));
                    return "1: lo: <LOOPBACK,UP,LOWER_UP> mtu 65536 qdisc noqueue state UNKNOWN group default qlen 1000\n" +
                           "    link/loopback 00:00:00:00:00:00 brd 00:00:00:00:00:00\n" +
                           "    inet 127.0.0.1/8 scope host lo\n" +
                           "       valid_lft forever preferred_lft forever\n" +
                           "2: eth0: <BROADCAST,MULTICAST,UP,LOWER_UP> mtu 1500 qdisc fq_codel state UP group default qlen 1000\n" +
                           "    link/ether 00:0c:29:ab:cd:ef brd ff:ff:ff:ff:ff:ff\n" +
                           "    inet 192.168.1.2/24 brd 192.168.1.255 scope global dynamic eth0\n" +
                           "       valid_lft 86388sec preferred_lft 86388sec";
                }
                return "Usage: ip addr or ip addr show";

            case "nmap":
                if (args.Length == 0)
                {
                    return "Usage: nmap -sn <IP range> or nmap <IP>";
                }
                else if (args[0] == "-sn")
                {
                    if (args.Length < 2)
                    {
                        return "Error: IP range not specified.";
                    }
                    string ipRange = args[1];
                    if (IsValidIPRange(ipRange))
                    {
                        StartCoroutine(SendStepTime("ģ��˿�ɨ�蹥��", "namp -sn <ip range>"));
                        return $"Starting Nmap 7.80 ( https://nmap.org ) at {currentTime} UTC\n" +
                               "Nmap scan report for 192.168.1.2\n" +
                               "Host is up (0.00010s latency).\n" +
                               "Nmap scan report for 192.168.1.3\n" +
                               "Host is up (0.00015s latency).\n" +
                               "Nmap done: 256 IP addresses (2 hosts up) scanned in 2.00 seconds";
                    }
                    else
                    {
                        return "Error: Invalid IP range.";
                    }
                }
                else
                {
                    string ip = args[0];
                    if (IsValidIPRange(ip))
                    {
                        return $"Starting Nmap 7.80 ( https://nmap.org ) at {currentTime} UTC\n" +
                               "Warning: Scanning the entire subnet can be slow. Use 'nmap -sn' for a faster host discovery.\n" +
                               "Nmap scan report for 192.168.1.0/24\n" +
                               "Host is up (0.00010s latency).\n" +
                               "Nmap done: 256 IP addresses (2 hosts up) scanned in 10.00 seconds";
                    }
                    else if (IsValidIP(ip))
                    {
                        if (ip == "192.168.1.3")
                        {
                            StartCoroutine(SendStepTime("ģ��˿�ɨ�蹥��", "nmap ip"));
                            return $"Starting Nmap 7.80 ( https://nmap.org ) at {currentTime} UTC\n" +
                                   "Nmap scan report for 192.168.1.3\n" +
                                   "Host is up (0.00010s latency).\n" +
                                   "Not shown: 995 closed ports\n" +
                                   "PORT    STATE SERVICE\n" +
                                   "22/tcp  open  ssh\n" +
                                   "80/tcp  open  http\n" +
                                   "443/tcp open  https\n" +
                                   "Nmap done: 1 IP address (1 host up) scanned in 0.10 seconds";
                        }
                        else
                        {
                            return $"Starting Nmap 7.80 ( https://nmap.org ) at {currentTime} UTC\n" +
                                   "Nmap scan report for " + ip + "\n" +
                                   "Note: Host seems down. If it is really up, but blocking our ping probes, try -Pn\n" +
                                   "Nmap done: 1 IP address (0 hosts up) scanned in 0.10 seconds";
                        }
                    }
                    else
                    {
                        return "Error: Invalid IP address or range.";
                    }
                }

            case "help":
                return "Available commands: ifconfig, ip addr, nmap, help";

            default:
                return "Unknown command: " + cmd;
        }
    }

    bool IsValidIP(string ip)
    {
        // �򵥵�IP��ַ��֤
        return Regex.IsMatch(ip, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
    }

    bool IsValidIPRange(string ipRange)
    {
        // �򵥵�IP��Χ��֤
        return Regex.IsMatch(ipRange, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}/\d{1,2}$");
    }

    void UpdateContentSize()
    {
        // ��ȡ����ı��� Preferred Height
        float preferredHeight = outputText.preferredHeight;

        // ���� Content �ĸ߶�
        content.sizeDelta = new Vector2(content.sizeDelta.x, preferredHeight);

        // ǿ�Ƹ��� Canvas
        Canvas.ForceUpdateCanvases();
    }

    void ScrollToBottom()
    {
        // ���������������ײ�
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void OnMinimizeButtonClick()
    {
        // ���ؿ���̨���
        commandPanel.SetActive(false);
    }

    void OnCloseButtonClick()
    {
        // �������������
        outputText.text = "";
        inputField.text = "";

        // ���ؿ���̨���
        commandPanel.SetActive(false);
    }

    private IEnumerator SendStepTime(string experimentId, string stepName)
    {
        // ����JSON����
        string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\",\"experimentId\":\"" + experimentId + "\",\"stepName\":\"" + stepName + "\"}";
        Debug.Log("Sending JSON: " + jsonData);

        // ����UnityWebRequest����
        using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/record-step-time", "POST"))
        // using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/record-step-time", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // �������󲢵ȴ���Ӧ
            yield return request.SendWebRequest();

            // ������Ӧ
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