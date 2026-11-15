using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class SendDataToServer : MonoBehaviour
{
    // 后端服务器URL
    private string serverUrl = "http://localhost:8081/save-data";

    // 引用TMP输入框
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    // 引用登录按钮
    public Button loginButton;

    void Start()
    {
        // 绑定登录按钮的点击事件
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    // 登录按钮点击事件
    private void OnLoginButtonClicked()
    {
        // 启动协程发送数据
        StartCoroutine(SendData());
    }

    IEnumerator SendData()
    {
        // 获取输入框中的账号和密码
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // 创建一个JSON对象
        var data = new
        {
            username = username,
            password = password
        };

        // 将对象转换为JSON字符串
        string jsonData = JsonUtility.ToJson(data);

        // 创建UnityWebRequest对象
        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 发送请求
        yield return request.SendWebRequest();

        // 处理响应
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}