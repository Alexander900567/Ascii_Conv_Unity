using UnityEngine;

public class InputHandler : MonoBehaviour
{

    public GridManager grid_manager;
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
            Vector3 mouse_pos = Input.mousePosition;
            (int row, int col) grid_pos = grid_manager.get_grid_pos(mouse_pos);
            
            grid_manager.add_to_preview_buffer(grid_pos.row, grid_pos.col, "a");
            clicked_grid = true;
        }
        else if (clicked_grid && !controls.Grid.MainClick.IsPressed()){
            grid_manager.write_pbuffer_to_array();
            clicked_grid = false;
        }
    }
}
