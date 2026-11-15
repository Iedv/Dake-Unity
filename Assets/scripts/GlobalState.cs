using UnityEngine;

public static class GlobalState
{
    public static int activeButtonIndex = -1; // 保存当前被按下的按钮索引
    public static string CurrentUsername { get; set; } // 保存当前登录的用户名

    public static string answerOfLab1 = "443";

    public static string answerOfLab2 = "192.168.1.100";

    public static string answerOfLab3 = "B";

    public static string answerOfLab4 = "192.168.1.10";
}