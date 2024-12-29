using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Sprite goodSlot;
    [SerializeField] Sprite badSlot;
    [SerializeField] Sprite regularSlot;
    [SerializeField] Color highlightColor;
    [SerializeField] Color regularColor;
    
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

    public bool TryPlaceItem(item item)
    {
        item.SetInInventory(true);

        List<slot> possibleSlots = owner.GetPossibleSlots(item, gridPos);


        if (owner.ValidSlotPlacement(possibleSlots) == false || owner.GetBadGroup())
        {
            item.SetInInventory(false);
            item.ResetPos();
            return false;
        }

        // item gets added

        foreach (slot slot in possibleSlots)
        {
            slot.myItem = item;
        }
        item.SetGridPos(gridPos);
        item.SetPos(this.transform.localPosition);
        return true;
    }

    public void Highlight(bool goodPlacement)
    {
        GetComponent<Image>().sprite = (goodPlacement) ? goodSlot : badSlot  ;
        GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
        GetComponent<Image>().color = highlightColor;
        if(goodPlacement == false)
        {
            transform.SetAsLastSibling();
        }
    }

    public void Deselect()
    {
        GetComponent<Image>().sprite = regularSlot;
        GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);
        GetComponent<Image>().color = regularColor;
        badgroup = false;
        transform.SetAsFirstSibling();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //EnterSlot();
    }

    private void EnterSlot()
    {
        isHovering = true;
        if (owner.itemDragging)
        {
            List<slot> checkSlots = owner.GetPossibleSlots(draggingItem, gridPos);

            foreach (slot slot in checkSlots)
            {
                highlightedSlots.Add(slot);

                if (owner.GetBadGroup())
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
        //ExitSlot();
    }

    private void ExitSlot()
    {
        isHovering = false;
        foreach (slot slot in highlightedSlots)
        {
            slot.Deselect();
        }
        highlightedSlots.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnterSlot();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ExitSlot();
    }
}
