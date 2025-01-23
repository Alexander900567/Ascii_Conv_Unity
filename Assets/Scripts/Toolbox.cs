using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager grid_manager;
    public char active_tool;
    public char active_letter;
    private (int row, int col) prev_grid_pos; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        active_tool = 'p';        
        active_letter = 'a';
        prev_grid_pos = (-1, -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tool_draw(){
        Vector3 mouse_pos = Input.mousePosition;
        (int row, int col) grid_pos = grid_manager.get_grid_pos(mouse_pos);


        if (active_tool == 'p'){
            pencil(grid_pos, prev_grid_pos);
        }

        if (prev_grid_pos != grid_pos){
            global.render_update = true;
        }
        prev_grid_pos = grid_pos;
    }

    private void pencil((int row, int col) grid_pos, (int row, int col) prev_grid_pos){
        if (grid_pos != prev_grid_pos){
            grid_manager.add_to_preview_buffer(grid_pos.row, grid_pos.col, active_letter);
        }
    }

    private void line(){

    }

}
