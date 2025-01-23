using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager grid_manager;
    public Toolbox toolbox;
    private ControlFile controls;
    private bool clicked_grid; 
    private (int row, int col) prev_grid_pos; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        clicked_grid = false;
        prev_grid_pos = (-1, -1);
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
            Vector3 mouse_pos = Input.mousePosition;
            (int row, int col) grid_pos = grid_manager.get_grid_pos(mouse_pos);


            if (prev_grid_pos != grid_pos){
                toolbox.tool_draw(grid_pos, prev_grid_pos);
            }


            if (prev_grid_pos != grid_pos){
                global.render_update = true;
            }
            prev_grid_pos = grid_pos;
            clicked_grid = true;
        }
        else if (clicked_grid && !controls.Grid.MainClick.IsPressed()){
            grid_manager.write_pbuffer_to_array();
            clicked_grid = false;
        }
    }
}
