using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class usableItem : MonoBehaviour, IPointerDownHandler
{
    public item itemOwner;
    private actionButtons actButtons;

    public Transform list;
    public Transform description;

    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] Image itemImage;

    [SerializeField] CanvasGroup group;

    [Header("Spells")]
    public bool isSpell;
    [SerializeField] TextMeshProUGUI manaCostText;

    public delegate void OnSelectItem(usableItem usingItem);
    public event OnSelectItem onSelectItem;


    public void Init(item itemOwner, actionButtons actButtons, Transform myList, Transform myDescription, string spellName, Sprite itemIcon, int manaCost, int availableMana)
    {
        this.itemOwner = itemOwner;

        this.actButtons = actButtons;

        list = myList;

        description = myDescription;

        itemText.text = spellName;
        itemImage.sprite = itemIcon;

        if(isSpell)
        {
            manaCostText.text = "" + manaCost;
            if(availableMana < manaCost)
            {
                group.alpha = 0.3f;
                group.interactable = false;
            }
        }
    }

    public bool IsAttackingSpell()
    {
        if(itemOwner.myEffect.targetsEnemies && isSpell)
        {
            return true;
        }
        return false;
    }

    public bool IsSelfConsumable()
    {
        if(itemOwner.myEffect.targetsSelf && !isSpell)
        {
            return true;
        }
        return false;
    }

    public void SelectThisItem()
    {
        StartCoroutine(SelectThisItemDelay());
    }

    private IEnumerator SelectThisItemDelay()
    {
        yield return new WaitForEndOfFrame();
        onSelectItem?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        actButtons.hovingSpecialCanvas = true;
    }
}
