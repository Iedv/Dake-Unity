using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class SendToJavaBackend : MonoBehaviour
{
    public TMP_InputField inputField; // 输入字段
    public Button sendButton;     // 发送按钮

    private string url = "http://localhost:8081/api/endpoint"; // 替换为你的 Java 后端 URL

    void Start()
    {
        // 绑定按钮点击事件
        sendButton.onClick.AddListener(SendData);
    }

    void SendData()
    {
        string inputText = inputField.text; // 获取输入内容
        StartCoroutine(PostRequest(inputText)); // 发送 HTTP 请求
    }

    IEnumerator PostRequest(string data)
    {
        // 创建 JSON 数据
        string jsonData = "{\"input\":\"" + data + "\"}";

        // 创建 UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 发送请求并等待响应
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
        }
    }
}