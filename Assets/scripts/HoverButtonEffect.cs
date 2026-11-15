using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonBackground; // 按钮的背景 Image 组件
    public float hoverAlpha = 0.5f; // 鼠标悬停时的透明度
    private float normalAlpha; // 正常状态下的透明度

    private void Start()
    {
        // 确保按钮背景已赋值
        if (buttonBackground == null)
        {
            buttonBackground = GetComponent<Image>();
        }

        // 记录正常状态下的透明度
        if (buttonBackground != null)
        {
            normalAlpha = buttonBackground.color.a;
        }
    }

    // 鼠标悬停时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonBackground != null)
        {
            // 改变背景透明度
            Color newColor = buttonBackground.color;
            newColor.a = hoverAlpha;
            buttonBackground.color = newColor;
        }
    }

    // 鼠标离开时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonBackground != null)
        {
            // 恢复背景透明度
            Color newColor = buttonBackground.color;
            newColor.a = normalAlpha;
            buttonBackground.color = newColor;
        }
    }
}