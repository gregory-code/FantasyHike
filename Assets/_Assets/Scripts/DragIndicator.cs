using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragIndicator : MonoBehaviour
{
    private Inventory owner;
    private Canvas canvas;
    private RectTransform myTransform;

    public void Init(Inventory owner, Canvas canvas)
    {
        this.owner = owner;
        this.canvas = canvas;
        myTransform = GetComponent<RectTransform>();
        EndDrag();
    }

    public void Drag(PointerEventData eventData)
    {
        myTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void BeginDrag(Vector2 itemSize)
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(owner.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        myTransform.anchoredPosition = mousePos;
        myTransform.anchoredPosition += new Vector2(-45 * (itemSize.x) + (itemSize.x * 20f) - 40, 45 * (itemSize.y) - (itemSize.y * 20f) + 40);
    }

    public void EndDrag()
    {
        transform.localPosition = new Vector2(0, 1000);
    }
}
