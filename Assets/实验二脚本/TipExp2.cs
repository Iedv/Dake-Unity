using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class TipExp2 : MonoBehaviour
{
    public Button hintButton; // 提示按钮
    public GameObject hintPanel; // 提示Panel

    private void Start()
    {
        // 绑定提示按钮的点击事件
        hintButton.onClick.AddListener(OnHintButtonClicked);

        // 初始化时隐藏提示Panel
        hintPanel.SetActive(false);
    }

    private void OnHintButtonClicked()
    {
        // 显示提示Panel
        hintPanel.SetActive(true);

        // 发送POST请求记录提示点击时间
        StartCoroutine(SendHintClickTime());
    }

    private IEnumerator SendHintClickTime()
    {
        // 构造JSON数据
        string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\",\"experimentType\":\"timing\",\"experimentId\":\"检测端口扫描攻击\",\"timeType\":\"hintClickTime\"}";
        Debug.Log("Sending JSON: " + jsonData);

        // 创建UnityWebRequest对象
        using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/record-experiment-time", "POST"))
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