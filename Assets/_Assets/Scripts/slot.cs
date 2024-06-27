using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Color highlightGood;
    [SerializeField] Color highlightBad;
    
    private List<slot> highlightedSlots = new List<slot>();
    private Inventory owner;
    private bool isHovering;
    private item draggingItem;

    [Header("Public, do not touch")]

    public Vector2 gridPos;
    public bool badgroup;
    public item myItem;

    public void Init(Inventory owner, int x, int y)
    {
        this.owner = owner;
        gridPos = new Vector2(x, y);
    }
    
    public void RotateItem(item item)
    {
        ExitSlot();
        EnterSlot();
    }

    public void ItemDragState(item item, bool dragging)
    {
        if(dragging)
        {
            draggingItem = item;
            item.onDropItem += DropItem;
        }
        else
        {
            draggingItem = null;
            item.onDropItem -= DropItem;
        }
    }

    public void DropItem(item item)
    {
        if (!isHovering)
        {
            return;
        }

        TryPlaceItem(item);
    }

    public void TryPlaceItem(item item)
    {
        item.inInventory = true;

        List<slot> possibleSlots = owner.GetPossibleSlots(item, gridPos);

        if (owner.ValidSlotPlacement(possibleSlots) == false || possibleSlots[0].badgroup)
        {
            item.ResetPos();
            return;
        }

        // item gets added

        foreach (slot slot in possibleSlots)
        {
            slot.myItem = item;
        }
        item.gridPos = gridPos;
        item.SetPos(this.transform.localPosition);
    }

    public void Highlight(bool goodPlacement)
    {
        GetComponent<Image>().color = (goodPlacement) ? highlightGood : highlightBad  ;
        if(goodPlacement == false)
        {
            transform.SetAsLastSibling();
        }
    }

    public void Deselect()
    {
        GetComponent<Image>().color = Color.white;
        badgroup = false;
        transform.SetAsFirstSibling();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EnterSlot();
    }

    private void EnterSlot()
    {
        isHovering = true;
        if (owner.itemDragging)
        {
            draggingItem.overlappingSlot = this;

            List<slot> checkSlots = owner.GetPossibleSlots(draggingItem, gridPos);

            foreach (slot slot in checkSlots)
            {
                highlightedSlots.Add(slot);

                if (checkSlots[0].badgroup)
                {
                    slot.Highlight(false);
                    continue;
                }

                slot.Highlight(!slot.myItem);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitSlot();
    }

    private void ExitSlot()
    {
        isHovering = false;
        foreach (slot slot in highlightedSlots)
        {
            slot.Deselect();
        }
        highlightedSlots.Clear();

        if(owner.itemDragging)
        {
            draggingItem.overlappingSlot = null;
        }
    }
}
