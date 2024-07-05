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
    public int baseDamage;
    public projectile spellProjectile;

    [Header("Description")]
    public description itemDescription;

    public bool targetsEnemies;

    public itemType myType;
    public enum itemType 
    { 
        consumable,
        spell,
        effect
    };
}
