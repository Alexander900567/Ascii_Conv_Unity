using System;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager grid_manager;
    public RectangleSelector rectangle_selector;
    public (int row, int col) prev_grid_pos; 
    public enum Tools{
        pencil,
        line,
        rectangle,
        circle,
        text,
        rectangle_selector,
    }
    public char active_letter;

    private (int row, int col) start_grid_pos;
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

    public void tool_draw(bool mouse_just_down){
        if (mouse_just_down){
            set_start_grid_pos();
        } 
        Vector3 mouse_pos = Input.mousePosition;
        (int row, int col) grid_pos = grid_manager.get_grid_pos(mouse_pos);

        if (active_tool == Tools.pencil){ 
            pencil(grid_pos, prev_grid_pos);
        }
        else if (active_tool == Tools.line){
            line(start_grid_pos, grid_pos, true);
        }
        else if (active_tool == Tools.rectangle){
            rectangle(start_grid_pos, grid_pos);
        }
        else if (active_tool == Tools.circle){
            circle(start_grid_pos, grid_pos);
        }
        else if (active_tool == Tools.rectangle_selector){
            if (mouse_just_down){
                rectangle_selector.on_mouse_down(grid_pos);
            }
            else{
                rectangle_selector.on_mouse_move(grid_manager, grid_pos);
            }
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
        bool clear_buffer
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
    private void circle(
        (int row, int col) start_grid_pos,
        (int row, int col) grid_pos
    ){
        grid_manager.empty_preview_buffer(); //Assume user will make a new circle

        int row_dif = grid_pos.row - start_grid_pos.row; //row and col components of
        int col_dif = grid_pos.col - start_grid_pos.col; //difference between start and end
        float diagonal_r = (float)Math.Sqrt((row_dif * row_dif) + (col_dif * col_dif));
        //pythag: c = sqrt(a^2 + b^2)
        //this is not yet usable due to the geometry of a grid in non-cardinal cases
        //Note about precision: if not good enough, make these floats into doubles
        int r;
        if (row_dif != 0 && col_dif != 0) { //non-cardinal case AKA trig time
            int o = Math.Abs(col_dif); //converts o to be positive to work with sin()
            float angle_theta = (float)Math.Asin(o / diagonal_r);
            float h = (float)(o / diagonal_r) / (float)Math.Sin(angle_theta); //hypotenuse
            float r0 = diagonal_r / h; //radius in terms of pixels
            r = (int)Math.Floor(r0); //floor makes our radius usable
        }
        else if (row_dif == 0 || col_dif == 0) {
            r = (int)Math.Floor(diagonal_r);
        }
        else {
            r = 0;
        }

        int row_num = 0;
        int col_num = r;
        int p = 1 - r;

        while (row_num <= col_num) { // draws 8 sections "simulataneously"
            grid_manager.add_to_preview_buffer(start_grid_pos.row + row_num, start_grid_pos.col + col_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row + col_num, start_grid_pos.col + row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - col_num, start_grid_pos.col + row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - row_num, start_grid_pos.col + col_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - row_num, start_grid_pos.col - col_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - col_num, start_grid_pos.col - row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row + col_num, start_grid_pos.col - row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row + row_num, start_grid_pos.col - col_num, active_letter);
        
            row_num += 1;
            if (p < 0) {
                p += 2 * row_num + 1;
            }
            else {
                col_num -= 1;
                p += 2 * (row_num - col_num) + 1;
            }
        }
    }
    public void text(){

        if(Input.GetKeyDown(KeyCode.Backspace)){
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
            prev_grid_pos.row = Mathf.Min(prev_grid_pos.row + 1, grid_manager.get_row_count() - 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            prev_grid_pos.col = Mathf.Max(prev_grid_pos.col - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
            prev_grid_pos.col = Mathf.Min(prev_grid_pos.col + 1, grid_manager.get_col_count() - 1);
        }
        else if (Input.anyKeyDown && Input.inputString.Length > 0){
            //Debug.Log(Input.inputString);
            grid_manager.add_to_grid_array(prev_grid_pos.row, prev_grid_pos.col, Input.inputString[0]);
            if (prev_grid_pos.col < grid_manager.get_col_count() - 1){
                prev_grid_pos.col += 1;
            }
        }

        global.render_update = true;
    }

}
