using UnityEditor;
using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager grid_manager;
    public char active_letter;
    public (int row, int col) prev_grid_pos; 
    private (int row, int col) start_grid_pos;

    public enum Tools{
        pencil,
        line,
        rectangle,
        text,
    }
    public Tools active_tool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prev_grid_pos = (-1, -1);
        active_tool = Tools.pencil;
    }

    // Update is called once per frame
    void Update()
    {
        if (active_tool == Tools.text){
            grid_manager.RenderTextCursor(prev_grid_pos);
        }
    }

    public void set_start_grid_pos(){
        start_grid_pos = grid_manager.get_grid_pos(Input.mousePosition);
    }

    public void reset_start_grid_pos(){
        start_grid_pos = (-1, -1);
    }

    public void tool_draw(){
        Vector3 mouse_pos = Input.mousePosition;
        (int row, int col) grid_pos = grid_manager.get_grid_pos(mouse_pos);


        if (active_tool == Tools.pencil){ 
            pencil(grid_pos, prev_grid_pos);
        }
        else if (active_tool == Tools.line){
            line(start_grid_pos, grid_pos);
        }
        else if (active_tool == Tools.rectangle){
            rectangle(start_grid_pos, grid_pos);
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
        
        if (clear_buffer){
            grid_manager.empty_preview_buffer();
        }

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

        for (int x = 0; x < short_slope; x++){
            int this_chunk = per_chunk;
            if (extra > 0){
                this_chunk += 1;
                extra -= 1;
            }
            for (int y = 0; y < this_chunk; y++){
                grid_manager.add_to_preview_buffer(start_grid_pos.row, start_grid_pos.col, active_letter);
                if (row_length_is_long){
                    start_grid_pos.row += row_iter;
                }
                else {
                    start_grid_pos.col += col_iter;
                }
            }
            if (!row_length_is_long){
                start_grid_pos.row += row_iter;
            }
            else {
                start_grid_pos.col += col_iter;
            }
        }
    }

    private void rectangle(
        (int row, int col) start_grid_pos, 
        (int row, int col) grid_pos
    ){
        grid_manager.empty_preview_buffer();

        line(
            (start_grid_pos.row, start_grid_pos.col),
            (start_grid_pos.row, grid_pos.col),
            false
        );
        line(
            (start_grid_pos.row, start_grid_pos.col),
            (grid_pos.row, start_grid_pos.col),
            false
        );
        line(
            (grid_pos.row, start_grid_pos.col),
            (grid_pos.row, grid_pos.col),
            false
        );
        line(
            (start_grid_pos.row, grid_pos.col),
            (grid_pos.row, grid_pos.col),
            false
        );
    }

    public void text(){

        if (Input.GetKeyDown(KeyCode.Escape)){
            active_tool = Tools.pencil;
        }
        else if(Input.GetKeyDown(KeyCode.Backspace)){
            if (prev_grid_pos.col == 0){
                grid_manager.add_to_grid_array(prev_grid_pos.row, prev_grid_pos.col, ' ');
            }
            else{
                grid_manager.add_to_grid_array(prev_grid_pos.row, prev_grid_pos.col - 1, ' ');
                prev_grid_pos.col -= 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)){
            prev_grid_pos.row = Mathf.Max(prev_grid_pos.row - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            prev_grid_pos.row = Mathf.Min(prev_grid_pos.row + 1, grid_manager.row_count - 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            prev_grid_pos.col = Mathf.Max(prev_grid_pos.col - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
            prev_grid_pos.col = Mathf.Min(prev_grid_pos.col + 1, grid_manager.col_count - 1);
        }
        else if (Input.anyKeyDown && Input.inputString.Length > 0){
            //Debug.Log(Input.inputString);
            grid_manager.add_to_grid_array(prev_grid_pos.row, prev_grid_pos.col, Input.inputString[0]);
            if (prev_grid_pos.col < grid_manager.col_count - 1){
                prev_grid_pos.col += 1;
            }
        }

        global.render_update = true;
    }

}
