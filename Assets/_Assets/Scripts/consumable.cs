using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class consumable : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] Image itemImage;

    public void Init(string itemName, Sprite itemIcon)
    {
        itemNameText.text = itemName;
        itemImage.sprite = itemIcon;
    }
}
