using UnityEngine;
using UnityEngine.UI;

public class ManualChanger : MonoBehaviour
{
    [SerializeField]
    GameObject elementToMove;

    float elementToMoveHeight;

    Slider slider;

    float startY;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        elementToMoveHeight = elementToMove.GetComponent<RectTransform>().rect.height;
        startY = elementToMove.GetComponent<RectTransform>().anchoredPosition.y;
    }

    public void OnSliderValueChanged(float value)
    {
        Vector2 newPosition = new Vector2(elementToMove.GetComponent<RectTransform>().anchoredPosition.x, startY + value * elementToMoveHeight);
        elementToMove.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }

}
