using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DropdownButton : MonoBehaviour
{

    private List<Transform> buttonList;    
    private Transform backPanel;
    private TextMeshProUGUI dropdownTextComponent;
    private bool active;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        buttonList = new List<Transform>();

        foreach(Transform child in gameObject.transform){
            //record buttons
            Button button = child.GetComponent<Button>();
            if(button != null){
                buttonList.Add(child);
            }
            //record backPanel
            else if(child.name == "BackPanel"){
                backPanel = child;
            }
            //rectord dropdownText
            else if(child.name == "DropdownText"){
                dropdownTextComponent = child.GetComponent<TextMeshProUGUI>();
                dropdownTextComponent.text += " \u2193";
            }
        }

    }

    public void showDropdown(){
        changeDropdownStatus(true);
    }
    public void hideDropdown(){
        changeDropdownStatus(false);
    }
    public void toggleDropown(){
        changeDropdownStatus(!active);
    }
    private void changeDropdownStatus(bool visible){
        active = visible;
        backPanel.gameObject.SetActive(visible);
        foreach(Transform button in buttonList){
            button.gameObject.SetActive(visible);
        }

        dropdownTextComponent.text = dropdownTextComponent.text.Substring(0, dropdownTextComponent.text.Length - 2);
        if (visible){
            dropdownTextComponent.text += " \u2191";
        }
        else{
            dropdownTextComponent.text += " \u2193";
        }
    }
}
