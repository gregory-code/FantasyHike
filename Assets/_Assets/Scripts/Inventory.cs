using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IDragHandler
{
    private Canvas canvas;

    private SaveManager saveManager;
    
    [SerializeField] slot slotPrefab;
    [SerializeField] DragIndicator dragPrefab;

    [SerializeField] RectTransform inventoryRectTransform;
    [SerializeField] RectTransform headerRectTransform;
    [SerializeField] Image moneyIcon;
    [SerializeField] TextMeshProUGUI moneyText;

    [SerializeField] int x = 2;
    [SerializeField] int y = 2;

    [SerializeField] private List<item> ItemLibrary = new List<item>();

    private int money = 0;

    [Header("Public, do not touch")]
    
    public DragIndicator drag;
    public List<slot> slots = new List<slot>();
    public List<item> items = new List<item>();

    public bool itemDragging;

    private void Start()
    {
        saveManager = GameObject.FindObjectOfType<SaveManager>();
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();

        FindObjectOfType<OnwardButton>().onOnward += Onward;

        drag = Instantiate(dragPrefab, transform);
        drag.Init(this, canvas);

        InitSaveData();
    }

    private void Onward()
    {
        item[] allItems = FindObjectsOfType<item>();

        for (int i = 0; i < allItems.Length; i++)
        {
            if (allItems[i].InInventory() == false)
            {
                int itemValue = Mathf.RoundToInt(allItems[i].buyPrice / 2);
                FindObjectOfType<ShopManager>().SellItem(allItems[i], itemValue);
            }
        }
    }

    public void AddInventorySize(int addedX, int addedY)
    {
        x += addedX;
        y += addedY;
        Resize(x, y, true);
    }

    public Vector2 GetInventorySize()
    {
        return new Vector2(x, y);
    }

    private void InitSaveData()
    {
        SetMoney(saveManager.saveData.money);

        int newSizeX = saveManager.saveData.sizeX;
        int newSizeY = saveManager.saveData.sizeY;
        x = newSizeX;
        y = newSizeY;
        Resize(newSizeX, newSizeY, false);

        for(int x = 0; x < saveManager.saveData.itemIDs.Count; x++)
        {
            for(int i = 0; i < ItemLibrary.Count; i++)
            {
                if (ItemLibrary[i].itemName == saveManager.GetIDName(x))
                {
                    item newItem = Instantiate(ItemLibrary[i], this.transform);
                    slot placingSlot = GetSlotFromGridPos(saveManager.GetIDGridPos(x));
                    newItem.Init(placingSlot.transform.localPosition, saveManager.GetIDRotation(x)); // slot local position
                    newItem.SetGridPos(saveManager.GetIDGridPos(x));
                    newItem.ForceAdjustment(true);
                    bool success = placingSlot.TryPlaceItem(newItem);
                }
            }
        }    
    }

    public List<item> GetItemLibrary()
    {
        return ItemLibrary;
    }

    public void SetMoney(int newMoney)
    {
        money = newMoney;
        saveManager.saveData.money = money;
        moneyText.text = "" + money;
    }

    public int GetMoney()
    {
        return money;
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Space))
        {
            Resize(x, y, true);
        }*/
    }

    private void Resize(int x, int y, bool save)
    {
        inventoryRectTransform.sizeDelta = new Vector2(x * 100, y * 100);
        headerRectTransform.sizeDelta = new Vector2(x * 100, 40);
        headerRectTransform.transform.localPosition = new Vector2(0, 70 + ((y - 1) * 50));

        float xslot = (-45 * (x - 1));
        float yslot = (45 * (y - 1));

        foreach (slot slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();

        for(int i = 0; i < x; i++)
        {

            for (int j = 0; j < y; j++)
            {
                slot newSlot = Instantiate(slotPrefab, inventoryRectTransform.transform);
                newSlot.Init(this, i, j);
                newSlot.transform.localPosition = new Vector2(xslot + (i * 90), yslot + (j * -90));
                slots.Add(newSlot);
            }
        }

        foreach(item item in items)
        {
            if(item.InInventory())
            {
                slot itemsSlot = GetSlotFromGridPos(item.GetGridPos());
                item.transform.SetAsLastSibling();
                itemsSlot.TryPlaceItem(item);
                item.SetPos(itemsSlot.transform.localPosition);
            }
        }

        if(save)
        {
            saveManager.saveData.sizeX = x;
            saveManager.saveData.sizeY = y;
        }
        moneyIcon.transform.localPosition = new Vector2(-43.33f * x, 0);
        moneyText.transform.localPosition = new Vector2(-28.33f * x, 0);
    }

    public void RegistarItem(item item, bool needsRegistaring)
    {
        if(needsRegistaring)
        {
            item.onItemAdjusted += ItemAdjusted; // make sure to unregistar I think
        }
        else
        {
            item.onItemAdjusted -= ItemAdjusted; // make sure to unregistar I think
        }
    }

    private void ItemAdjusted(item item, bool added)
    {
        if (items.Contains(item) && added)
            return;

        if(added)
        {
            items.Add(item);
            if (saveManager.saveData.itemIDs.Contains($"{item.itemName}/{item.GetGridPos().x}_{item.GetGridPos().y}/{item.GetBlankNum()}") == false)
                saveManager.saveData.itemIDs.Add($"{item.itemName}/{item.GetGridPos().x}_{item.GetGridPos().y}/{item.GetBlankNum()}");

        }
        else
        {
            items.Remove(item);
            if (saveManager.saveData.itemIDs.Contains($"{item.itemName}/{item.GetGridPos().x}_{item.GetGridPos().y}/{item.GetBlankNum()}"))
                saveManager.saveData.itemIDs.Remove($"{item.itemName}/{item.GetGridPos().x}_{item.GetGridPos().y}/{item.GetBlankNum()}");

        }
    }

    public List<item> GetItemType(item.itemType type)
    {
        List<item> consumables = new List<item>();
        foreach(item item in items)
        {
            if(item.myType == type)
            {
                consumables.Add(item);
            }
        }
        return consumables;
    }

    public List<slot> GetPossibleSlots(item item, Vector2 gridPos)
    {
        Vector2 itemSize = item.itemSize;
        List<slot> possibleSlots = new List<slot>();

        bool badGroup = false;

        float dimensionX = gridPos.x + (itemSize.x - 1);
        float dimensionY = gridPos.y + (itemSize.y - 1);

        List<Vector2> blanks = new List<Vector2>();
        for(int i = 0; i < item.GetSizeBlanks().Count; i ++)
        {
            Vector2 newBlank = new Vector2(item.GetSizeBlanks()[i].x - 1, item.GetSizeBlanks()[i].y - 1);
            newBlank.x += gridPos.x;
            newBlank.y += gridPos.y;
            blanks.Add(newBlank);
        }

        for (float i = gridPos.x; i <= dimensionX; i++)
        {
            for (float j = gridPos.y; j <= dimensionY; j++)
            {
                Vector2 gridCheck = new Vector2(i, j);
                bool skipHere = false;

                foreach(Vector2 blank in blanks)
                {
                    if (gridCheck == blank)
                    {
                        skipHere = true;
                    }
                }

                if (skipHere)
                    continue;

                if (gridCheck.x >= 0 && gridCheck.y >= 0 && gridCheck.x < x && gridCheck.y < y)
                {
                    slot slot = slots[GetIndexFromSlot(gridCheck)];
                    if (slot != null)
                    {
                        possibleSlots.Add(slot);
                    }
                }
                else
                {
                    badGroup = true;
                }
            }
        }

        if(badGroup)
        {
            if (possibleSlots.Count >= 1)
            {
                SetBadGroup(possibleSlots, true);
            }
        }

        return possibleSlots;
    }

    public void SetBadGroup(List<slot> slotsToSet, bool state)
    {
        foreach(slot slotToAdjust in slotsToSet)
        {
            slotToAdjust.badgroup = state;
        }
    }

    public bool GetBadGroup()
    {
        bool badGroup = false;
        foreach(slot slot in slots)
        {
            if (slot.badgroup)
                badGroup = true;
        }
        return badGroup;
    }

    public bool ValidSlotPlacement(List<slot> possibleSlots)
    {
        bool valid = true;
        foreach (slot slot in possibleSlots)
        {
            if (slot.myItem != null)
                valid = false;
        }
        return valid;
    }

    private int GetIndexFromSlot(Vector2 slot)
    {
        float index = slot.y + (slot.x * x);
        return (int)index;
    }

    public slot GetSlotFromGridPos(Vector2 gridPos)
    {
        foreach (slot slot in slots)
        {
            if (slot.gridPos == gridPos)
            {
                return slot;
            }
        }
        return slots[0];
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 previousPosition = inventoryRectTransform.anchoredPosition;

        inventoryRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRectTransform.sizeDelta;

        // Calculate the minimum and maximum anchored positions
        float minX = (-canvasSize.x / 2) + (50 * x); // Left edge of the canvas
        float maxX = (canvasSize.x / 2) - (50 * x); // Right edge of the canvas
        float minY = (-canvasSize.y / 2) - (50 * y); // Bottom edge of the canvas
        float maxY = (canvasSize.y / 2) - (50 * y); // Top edge of the canvas

        // Clamp the anchored position
        Vector2 clampedPosition = inventoryRectTransform.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        inventoryRectTransform.anchoredPosition = clampedPosition;

        Vector2 actualDelta = inventoryRectTransform.anchoredPosition - previousPosition;

        item[] allItems = FindObjectsOfType<item>();

        foreach (item movingItem in allItems)
        {
            if (movingItem.InInventory() == false && movingItem.GetInLootPool() == false)
            {
                movingItem.GetComponent<RectTransform>().anchoredPosition -= actualDelta;
            }
        }
    }
}
