using UnityEngine;

public class GlobalOperations : MonoBehaviour
{
    public bool render_update;
    public char active_letter;
    public ControlFile controls;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        render_update = true; 
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
