using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
public class SQLInjectionSimulator : MonoBehaviour
{
    public InputField inputField;
    public Button queryButton;
    public Text resultText;

    // 模拟的数据库数据
    private string[,] database = new string[5, 2]
    {
        { "员工甲", "D" },
        { "员工乙", "A" },
        { "员工丙", "B" },
        { "员工丁", "C" },
        { "员工戊", "B" }
    };

    // 员工甲的固定秘钥
    private string employeeAKey = "10q174bw";

    void Start()
    {
        queryButton.onClick.AddListener(OnQueryButtonClicked);
    }

    void OnQueryButtonClicked()
    {
        string input = inputField.text;

        // 模拟SQL查询
        string query = $"select user,grade from grades where code='{input}'";

        // 检测是否是SQL注入攻击
        if (IsSQLInjection(input))
        {
            // 如果是SQL注入攻击，显示所有员工的信息
            StartCoroutine(SendStepTime("模拟sql注入攻击", "sql injection"));
            DisplayAllEmployees();
        }
        else if (input == employeeAKey)
        {
            // 如果是员工甲的秘钥，显示员工甲的信息
            DisplayEmployee("员工甲");
        }
        else
        {
            // 否则显示错误信息
            resultText.text = "秘钥输入错误";
        }
    }

    bool IsSQLInjection(string input)
    {
        // 检测输入是否包含SQL注入的特征
        // 规则1: 检测 ' OR '1'='1 或 ' OR '2'='2 等
        bool rule1 = Regex.IsMatch(input, @"'\s*or\s*'1'='1") || Regex.IsMatch(input, @"'\s*or\s*'2'='2");

        // 规则2: 检测 ' OR 'a'='a 或 ' OR 'b'='b 等
        bool rule2 = Regex.IsMatch(input, @"'\s*or\s*'[a-zA-Z]'='[a-zA-Z]");

        // 规则3: 检测 '; DROP TABLE grades; -- 等
        bool rule3 = Regex.IsMatch(input, @"';\s*DROP\s*TABLE\s*grades\s*;");

        // 规则4: 检测 ' UNION SELECT user, grade FROM grades --
        bool rule4 = Regex.IsMatch(input, @"'\s*UNION\s*SELECT\s*user,\s*grade\s*FROM\s*grades\s*--");

        // 规则5: 检测 ' OR '1 或 ' OR 'a
        bool rule5 = Regex.IsMatch(input, @"'\s*or\s*'[0-9a-zA-Z]");

        // 规则6: 检测 ' OR 1=1 -- 或 ' OR 2=2 --
        bool rule6 = Regex.IsMatch(input, @"'\s*or\s*\d+\s*=\s*\d+\s*--");

        // 规则7: 检测 ' OR '1'='1' --
        bool rule7 = Regex.IsMatch(input, @"'\s*or\s*'1'='1'\s*--");

        // 如果满足任意一条规则，则认为是SQL注入攻击
        return rule1 || rule2 || rule3 || rule4 || rule5 || rule6 || rule7;
    }

    void DisplayAllEmployees()
    {
        string result = "";
        for (int i = 0; i < database.GetLength(0); i++)
        {
            result += $"{database[i, 0]} {database[i, 1]}\n";
        }
        resultText.text = result;
    }

    void DisplayEmployee(string employeeName)
    {
        // 查找对应员工的信息
        for (int i = 0; i < database.GetLength(0); i++)
        {
            if (database[i, 0] == employeeName)
            {
                resultText.text = $"{database[i, 0]} {database[i, 1]}";
                return;
            }
        }
        resultText.text = "员工信息未找到";
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