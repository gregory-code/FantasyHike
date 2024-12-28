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

    [SerializeField] Sprite commonIcon;
    [SerializeField] Sprite uncommonIcon;
    [SerializeField] Sprite rareIcon;
    [SerializeField] Sprite epicIcon;
    [SerializeField] Sprite legendaryIcon;

    [SerializeField] Color commonColor;
    [SerializeField] Color uncommonColor;
    [SerializeField] Color rareColor;
    [SerializeField] Color epicColor;
    [SerializeField] Color legendaryColor;

    public void Init(item itemInit)
    {
        myItem = itemInit;
        SetRariety(itemInit.myRariety);
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
