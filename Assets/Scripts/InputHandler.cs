using UnityEditor;
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
        Vector3 mouse_pos = Input.mousePosition;

        if (mouse_pos.x > grid_manager.ui_manager.ui_panel_transform.rect.width && controls.Grid.MainClick.IsPressed()){
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
        else if (controls.Grid.PenSwitch.IsPressed()) {
            switch_to_pencil();
        }
        else if (controls.Grid.LineSwitch.IsPressed()) {
            switch_to_line();
        }
        else if (controls.Grid.RectangleSwitch.IsPressed()) {
            switch_to_rectangle();
        }
    }

    public void switch_to_pencil(){
        toolbox.active_tool = Toolbox.Tools.pencil;
    }

    public void switch_to_line(){
        toolbox.active_tool = Toolbox.Tools.line;
    }

    public void switch_to_rectangle(){
        toolbox.active_tool = Toolbox.Tools.rectangle;
    }
}
