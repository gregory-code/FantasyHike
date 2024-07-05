using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class specialCanvas : MonoBehaviour, IPointerDownHandler
{
    actionButtons owner;

    private void Start()
    {
        owner = transform.parent.GetComponent<actionButtons>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        owner.hovingSpecialCanvas = true;
    }
}
