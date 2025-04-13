using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SliderWithValue : MonoBehaviour
{
    [SerializeField] private Slider sliderMain;
    [SerializeField] private TextMeshProUGUI textField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateText();
    }

    public void updateText(){
        textField.text = Math.Round(sliderMain.value, 2).ToString();
    }
}
