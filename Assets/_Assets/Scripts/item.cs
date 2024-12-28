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

    private RectTransform itemRectTransform;
    private RectTransform iconRectTransform;
    private Image iconImage;
    
    private List<slot> slots = new List<slot>();
    private List<Vector2> sizeBlanks = new List<Vector2>();
    private Vector2 previousPos;
    private bool isClicked;
    private bool wasFlipped;
    private bool wasInInventory;
    private bool inInventory;
    private Vector2 gridPos;
    private int blankNum;
    private bool bInitalized;

    [Header("Item")]

    public Vector2 itemSize;
    public blankMap[] blankMaps;

    public itemType myType;
    public enum itemType
    {
        consumable,
        spell,
        effect
    };

    [Header("Name")]
    public string itemName;

    [Header("Icon")]
    public Sprite itemIcon;

    [Header("Spell Specific")]
    public int manaCost;
    public string spellName;
    public Sprite spellIcon;
    public projectile spellProjectile;

    [Header("Item Specific")]
    public GameObject particle;

    [Header("All Effects")]
    public int baseValue;

    public description itemDescription;
    public bool targetsEnemies;
    public bool targetsSelf;

    public delegate void OnDropItem(item item);
    public event OnDropItem onDropItem;

    public delegate void OnItemAdjusted(item item, bool added);
    public event OnItemAdjusted onItemAdjusted;

    public delegate void OnUseItem(item usingItem, character usingCharacter, character recivingCharacter);
    public event OnUseItem onUseItem;

    public void Start()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.2f);
        if (bInitalized == false)
            Init(new Vector2(0, 0));
    }

    public void Init(Vector2 spawnPos)
    {
        itemRectTransform = GetComponent<RectTransform>();
        iconRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        iconImage = transform.GetChild(0).GetComponent<Image>();

        iconImage.sprite = itemIcon;
        sizeBlanks = GetBlanks(0);

        itemRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y); // this needs to be done only once, on init
        iconRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y);

        inventory = FindObjectOfType<Inventory>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        inventory.RegistarItem(this); // unregistar item when it's sold

        bInitalized = true;

        SetPos(spawnPos);
    }

    public void UseItem(item usingItem, character usingCharacter, character recivingCharacter)
    {
        onUseItem?.Invoke(usingItem, usingCharacter, recivingCharacter);
    }

    private List<Vector2> GetBlanks(int ID)
    {
        List<Vector2> newBlanks = new List<Vector2>();
        if(blankMaps.Length <= 2)
        {
            return newBlanks;
        }

        for(int i = 0; i < blankMaps[ID].blanks.Length; i ++)
        {
            Vector2 newblank = new Vector2(blankMaps[ID].blanks[i].x, blankMaps[ID].blanks[i].y);
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

    public bool InInventory()
    {
        return inInventory;
    }

    public void SetInInventory(bool state)
    {
        inInventory = state;
    }

    public Vector2 GetGridPos()
    {
        return gridPos;
    }

    public void SetGridPos(Vector2 newGridPos)
    {
        SaveManager saveManager = FindObjectOfType<SaveManager>();
        for (int i = 0; i < saveManager.saveData.itemGridPos.Count; i++)
        {
            if (saveManager.saveData.itemNames[i] == itemName)
            {
                saveManager.saveData.itemGridPos[i] = newGridPos;
                break;
            }
        }
        saveManager.Save();
        gridPos = newGridPos;
    }

    public List<Vector2> GetSizeBlanks()
    {
        return sizeBlanks;
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
