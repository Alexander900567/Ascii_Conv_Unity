using UnityEngine;

public class MiscTopBar : MonoBehaviour
{
    [SerializeField] GlobalOperations global;
    [SerializeField] GameObject aboutPopUp;
    [SerializeField] GameObject keyBindsPopUp;

    public void openKeybindPopup(){
        global.openPopUp(keyBindsPopUp);
    }

    public void openAboutPopup(){
        global.openPopUp(aboutPopUp);
    }
}
