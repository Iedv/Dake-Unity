using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI; // 引入 UI 命名空间
using System.Collections;

public class LoginHandler : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public UIManager uiManager;
    public Button loginButton; // 添加按钮变量

    void Start()
    {
        // 绑定按钮点击事件
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }
        else
        {
            Debug.LogError("Login Button 未绑定！");
        }
    }

    public void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(LoginRequest(username, password));
    }

    private IEnumerator LoginRequest(string username, string password)
    {
        // 手动构造 JSON 数据
        string jsonData = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/login", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                uiManager.ShowPrompt("网络错误，请重试");
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);

                if (response.status == "success")
                {
                    GlobalState.CurrentUsername = username;
                    if(username == "admin")
                    {
                        uiManager.LoginManager();
                    }
                    else
                    {
                        uiManager.OnLoginSuccess();
                        StartCoroutine(HeartbeatCoroutine());
                    }
                    
                }
                else
                {
                    uiManager.ShowPrompt(response.message);
                }
            }
        }
    }

    private IEnumerator HeartbeatCoroutine()
    {
        Debug.Log("心跳包协程启动");
        float lastHeartbeatTime = Time.realtimeSinceStartup;
        while (true)
        {
            if (Time.realtimeSinceStartup - lastHeartbeatTime >= 60)
            {
                lastHeartbeatTime = Time.realtimeSinceStartup;
                Debug.Log("发送心跳包");
                string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\"}";

                using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/heartbeat", "POST"))
                {
                    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("心跳包发送失败: " + request.error);
                    }
                    else
                    {
                        Debug.Log("心跳包发送成功: " + request.downloadHandler.text);
                    }
                }
            }
            yield return null; // 每帧检查一次
        }
    }

    [System.Serializable]
    private class Response
    {
        public string status;
        public string message;
    }
}