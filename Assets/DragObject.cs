using UnityEngine;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;

    void OnMouseDown()
    {
        // 获取物体在屏幕空间的Z坐标
        zCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        // 计算鼠标点击位置与物体位置的偏移
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        // 更新物体位置
        transform.position = GetMouseWorldPos() + offset;
    }

    private Vector3 GetMouseWorldPos()
    {
        // 获取鼠标在屏幕空间的位置
        Vector3 mousePoint = Input.mousePosition;

        // 设置Z坐标为物体的Z坐标
        mousePoint.z = zCoord;

        // 将屏幕空间坐标转换为世界空间坐标
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}