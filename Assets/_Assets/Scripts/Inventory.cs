using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IDragHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] slot slotPrefab;

    [SerializeField] RectTransform inventoryRectTransform;
    [SerializeField] RectTransform headerRectTransform;

    [SerializeField] int x = 2;
    [SerializeField] int y = 2;

    [Header("Public, do not touch")]
    
    public List<slot> slots = new List<slot>();
    public List<item> items = new List<item>();

    public bool itemDragging;

    private void Start()
    {
        Resize(x, y);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Resize(x, y);
        }
    }

    private void Resize(int x, int y)
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
            if(item.inInventory)
            {
                slot itemsSlot = GetSlotFromGridPos(item.gridPos);
                item.transform.SetAsLastSibling();
                itemsSlot.TryPlaceItem(item);
                item.wasFlipped = false;
                item.SetPos(itemsSlot.transform.localPosition);
            }
        }
    }

    public void RegistarItem(item item)
    {
        item.onItemAdjusted += ItemAdjusted; // make sure to unregistar I think
    }

    private void ItemAdjusted(item item, bool added)
    {
        if (items.Contains(item) && added)
            return;

        if(added)
        {
            items.Add(item);
        }
        else
        {
            items.Remove(item);
        }
    }

    public List<slot> GetPossibleSlots(item item, Vector2 gridPos)
    {
        Vector2 itemSize = item.itemSize;
        List<slot> possibleSlots = new List<slot>();

        float dimensionX = gridPos.x + (itemSize.x - 1);
        float dimensionY = gridPos.y + (itemSize.y - 1);

        for (float i = gridPos.x; i <= dimensionX; i++)
        {
            for (float j = gridPos.y; j <= dimensionY; j++)
            {
                Vector2 gridCheck = new Vector2(i, j);

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
                    possibleSlots[0].badgroup = true;
                }
            }
        }

        return possibleSlots;
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

    private slot GetSlotFromGridPos(Vector2 gridPos)
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
        inventoryRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
