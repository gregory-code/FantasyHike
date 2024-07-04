using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class actionBtn : Button
{
    public delegate void OnActionClicked(string ID);
    public event OnActionClicked onActionClicked;

    private Transform buttonParent;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (buttonParent == null)
        {
            buttonParent = transform.parent.transform.parent.transform;
        }

        if (interactable == false)
            return;

        onActionClicked?.Invoke(buttonParent.name);
    }

    public void InteractState(bool state, Sprite unusable, Sprite usable)
    {
        if (buttonParent == null)
        {
            buttonParent = transform.parent.transform.parent.transform;
        }

        interactable = state;
        buttonParent.GetComponent<SpriteRenderer>().sprite = (state) ? usable : unusable;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
