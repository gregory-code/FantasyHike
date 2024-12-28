using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool bHoveringSellMenu = false;
    private ShopManager shopManager;

    public void Start()
    {
        shopManager = FindObjectOfType<ShopManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bHoveringSellMenu = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bHoveringSellMenu = false;
    }
}
