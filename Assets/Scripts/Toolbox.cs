using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager grid_manager;
    public char active_tool;
    public char active_letter;
    private (int row, int col) prev_grid_pos; 
    private (int row, int col) start_grid_pos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prev_grid_pos = (-1, -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void set_start_grid_pos(){
        start_grid_pos = grid_manager.get_grid_pos(Input.mousePosition);
    }

    public void reset_start_grid_pos(){
        start_grid_pos = (0, 0);
    }

    public void tool_draw(){
        Vector3 mouse_pos = Input.mousePosition;
        (int row, int col) grid_pos = grid_manager.get_grid_pos(mouse_pos);


        if (active_tool == 'p'){
            pencil(grid_pos, prev_grid_pos);
        }
        else if (active_tool == 'l'){
            line(start_grid_pos, grid_pos);
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

    private void line(
        (int row, int col) start_grid_pos, 
        (int row, int col) grid_pos, 
        bool clear_buffer=true
    ){
        
        //Debug.Log($"previewbuffer: {grid_manager.preview_buffer}");
        
        if (clear_buffer){
            grid_manager.empty_preview_buffer();
        }

        Debug.Log($"start_grid_pos: {start_grid_pos}");
        Debug.Log($"grid_pos: {grid_pos}");

        int horizontal_slope = grid_pos.row - start_grid_pos.row;
        int vertical_slope = grid_pos.col - start_grid_pos.col;
        int row_iter = 0;
        int col_iter = 0;

        if (horizontal_slope != 0){
            row_iter = horizontal_slope / System.Math.Abs(horizontal_slope);
        }
        if (vertical_slope != 0){
            col_iter = vertical_slope / System.Math.Abs(vertical_slope);
        }

        horizontal_slope = System.Math.Abs(horizontal_slope);
        vertical_slope = System.Math.Abs(vertical_slope);

        int long_slope;
        int short_slope;
        bool row_length_is_long;

        if (horizontal_slope > vertical_slope){
            long_slope = horizontal_slope;
            short_slope = vertical_slope + 1;
            row_length_is_long = true;
        }        
        else{
            long_slope = vertical_slope;
            short_slope = horizontal_slope + 1;
            row_length_is_long = false;
        }

        int per_chunk = long_slope / short_slope;
        int extra = (long_slope % short_slope) + 1;

        Debug.Log($"per_chunk: {per_chunk}");
        Debug.Log($"extra: {extra}");

        for (int x = 0; x < short_slope; x++){
            int this_chunk = per_chunk;
            if (extra > 0){
                this_chunk += 1;
                extra -= 1;
            }
            for (int y = 0; y < this_chunk; y++){
                Debug.Log($"adding: {grid_pos.row}, {grid_pos.col}");
                grid_manager.add_to_preview_buffer(grid_pos.row, grid_pos.col, active_letter);
                if (row_length_is_long){
                    grid_pos.row += row_iter;
                }
                else {
                    grid_pos.col += col_iter;
                }
            }
            if (!row_length_is_long){
                grid_pos.row += row_iter;
            }
            else {
                grid_pos.col += col_iter;
            }
        }

        
    }

}
