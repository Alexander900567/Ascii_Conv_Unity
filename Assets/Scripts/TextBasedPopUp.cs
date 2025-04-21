using UnityEngine;
using TMPro;

public class TextBasedPopUp : MonoBehaviour
{
    [SerializeField] private GlobalOperations globalOperations;
    [SerializeField] private GameObject keybindsPopUp;
    [SerializeField] private TMPro.TextMeshProUGUI keybindsText;

    public void displayKeybindsPopUp()
    {
        // Load the file from Resources
        TextAsset file = Resources.Load<TextAsset>("keybinds"); //Finds file in Resources folder
        if (file != null)
        {
            keybindsText.text = file.text;
        }
        else
        {
            keybindsText.text = "Keybinds file not found.";
        }

        globalOperations.openPopUp(keybindsPopUp);
    }
}
