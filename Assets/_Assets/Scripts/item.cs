using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private Canvas canvas;
    private Inventory inventory;

    [SerializeField] RectTransform itemRectTransform;
    [SerializeField] RectTransform iconRectTransform;
    [SerializeField] Image iconImage;

    public itemEffect myEffect;
    
    private List<slot> slots = new List<slot>();
    private Vector2 previousPos;
    private bool isClicked;
    private bool wasFlipped;
    private bool wasInInventory;

    [Header("Public, do not touch")]

    public Vector2 itemSize;
    public List<Vector2> sizeBlanks = new List<Vector2>();
    private int blankNum;

    public Vector2 gridPos;
    
    public bool inInventory;

    public delegate void OnDropItem(item item);
    public event OnDropItem onDropItem;

    public delegate void OnItemAdjusted(item item, bool added);
    public event OnItemAdjusted onItemAdjusted;

    private void Start()
    {
        iconImage.sprite = myEffect.itemIcon;
        itemSize = new Vector2(myEffect.itemSize.x, myEffect.itemSize.y);
        sizeBlanks = GetBlanks(0);

        itemRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y); // this needs to be done only once, on init
        iconRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y);

        inventory = FindObjectOfType<Inventory>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        inventory.RegistarItem(this); // unregistar item when it's sold
    }

    private List<Vector2> GetBlanks(int ID)
    {
        List<Vector2> newBlanks = new List<Vector2>();
        if(myEffect.blankMaps.Length <= 2)
        {
            return newBlanks;
        }

        for(int i = 0; i < myEffect.blankMaps[ID].blanks.Length; i ++)
        {
            Vector2 newblank = new Vector2(myEffect.blankMaps[ID].blanks[i].x, myEffect.blankMaps[ID].blanks[i].y);
            newBlanks.Add(newblank);
        }
        return newBlanks;
    }

    public void SetPos(Vector2 pos)
    {
        itemRectTransform.anchoredPosition = pos;
        itemRectTransform.anchoredPosition += new Vector2(45 * (itemSize.x - 1), -45 * (itemSize.y - 1));
    }

    public void ResetPos()
    {
        if(wasFlipped)
        {
            wasFlipped = false;
            TryRotateItem(true);
        }

        if(wasInInventory)
        {
            List<slot> possibleSlots = inventory.GetPossibleSlots(this, gridPos);

            foreach (slot slot in possibleSlots)
            {
                slot.myItem = this;
            }
        }

        transform.localPosition = previousPos;
    }

    public void TryRotateItem(bool forceRotate)
    {
        previousPos = transform.localPosition;
        RemoveItem();

        transform.Rotate(new Vector3(0, 0, -90));
        float oldX = itemSize.x;
        itemSize.x = itemSize.y;
        itemSize.y = oldX;

        blankNum++;
        if(blankNum >= 4)
        {
            blankNum = 0;
        }
        sizeBlanks = GetBlanks(blankNum);

        if (forceRotate)
            return;

        if(inInventory)
        {
            slot mySlot = inventory.GetSlotFromGridPos(gridPos);
            wasFlipped = true;
            mySlot.TryPlaceItem(this);
            wasFlipped = false;
            
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

        iconImage.color = (state) ? new Color(1,1,1, 0.8f) : new Color(1,1,1,1);
        inventory.itemDragging = state;
    }

    public void RemoveItem()
    {
        if (!inInventory)
            return;

        List<slot> possibleSlots = inventory.GetPossibleSlots(this, gridPos);

        foreach (slot slot in possibleSlots)
        {
            if (slot.myItem == this)
            {
                slot.myItem = null;
            }
        }
    }

    public void ItemPressed()
    {
        if(isClicked)
        {
            TryRotateItem(false);
        }
        isClicked = true;
        StartCoroutine(DoubleClickDelay());
    }

    private IEnumerator DoubleClickDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isClicked = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        itemRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        inventory.drag.Drag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousPos = transform.localPosition;
        wasInInventory = inInventory;

        //inventory.RemoveItem(this, gridPos);

        if (inInventory)
        {
            RemoveItem();
        }

        // --- //
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventory.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        itemRectTransform.anchoredPosition = mousePos;
        itemRectTransform.anchoredPosition += new Vector2(-45 * (itemSize.x - 1) - 40, 45 * (itemSize.y - 1) + 40);
        inventory.drag.BeginDrag(itemSize);
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
        inventory.drag.EndDrag();
    }
}
