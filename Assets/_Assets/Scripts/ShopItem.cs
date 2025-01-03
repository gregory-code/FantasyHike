using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private item myItem;
    [SerializeField] Image itemIcon;
    [SerializeField] TextMeshProUGUI itemPriceText;
    [SerializeField] TextMeshProUGUI itemNameText;

    [SerializeField] Image rarietyBackground;
    [SerializeField] Image rarietyIcon;
    [SerializeField] TextMeshProUGUI rarietyText;
    [SerializeField] TextMeshProUGUI descriptoinText;

    [SerializeField] Sprite commonIcon;
    [SerializeField] Sprite uncommonIcon;
    [SerializeField] Sprite rareIcon;
    [SerializeField] Sprite epicIcon;
    [SerializeField] Sprite legendaryIcon;

    [SerializeField] Sprite takenSlotSprite;

    [SerializeField] Color commonColor;
    [SerializeField] Color uncommonColor;
    [SerializeField] Color rareColor;
    [SerializeField] Color epicColor;
    [SerializeField] Color legendaryColor;

    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] Image[] fakeSlots;

    private int myPrice;
    //public item testItem;

    public delegate void OnItemPicked(ShopItem shopItem);
    public event OnItemPicked onItemPicked;

    public void Init(item itemInit, bool freeItem)
    {
        if (freeItem)
            myPrice = 0;
        else
            myPrice = itemInit.buyPrice;

        myItem = itemInit;
        itemIcon.sprite = itemInit.itemIcon;
        itemNameText.text = itemInit.itemName;
        itemPriceText.text = myPrice + "";
        descriptoinText.text = itemInit.shopDescription;
        SetRariety(itemInit.myRariety);
        HighlightItem(itemInit);

        StartCoroutine(LateInit());

        FindObjectOfType<ShopManager>().onMoneyChanged += moneyChanged;
    }

    private IEnumerator LateInit()
    {
        yield return new WaitForEndOfFrame();
        CheckInteraction();
    }

    private void moneyChanged(int newMoney)
    {
        CheckInteraction();
    }

    public void Start()
    {
        //Init(testItem, true);
    }

    public void TryBuy()
    {
        Inventory playerInventory = FindObjectOfType<Inventory>();
        if(myPrice <= playerInventory.GetMoney())
        {
            FindObjectOfType<ShopManager>().onMoneyChanged -= moneyChanged;
            item spawnItem = Instantiate(myItem, this.transform);
            spawnItem.Init(new Vector3((spawnItem.itemSize.x - 1) * -45, (spawnItem.itemSize.y - 1) * 45, 0), 0);
            spawnItem.transform.SetParent(playerInventory.transform);
            spawnItem.transform.localScale = Vector3.one;
            playerInventory.SetMoney(playerInventory.GetMoney() - myPrice);

            onItemPicked?.Invoke(this);
            Destroy(this.gameObject);
        }
    }

    private void CheckInteraction()
    {
        int currentMoney = FindObjectOfType<Inventory>().GetMoney();
        if (myPrice > currentMoney)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.7f;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }
    }

    public void HighlightItem(item item)
    {
        List<Image> possibleSlots = GetPossibleSlots(item);

        foreach (Image slot in possibleSlots)
        {
            slot.sprite = takenSlotSprite;
        }
    }

    public List<Image> GetPossibleSlots(item item)
    {
        Vector2 itemSize = item.itemSize;
        List<Image> possibleSlots = new List<Image>();

        float dimensionX = (itemSize.x - 1);
        float dimensionY = (itemSize.y - 1);

        List<Vector2> blanks = new List<Vector2>();
        for (int i = 0; i < item.GetBlanks(0).Count; i++)
        {
            Vector2 newBlank = new Vector2(item.GetBlanks(0)[i].x - 1, item.GetBlanks(0)[i].y - 1);
            blanks.Add(newBlank);
        }

        for (float i = 0; i <= dimensionX; i++)
        {
            for (float j = 0; j <= dimensionY; j++)
            {
                Vector2 gridCheck = new Vector2(i, j);
                bool skipHere = false;

                foreach (Vector2 blank in blanks)
                {
                    if (gridCheck == blank)
                    {
                        skipHere = true;
                    }
                }

                if (skipHere)
                    continue;

                if (gridCheck.x >= 0 && gridCheck.y >= 0 && gridCheck.x < 3 && gridCheck.y < 3)
                {
                    Image slot = fakeSlots[GetIndexFromSlot(gridCheck)];
                    if (slot != null)
                    {
                        possibleSlots.Add(slot);
                    }
                }
            }
        }

        return possibleSlots;
    }

    private int GetIndexFromSlot(Vector2 slot)
    {
        float index = slot.y + (slot.x * 3);
        return (int)index;
    }

    private void SetRariety(item.itemRariety newRariety)
    {
        Color color = Color.white;
        switch(newRariety)
        {
            case item.itemRariety.common:
                color = commonColor;
                rarietyText.text = "Common";
                break;

            case item.itemRariety.uncommon:
                color = uncommonColor;
                rarietyText.text = "Uncommon";
                break;

            case item.itemRariety.rare:
                color = rareColor;
                rarietyText.text = "Rare";
                break;

            case item.itemRariety.epic:
                color = epicColor;
                rarietyText.text = "Epic";
                break;

            case item.itemRariety.legendary:
                color = legendaryColor;
                rarietyText.text = "Legendary";
                break;
        }

        rarietyBackground.color = new Color(color.r, color.g, color.b, rarietyBackground.color.a);
        rarietyText.color = new Color(color.r, color.g, color.b, rarietyText.color.a);
        rarietyIcon.color = new Color(color.r, color.g, color.b, rarietyIcon.color.a);

    }
}
