using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public GridManager grid_manager;
    public Toolbox toolbox;
    private ControlFile controls;
    private bool clicked_grid; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clicked_grid = false;
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
        if (controls.Grid.MainClick.IsPressed()){
            if (!clicked_grid){
                toolbox.set_start_grid_pos();
            }
            toolbox.tool_draw();
            clicked_grid = true;
        }
        else if (clicked_grid && !controls.Grid.MainClick.IsPressed()){
            grid_manager.write_pbuffer_to_array();
            clicked_grid = false;
            toolbox.reset_start_grid_pos();
        }
    }
}
