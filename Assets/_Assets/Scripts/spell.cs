using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class spell : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI manaCostText;
    [SerializeField] TextMeshProUGUI spellNameText;
    [SerializeField] Image spellImage;

    actionButtons owner;

    public void Init(actionButtons owner, int manaCost, string spellName, Sprite spellIcon)
    {
        this.owner = owner;

        manaCostText.text = "" + manaCost;
        spellNameText.text = spellName;
        spellImage.sprite = spellIcon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        owner.hovingSpecialCanvas = true;
    }
}
