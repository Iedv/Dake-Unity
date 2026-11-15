using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CreateLineButton : MonoBehaviour
{
    public Button createLineButton; // 连线按钮
    public GameObject linePrefab;   // 连线预制体
    public RectTransform content;   // content 物体

    private RectTransform firstObject; // 第一个选中的物体
    private bool isSelecting = false;  // 是否正在选择物体

    public Dictionary<(RectTransform, RectTransform), GameObject> lineDictionary = new Dictionary<(RectTransform, RectTransform), GameObject>();

    void Start()
    {
        createLineButton.onClick.AddListener(StartSelecting);
    }

    void Update()
    {
        if (isSelecting && Input.GetMouseButtonDown(0)) // 左键点击选择物体
        {
            RectTransform selectedObject = GetClickedObject();
            if (selectedObject != null)
            {
                if (firstObject == null)
                {
                    firstObject = selectedObject;
                    Debug.Log("First object selected: " + firstObject.name);

                    // 禁用第一个物体的拖拽功能
                    DraggableObject draggable = firstObject.GetComponent<DraggableObject>();
                    if (draggable != null)
                    {
                        draggable.SetDraggingEnabled(false);
                    }
                }
                else
                {
                    CreateLine(firstObject, selectedObject);
                    firstObject = null;
                    isSelecting = false;

                    // 重新启用所有物体的拖拽功能
                    SetAllDraggableObjectsEnabled(true);
                }
            }
        }
    }

    private void StartSelecting()
    {
        isSelecting = true;
        Debug.Log("Click two objects to create a line.");

        // 禁用所有物体的拖拽功能
        SetAllDraggableObjectsEnabled(false);
    }

    private RectTransform GetClickedObject()
    {
        // 检测是否点击了 UI 元素
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            RectTransform rectTransform = result.gameObject.GetComponent<RectTransform>();
            if (rectTransform != null && rectTransform.IsChildOf(content))
            {
                return rectTransform;
            }
        }

        // 检测是否点击了普通物体
        Vector2 mousePosition = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            RectTransform rectTransform = hit.collider.GetComponent<RectTransform>();
            if (rectTransform != null && rectTransform.IsChildOf(content))
            {
                return rectTransform;
            }
        }

        return null;
    }

    private void CreateLine(RectTransform startObject, RectTransform endObject)
    {
        if (lineDictionary.ContainsKey((startObject, endObject)) || lineDictionary.ContainsKey((endObject, startObject)))
        {
            Debug.Log("A line already exists between these objects.");
            return;
        }

        GameObject line = Instantiate(linePrefab, content);
        DynamicLine dynamicLine = line.GetComponent<DynamicLine>();
        dynamicLine.startObject = startObject;
        dynamicLine.endObject = endObject;

        lineDictionary[(startObject, endObject)] = line;
        Debug.Log("Line created between " + startObject.name + " and " + endObject.name);
    }

    private void SetAllDraggableObjectsEnabled(bool enabled)
    {
        DraggableObject[] draggableObjects = FindObjectsOfType<DraggableObject>();
        foreach (DraggableObject draggable in draggableObjects)
        {
            draggable.SetDraggingEnabled(enabled);
        }
    }
}