using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class FinishExp4 : MonoBehaviour
{
    public InputField answerInputField; // 文本输入框，用于提交答案
    public Button submitButton; // 提交按钮
    public GameObject feedbackPanel; // 提交反馈Panel
    public Text feedbackText; // 反馈Panel上的文本框

    private void Start()
    {
        // 绑定提交按钮的点击事件
        submitButton.onClick.AddListener(OnSubmitButtonClicked);

        // 初始化时隐藏反馈Panel
        feedbackPanel.SetActive(false);
    }

    private void OnSubmitButtonClicked()
    {
        // 获取用户输入的答案
        string userAnswer = answerInputField.text;

        // 校验答案是否正确
        if (userAnswer == GlobalState.answerOfLab4)
        {
            // 答案正确
            feedbackText.text = "回答正确！";
            feedbackPanel.SetActive(true);

            // 发送POST请求记录实验结束时间
            StartCoroutine(SendExperimentEndTime());
        }
        else
        {
            // 答案错误
            feedbackText.text = "回答错误，再想想吧";
            feedbackPanel.SetActive(true);
            StartCoroutine(SendError());
        }
    }

    private IEnumerator SendExperimentEndTime()
    {
        // 构造JSON数据
        string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\",\"experimentType\":\"timing\",\"experimentId\":\"检测sql注入攻击\",\"timeType\":\"endTime\"}";
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

    private IEnumerator SendError()
    {
        // 构造JSON数据
        string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\",\"experimentId\":\"检测sql注入攻击\"}";
        Debug.Log("Sending JSON: " + jsonData);

        // 创建UnityWebRequest对象
        using (UnityWebRequest request = new UnityWebRequest("http://localhost:8081/api/addErrorCount", "POST"))
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