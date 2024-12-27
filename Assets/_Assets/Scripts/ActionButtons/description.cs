using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class description : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI strengthTMP;
    [SerializeField] bool usesStrength;
    [SerializeField] string strengthString;

    [SerializeField] TextMeshProUGUI magicTMP;
    [SerializeField] bool usesMagic;
    [SerializeField] string magicString;

    public void SetStats(character owningCharacter, item usingItem)
    {
        if(usesStrength)
        {
            strengthTMP.text = $"{usingItem.baseValue + owningCharacter.baseStrength}{strengthString}"; 
        }

        if (usesMagic)
        {
            magicTMP.text = $"{usingItem.baseValue + owningCharacter.baseMagic}{magicString}";
        }
    }
}
