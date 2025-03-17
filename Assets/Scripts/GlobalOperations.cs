using UnityEngine;

public class GlobalOperations : MonoBehaviour
{
    public bool renderUpdate;
    public char activeLetter;
    public ControlFile controls;
    private GameObject currentPopUp = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderUpdate = true; 
    }

    void Awake(){
        controls = new ControlFile();
        controls.Enable();
    }

    void OnApplicationQuit(){
        controls.Disable();
    }

    public void openPopUp(GameObject popUp){
        if (currentPopUp != null){
            return;
        }

        controls.Grid.Disable();
        currentPopUp = Instantiate(
            popUp,
            new Vector3(0, Screen.height - 1, 0),
            transform.rotation
        );
        RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        currentPopUp.transform.SetParent(canvas);
    }

    public void closePopUp(){
        if(currentPopUp == null){
            return;
        }

        controls.Grid.Enable();
        Destroy(currentPopUp);
    }



}
