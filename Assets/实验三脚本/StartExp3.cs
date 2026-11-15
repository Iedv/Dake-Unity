using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class StartExp3 : MonoBehaviour
{
    public GameObject scenePanel; // 场景Panel
    public Button startExperimentButton; // 开始实验按钮
    public GameObject[] gameObjectsToShow; // 需要显示的游戏物体数组

    private void Start()
    {
        // 初始化时隐藏所有需要显示的游戏物体
        foreach (var obj in gameObjectsToShow)
        {
            obj.SetActive(false);
        }

        // 绑定按钮点击事件
        startExperimentButton.onClick.AddListener(OnStartExperimentButtonClicked);
    }

    private void OnStartExperimentButtonClicked()
    {
        // 隐藏场景Panel
        scenePanel.SetActive(false);

        // 显示游戏物体数组中的游戏物体
        foreach (var obj in gameObjectsToShow)
        {
            obj.SetActive(true);
        }

        // 发送POST请求记录实验开始时间
        StartCoroutine(SendExperimentStartTime());
    }

    private IEnumerator SendExperimentStartTime()
    {
        // 构造JSON数据
        string jsonData = "{\"username\":\"" + GlobalState.CurrentUsername + "\",\"experimentType\":\"timing\",\"experimentId\":\"模拟sql注入攻击\",\"timeType\":\"startTime\"}";
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