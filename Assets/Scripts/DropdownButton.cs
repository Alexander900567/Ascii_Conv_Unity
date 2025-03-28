using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DropdownButton : MonoBehaviour
{

    private GlobalOperations global;
    private List<Transform> buttonList;    
    private Transform backPanel;
    private bool active;
    private int releaseCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        global = GameObject.Find("GlobalOperations").GetComponent<GlobalOperations>();
        buttonList = new List<Transform>();
        releaseCount = 0;

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
        }
    }

    void Update(){
        if (active && global.controls.Grid.MainClick.WasReleasedThisFrame()){
            releaseCount += 1;
        }
        else if(releaseCount >= 2){
            releaseCount = 0;
            hideDropdown();
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

    }
}
