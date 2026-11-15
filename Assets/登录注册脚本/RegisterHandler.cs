using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI; // 引入 UI 命名空间
using System.Collections;

public class RegisterHandler : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public UIManager uiManager;
    public Button registerButton; // 添加按钮变量

    void Start()
    {
        // 绑定按钮点击事件
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(OnRegisterButtonClicked);
        }
        else
        {
            Debug.LogError("Register Button 未绑定！");
        }
    }

    public void OnRegisterButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(RegisterRequest(username, password));
    }

    private IEnumerator RegisterRequest(string username, string password)
    {
        // 手动构造 JSON 数据
        string jsonData = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}";
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/register", "POST"))
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
                uiManager.ShowPrompt(response.message);
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