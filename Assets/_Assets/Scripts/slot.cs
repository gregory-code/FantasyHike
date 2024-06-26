using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Inventory owner;
    private bool isHovering;
    private Vector2 gridPos;
    [SerializeField] item myItem;

    public void Init(Inventory owner, int x, int y)
    {
        this.owner = owner;
        gridPos = new Vector2(x, y);
    }

    public item GetItem()
    {
        return myItem;
    }

    public Vector2 GetGridPos()
    {
        return gridPos;
    }

    public void ItemDragState(item item, bool dragging)
    {
        if(dragging)
        {
            item.onDropItem += DropItem;
        }
        else
        {
            item.onDropItem -= DropItem;
        }
    }

    public void DropItem(item item)
    {
        if (!isHovering)
            return;

        if(owner.isSlotsValid(item, this))
        {
            item.transform.localPosition = this.transform.localPosition;
        }
        else
        {
            item.ResetPos();
        }

    }

    public void DraggingItem(Vector2 itemPos)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
