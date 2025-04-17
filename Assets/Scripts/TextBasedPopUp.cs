using UnityEngine;

public class TextBasedPopUp : MonoBehaviour
{
    [SerializeField] private GlobalOperations globalOperations;
    [SerializeField] private GameObject keybindsPopUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*    void Start(){
        keybindsPopUp = GameObject.Find("KeybindsPopUp").GetComponent<KeybindsPopUp>();
    } */
    public void displayKeybindsPopUp(){
        globalOperations.openPopUp(keybindsPopUp);
    }

    public void test(){
        Debug.Log("Test");
    }
}