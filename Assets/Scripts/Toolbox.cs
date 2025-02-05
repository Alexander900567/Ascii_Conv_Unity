using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager grid_manager;
    public char active_letter;

    public int stroke_width;
    public (int row, int col) prev_grid_pos; 
    private (int row, int col) start_grid_pos;

    public enum Tools{
        pencil,
        eraser,
        line,
        rectangle,
        circle,
        text,
    }

    public enum Mods { //TODO: Implement
        none,
        fill,
        regular, //Makes shape "regular" e.g. square or circle vs. rectangle or ellipse
        regular_fill //Both
    }

    //Make a tool by: Adding to above enum, adding to draw, creating behvaior by creating a function, creating a switch function, creating an IsPressed() func
    // You should also be assigning button and keybind
    public Tools active_tool;
    public Mods active_mod;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prev_grid_pos = (-1, -1);
        active_tool = Tools.pencil;
        active_mod = Mods.none; //TODO: Make compatible w/ multiple mods
        stroke_width = 0;
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
        else if (active_tool == Tools.eraser){
            eraser(grid_pos, prev_grid_pos);
        }
        else if (active_tool == Tools.line){
            line(start_grid_pos, grid_pos);
        }
        else if (active_tool == Tools.rectangle){
            if (active_mod == Mods.none){
                rectangle(start_grid_pos, grid_pos, false);
            }
            else if (active_mod == Mods.fill){
                rectangle(start_grid_pos, grid_pos, true);
            }
        }
        else if (active_tool == Tools.circle){
            if (active_mod == Mods.none){
                circle(start_grid_pos, grid_pos, false);
            }
            else if (active_mod == Mods.fill){
                circle(start_grid_pos, grid_pos, true);
            }
        }
        if (prev_grid_pos != grid_pos){
            global.render_update = true;
        }
        prev_grid_pos = grid_pos;
    }

    public void switch_active_letter(char new_active_letter) {
        active_letter = new_active_letter;
    }

    private void pencil((int row, int col) grid_pos, (int row, int col) prev_grid_pos){
        if (grid_pos != prev_grid_pos){
            grid_manager.add_to_preview_buffer(grid_pos.row, grid_pos.col, active_letter);
        }
    }

    private void eraser ((int row, int col) grid_pos,(int row, int col) prev_grid_pos) {
        if (grid_pos != prev_grid_pos) {
            grid_manager.add_to_preview_buffer(grid_pos.row, grid_pos.col, ' ');
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
        (int row, int col) grid_pos,
        bool fill
    ){
        grid_manager.empty_preview_buffer();

        bool fill_enabled = fill;
        bool regular_enabled;

        if (!fill_enabled) {
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

        else if (fill_enabled) {
        int upper_row;
        int lower_row;

        if (start_grid_pos.row <= grid_pos.row) {
            upper_row = start_grid_pos.row;
            lower_row = grid_pos.row;
        }
        else { //hopefully no error
            upper_row = grid_pos.row;
            lower_row = start_grid_pos.row;
        }

        for (int i = upper_row; i <= lower_row; i++) {
            line(
                (i, start_grid_pos.col),
                (i, grid_pos.col),
                false
            );
        }   
        }
    }

        private void circle(
        (int row, int col) start_grid_pos,
        (int row, int col) grid_pos,
        bool fill
    ) {
        grid_manager.empty_preview_buffer();

        bool fill_enabled = fill; //These two bools should prevent part of circle from being filled if enabled halfway through render
        bool regular_enabled;

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

        if (!fill_enabled) {
            grid_manager.add_to_preview_buffer(start_grid_pos.row + row_num, start_grid_pos.col + col_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row + col_num, start_grid_pos.col + row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - col_num, start_grid_pos.col + row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - row_num, start_grid_pos.col + col_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - row_num, start_grid_pos.col - col_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row - col_num, start_grid_pos.col - row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row + col_num, start_grid_pos.col - row_num, active_letter);
            grid_manager.add_to_preview_buffer(start_grid_pos.row + row_num, start_grid_pos.col - col_num, active_letter);
        }

        else if (fill_enabled) {
            line(
            (start_grid_pos.row + row_num, start_grid_pos.col + col_num),
            (start_grid_pos.row - row_num, start_grid_pos.col + col_num),
            false);
            line(
            (start_grid_pos.row + col_num, start_grid_pos.col + row_num),
            (start_grid_pos.row - col_num, start_grid_pos.col + row_num),
            false);
            line(
            (start_grid_pos.row + row_num, start_grid_pos.col - col_num),
            (start_grid_pos.row - row_num, start_grid_pos.col - col_num),
            false);
            line(
            (start_grid_pos.row + col_num, start_grid_pos.col - row_num),
            (start_grid_pos.row - col_num, start_grid_pos.col - row_num),
            false);
        }
            
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

    private void draw_ellipse() {

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

/*         private void square_modifier(
        (int row, int col) start_grid_pos,
        (int row, int col) grid_pos
    ) {
        int row_dif = grid_pos.row - start_grid_pos.row;
        int col_dif = grid_pos.col - start_grid_pos.col;
        int smaller_dif;

        if (row_dif <= col_dif) {
            smaller_dif = row_dif;
        }
        else {
            smaller_dif = col_dif;
        }

        if (active_tool == Tools.rectangle) {
            rectangle((start_grid_pos.row, start_grid_pos.col),
            (start_grid_pos.row + smaller_dif, start_grid_pos.col + smaller_dif));
        }

        else if (active_tool == Tools.filled_rectangle) {
            filled_rectangle((start_grid_pos.row, start_grid_pos.col),
            (start_grid_pos.row + smaller_dif, start_grid_pos.col + smaller_dif));
        }

    } */

}
