using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class item : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] Canvas canvas;

    [SerializeField] Vector2 itemSize;

    [SerializeField] RectTransform itemRectTransform;

    private List<slot> slots = new List<slot>();
    
    private Vector2 previousPos;

    public delegate void OnDropItem(item item);
    public event OnDropItem onDropItem;


    private void Start()
    {
        itemRectTransform.sizeDelta = new Vector2(90 * itemSize.x, 90 * itemSize.y);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //itemRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePos);
        itemRectTransform.anchoredPosition = mousePos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousPos = transform.localPosition;
        slots = FindObjectOfType<Inventory>().GetInventorySlots();
        ItemInteract(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDropItem?.Invoke(this);
        ItemInteract(false);
    }

    public void ResetPos()
    {
        transform.localPosition = previousPos;
    }

    public Vector2 GetItemSize()
    {
        return itemSize;
    }

    private void ItemInteract(bool state)
    {
        foreach (slot slot in slots)
        {
            slot.ItemDragState(this, state);
        }

        GetComponent<Image>().raycastTarget = !state;
    }
}
