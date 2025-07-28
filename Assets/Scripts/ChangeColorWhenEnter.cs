using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeColorWhenEnter : MonoBehaviour
{
    [SerializeField]
    TMP_FontAsset wasEnteredFontAsset;

    [SerializeField]
    TextMeshPro doorText;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Color color = new Color(0.5f, 0f, 0.5f); //purple
            TextColorChange(color, doorText);
        }
    }

    void TextColorChange(Color newColor, TextMeshPro oldText)
    {
        if (oldText.faceColor == newColor) return;

        oldText.font = wasEnteredFontAsset;
    }
}
