using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class SliderWithValue : MonoBehaviour
{
    [SerializeField] private Slider sliderMain;
    [SerializeField] private TMP_InputField inputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        matchInputToSlider();
    }

    public void matchInputToSlider(){
        inputField.text = Math.Round(sliderMain.value, 2).ToString();
    }

    public void matchSliderToInput(){
        float newValue;
        bool isFloat = float.TryParse(inputField.text, out newValue);
        if(!isFloat){
            return;
        }
        sliderMain.value = (float) Math.Round(newValue, 2);
    }

}
