/* using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StrokeWidthPopUp : MonoBehaviour
{
    [SerializeField] private GameObject globalOperations;
    [SerializeField] private GameObject strokeWidthPopUp;
    [SerializeField] private Slider strokeWidthSlider;
    [SerializeField] private TMP_InputField strokeWidthInput;
    [SerializeField] private TextMeshProUGUI strokeWidthTextField;
    void Start()
    {
        updateText();
    }
    public void updateText(){
        strokeWidthTextField.text = Math.Round(strokeWidthSlider.value, 2).ToString();
    }
    public void displayStrokeWidthPopUp(){
        globalOperations.openPopUp(strokeWidthPopUp);
    }
    private void changeStrokeWidth(){
        Toolbox.setStrokeWidth(target);
    }

}
 */