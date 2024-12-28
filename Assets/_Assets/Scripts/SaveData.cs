using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SaveData", menuName = "SaveSystem/SaveData")]
public class SaveData : ScriptableObject
{
    public int sizeX = 2; // inventory size
    public int sizeY = 2;

    public int money = 0;

    public List<string> itemNames = new List<string>();
    public List<Vector2> itemGridPos = new List<Vector2>();

    public void ClearSaveData() // currently not being used
    {
        sizeX = 2;
        sizeY = 2;

        money = 0;

        itemNames = new List<string>();
        itemGridPos = new List<Vector2>();
    }
}