using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class itemEffect : ScriptableObject
{
    public Vector2 itemSize;

    [Header("Name")]
    public string itemName;
    public string spellName;

    [Header("Icon")]
    public Sprite itemIcon;
    public Sprite spellIcon;

    [Header("Spell Cost")]
    public int manaCost;

    public itemType myType;
    public enum itemType 
    { 
        consumable,
        spell,
        effect
    };
}
