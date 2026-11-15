using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DynamicLine : MonoBehaviour
{
    public RectTransform startObject; // 连接的起始物体
    public RectTransform endObject;   // 连接的结束物体

    private RectTransform rectTransform;
    private Image lineImage;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        lineImage = GetComponent<Image>();
        lineImage.color = Color.black;
    }

    void Update()
    {
        if (startObject == null || endObject == null) return;

        // 计算线的中心点位置
        Vector2 startPosition = startObject.anchoredPosition;
        Vector2 endPosition = endObject.anchoredPosition;
        Vector2 lineCenter = (startPosition + endPosition) * 0.5f;
        rectTransform.anchoredPosition = lineCenter;

        // 计算线的长度和角度
        Vector2 direction = endPosition - startPosition;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 设置线的长度和旋转
        rectTransform.sizeDelta = new Vector2(distance, 2f); // 2f 是线的宽度
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}