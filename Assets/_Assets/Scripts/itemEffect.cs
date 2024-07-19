using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class itemEffect : ScriptableObject
{
    public Vector2 itemSize;
    public blankMap[] blankMaps;

    [Header("Name")]
    public string itemName;

    [Header("Icon")]
    public Sprite itemIcon;

    [Header("Spells")]
    public int manaCost;
    public int baseDamage;
    public string spellName;
    public Sprite spellIcon;
    public projectile spellProjectile;

    [Header("Items")]
    public int baseValue;
    public GameObject particle;

    [Header("Description")]
    public description itemDescription;

    public bool targetsEnemies;
    public bool targetsSelf;

    public itemType myType;
    public enum itemType 
    { 
        consumable,
        spell,
        effect
    };
}