using UnityEngine;
using UnityEngine.UI;

public class ToolToggleParent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        ToggleGroup toggleGroup = gameObject.GetComponent<ToggleGroup>(); 

        foreach(Transform child in gameObject.transform){
            Toggle toggle = child.GetComponent<Toggle>();
            if(toggle != null){
                toggle.group = toggleGroup;
            }
        }
    }

}
