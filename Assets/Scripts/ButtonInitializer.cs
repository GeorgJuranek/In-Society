using UnityEngine;
using UnityEngine.UI;

public class ButtonInitializer : MonoBehaviour
{
    //NOTE: This script is a modified version of a script from the Internet

    private Button[] buttons;

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button.name));// uses the name of the button to get method in singleton
        }
    }

    private void OnButtonClick(string methodName)
    {
        System.Reflection.MethodInfo method = typeof(MenuMethods).GetMethod(methodName);

        if (method != null)
        {
            method.Invoke(MenuMethods.Instance, null);
        }
    }
}