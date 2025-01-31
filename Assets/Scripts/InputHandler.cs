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

        /*
        if (Input.anyKeyDown){
            Debug.Log(Input.anyKeyDown);
            Debug.Log(Input.inputString);
        }
        */

        if (mouse_pos.x > grid_manager.ui_manager.ui_panel_transform.rect.width && controls.Grid.MainClick.IsPressed()){
            if (!clicked_grid){
                toolbox.set_start_grid_pos();
            }
            toolbox.tool_draw();
            clicked_grid = true;
        }
        else if (toolbox.active_tool == Toolbox.Tools.text){
            toolbox.text();
            if (Input.GetKeyDown(KeyCode.Escape)){
                switch_to_pencil();
            }
        }
        else if (clicked_grid && !controls.Grid.MainClick.IsPressed()){
            grid_manager.write_pbuffer_to_array();
            clicked_grid = false;
            toolbox.reset_start_grid_pos();
        }
        else if (controls.Grid.PenSwitch.IsPressed()) {
            switch_to_pencil();
        }
        else if (controls.Grid.EraserSwitch.IsPressed()) {
            switch_to_eraser();
        }
        else if (controls.Grid.LineSwitch.IsPressed()) {
            switch_to_line();
        }
        else if (controls.Grid.RectangleSwitch.IsPressed()) {
            switch_to_rectangle();
        }
        else if (controls.Grid.FilledRectangleSwitch.IsPressed()) {
            switch_to_filled_rectangle();
        }
        else if (controls.Grid.CircleSwitch.IsPressed()){
            switch_to_circle();
        }
        else if (controls.Grid.FilledCircleSwitch.IsPressed()) {
            switch_to_filled_circle();
        }
        else if (controls.Grid.TextSwitch.IsPressed()){
            switch_to_text();
        }
    }

    public void switch_to_pencil(){
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.pencil;
    }

    public void switch_to_eraser(){
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.eraser;
    }
    public void switch_to_line(){
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.line;
    }

    public void switch_to_rectangle(){
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.rectangle;
    }

    public void switch_to_filled_rectangle() {
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.filled_rectangle;
    }
    public void switch_to_circle(){
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.circle;
    }

    public void switch_to_filled_circle() {
        tools_unclick();
        toolbox.active_tool = Toolbox.Tools.filled_circle;
    }
    public void switch_to_text(){
        tools_unclick();
        grid_manager.text_cursor.localScale = new Vector3(1, 1, 1);
        if (toolbox.prev_grid_pos.row == -1) { toolbox.prev_grid_pos = (0, 0); }
        toolbox.active_tool = Toolbox.Tools.text;
    }

    private void tools_unclick(){
        if (toolbox.active_tool == Toolbox.Tools.text){
            grid_manager.text_cursor.localScale = new Vector3(0, 0, 0);
            toolbox.prev_grid_pos = (-1, -1);
        }
    }
}
