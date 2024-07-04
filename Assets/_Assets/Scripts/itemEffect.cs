using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class itemEffect : ScriptableObject
{
    public Vector2 itemSize;
    public string itemName;

    public Sprite itemIcon;

    public itemType myType;
    public enum itemType 
    { 
        consumable,
        spell,
        effect
    };
}
