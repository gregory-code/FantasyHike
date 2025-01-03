using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatUpgrade : item
{
    [SerializeField] public int addedMaxHealth;
    [SerializeField] public int addedMaxMana;
    [SerializeField] public int addedStrength;
    [SerializeField] public int addedMagic;
    [SerializeField] public int addedDefense;

    public void Awake()
    {
        Player player = FindObjectOfType<Player>();

        player.AddStats(addedMaxHealth, addedMaxMana, addedStrength, addedMaxMana, addedDefense);

        Destroy(this.gameObject);
    }
}
