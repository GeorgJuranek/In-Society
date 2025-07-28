using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArealChanger : MonoBehaviour
{
    [SerializeField]
    EAreas newArea;

    //[SerializeField]
    //TMP_FontAsset wasEnteredFontAsset;
    //
    //[SerializeField]
    //TextMeshPro doorText;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerController>().Area != newArea)
                other.GetComponent<PlayerController>().Area = newArea;
        }
    }

    //void TextColorChange(Color newColor, TextMeshPro oldText)
    //{
    //    if (oldText.faceColor == newColor) return;
    //
    //    oldText.font = wasEnteredFontAsset;
    //}
}
