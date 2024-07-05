using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class description : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI strengthTMP;
    [SerializeField] bool usesStrength;
    [SerializeField] int baseStrength;
    [SerializeField] string strengthString;

    [SerializeField] TextMeshProUGUI magicTMP;
    [SerializeField] bool usesMagic;
    [SerializeField] int baseMagic;
    [SerializeField] string magicString;

    public void SetStats(int strength, int magic)
    {
        if(usesStrength)
        {
            strengthTMP.text = $"{strength + baseStrength}{strengthString}"; 
        }

        if (usesMagic)
        {
            magicTMP.text = $"{magic + baseMagic}{magicString}";
        }
    }
}
