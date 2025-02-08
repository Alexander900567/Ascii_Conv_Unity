using UnityEngine;

public class GlobalOperations : MonoBehaviour
{
    public bool renderUpdate;
    public char activeLetter;
    public ControlFile controls;


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

    // Update is called once per frame
    void Update()
    {
        
    }
}
