using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] Inventory inventory;

    [SerializeField] RectTransform itemRectTransform;
    [SerializeField] RectTransform iconRectTransform;
    [SerializeField] Image iconImage;
    
    private List<slot> slots = new List<slot>();
    private Vector2 previousPos;
    private bool imDragging;

    public Vector2 itemSize;

    [Header("Public, do not touch")]

    public Vector2 gridPos;

    public slot overlappingSlot;
    
    public bool wasFlipped;
    public bool inInventory;

    public delegate void OnDropItem(item item);
    public event OnDropItem onDropItem;

    public delegate void OnItemAdjusted(item item, bool added);
    public event OnItemAdjusted onItemAdjusted;

    private void Start()
    {
        itemRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y); // this needs to be done only once, on init
        iconRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y);

        inventory.RegistarItem(this);
    }

    private void Update()
    {
        if (!imDragging)
            return;

        if (Input.touchCount >= 2 || Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
    }

    public void SetPos(Vector2 pos)
    {
        itemRectTransform.anchoredPosition = pos;
        itemRectTransform.anchoredPosition += new Vector2(45 * (itemSize.x - 1), -45 * (itemSize.y - 1));
    }

    public void ResetPos()
    {
        if(wasFlipped == true)
        {
            RotateItem();
        }

        if(inInventory)
        {
            List<slot> possibleSlots = inventory.GetPossibleSlots(this, gridPos);

            foreach (slot slot in possibleSlots)
            {
                slot.myItem = this;
            }
        }

        transform.localPosition = previousPos;
    }

    public void RotateItem()
    {
        transform.Rotate(new Vector3(0, 0, 90));
        float oldX = itemSize.x;
        itemSize.x = itemSize.y;
        itemSize.y = oldX;

        wasFlipped = true;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        SetPos(mousePos);

        if(overlappingSlot != null)
        {
            overlappingSlot.RotateItem(this);
        }
    }

    private void ItemInteract(bool state)
    {
        foreach (slot slot in slots)
        {
            slot.ItemDragState(this, state);
        }

        foreach (item item in FindObjectsOfType<item>())
        {
            item.GetComponent<Image>().raycastTarget = !state;
        }

        iconImage.color = (state) ? new Color(1,1,1, 0.1f) : new Color(1,1,1,1);
        imDragging = state;
        inventory.itemDragging = state;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        itemRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousPos = transform.localPosition;
        wasFlipped = false;

        //inventory.RemoveItem(this, gridPos);

        if (inInventory)
        {
            List<slot> possibleSlots = inventory.GetPossibleSlots(this, gridPos);

            foreach (slot slot in possibleSlots)
            {
                if (slot.myItem == this)
                {
                    slot.myItem = null;
                }
            }
        }

        // --- //
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        SetPos(mousePos);
        // --- //

        slots = inventory.slots;
        ItemInteract(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inInventory = false;
        onDropItem?.Invoke(this);
        onItemAdjusted?.Invoke(this, inInventory);
        ItemInteract(false);
    }
}
