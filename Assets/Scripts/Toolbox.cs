using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GridManager grid_manager;
    public char active_tool;
    public char active_letter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        active_tool = 'p';        
        active_letter = 'a';
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tool_draw((int row, int col) mouse_pos, (int row, int col) prev_mouse_pos){
        if (active_tool == 'p'){
            pencil(mouse_pos, prev_mouse_pos);
        }
    }

    private void pencil((int row, int col) mouse_pos, (int row, int col) prev_grid_pos){
        if (mouse_pos != prev_grid_pos){
            grid_manager.add_to_preview_buffer(mouse_pos.row, mouse_pos.col, active_letter);
        }
    }

    private void line(){

    }

}
