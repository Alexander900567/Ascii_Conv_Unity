using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

public class TextBasedPopUp : MonoBehaviour
{
    [SerializeField] private GlobalOperations globalOperations;
    [SerializeField] private GameObject keybindsPopUp;
    [SerializeField] private TMPro.TextMeshProUGUI keybindsText; //Gets keybinds.txt from files
    [SerializeField] private GameObject aboutPopUp;
    [SerializeField] private GameObject strokeWidthPopUp;
    [SerializeField] private Slider strokeWidthSlider;
    [SerializeField] private TMP_InputField strokeWidth;

    void Start()
    {
        strokeWidth.onValueChanged.AddListener(delegate { updateSlider(); });
        strokeWidth.text = Toolbox.getStrokeWidth().ToString();
        strokeWidthSlider.value = (float)Toolbox.getStrokeWidth();
    }
    public void updateFieldText(){
        strokeWidth.text = (strokeWidthSlider.value).ToString();
        Toolbox.setStrokeWidth((int)strokeWidthSlider.value);
    }
    public void updateSlider()
    {
        if (int.TryParse(strokeWidth.text, out int target))
        {
            strokeWidthSlider.value = target;
            Toolbox.setStrokeWidth(target);
        }
    }

    public void displayKeybindsPopUp(){
        // Load the file from Resources
        TextAsset file = Resources.Load<TextAsset>("keybinds"); //Finds file in Resources folder
        if (file != null)
        {
            keybindsText.text = file.text;
        }
        else
        {
            keybindsText.text = "Keybinds file not found. Does Assets/Resources/keybinds.txt exist?";
        }

        globalOperations.openPopUp(keybindsPopUp);
    }
    public void displayAboutPopUp(){
        globalOperations.openPopUp(aboutPopUp);
    }
    public void displayStrokeWidthPopUp(){
        globalOperations.openPopUp(strokeWidthPopUp);
    }
}
