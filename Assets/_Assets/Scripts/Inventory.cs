using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour, IDragHandler
{
    [SerializeField] Canvas canvas;
    [SerializeField] slot slotPrefab;

    [SerializeField] RectTransform inventoryRectTransform;
    [SerializeField] RectTransform headerRectTransform;

    List<slot> slots = new List<slot>();

    [SerializeField] int x = 3;
    [SerializeField] int y = 3;

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
    }

    public List<slot> GetInventorySlots()
    {
        return slots;
    }

    private Vector2 GetTileFromIndex(int index)
    {
        Vector2 slot;
        slot.x = index % y;
        slot.y = index / y;

        return slot;
    }

    float GetIndexFromSlot(Vector2 slot)
    {
        float index = slot.y + (slot.y * y);
        return index;
    }

    public bool isSlotsValid(item item, slot slot)
    {
        Vector2 itemSize = item.GetItemSize();
        Vector2 gridPos = slot.GetGridPos();

        for (float i = gridPos.x; i <= itemSize.x; i++)
        {
            for (float j = gridPos.y; j <= itemSize.y; j++)
            {
                Vector2 check = new Vector2(i, j);
                if (check.x >= 0 && check.y >= 0 && check.x < this.x && check.y < this.y)
                {
                    item checkItem = slots[(int)GetIndexFromSlot(check)].GetItem();
                    if (checkItem == null)
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        inventoryRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
