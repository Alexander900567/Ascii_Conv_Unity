using UnityEngine;
using System;

public class Rectangle : Tool
{
    [SerializeField] private Line Line;
    [SerializeField] private Brush Brush;
    private bool isFilled = false;
    private void DrawRectangle((int row, int col) beginGpos, (int row, int col) gpos) {
        if (globalOperations.controls.Grid.RegularToggle.IsPressed()){ //Checks if user wants a square
            int row_dif = gpos.row - beginGpos.row; //For math to make rectangle a square
            int col_dif = gpos.col - beginGpos.col;

            if (Math.Abs(row_dif) != Math.Abs(col_dif)) { //If not already a square, then make square
                int smaller_dif;
                if (Math.Abs(row_dif) < Math.Abs(col_dif)){ //Determine shorter side
                    smaller_dif = row_dif;
                }
                else{
                    smaller_dif = col_dif;
                }
                if (smaller_dif == 0){ //Cardinal lines are not drawn
                    return;
                }
                smaller_dif = Math.Abs(smaller_dif); //The logic can be optimized I bet
                if (col_dif > 0 && row_dif < 0){
                    gpos.row = beginGpos.row - smaller_dif;
                    gpos.col = beginGpos.col + smaller_dif;
                }
                else if (col_dif > 0 && row_dif > 0){ //Conforms longer side to be equal to smaller side
                    gpos.row = beginGpos.row + smaller_dif;
                    gpos.col = beginGpos.col + smaller_dif;
                }
                else if (col_dif < 0 && row_dif > 0){
                    gpos.row = beginGpos.row + smaller_dif;
                    gpos.col = beginGpos.col - smaller_dif;
                }
                else if (col_dif < 0 && row_dif < 0){
                    gpos.row = beginGpos.row - smaller_dif;
                    gpos.col = beginGpos.col - smaller_dif;
                }
            }
        }

        if (!isFilled) {
            Line.line(
            (beginGpos.row, beginGpos.col),
            (beginGpos.row, gpos.col),
            false
            );
            Line.line(
                (beginGpos.row, beginGpos.col),
                (gpos.row, beginGpos.col),
                false
            );
            Line.line(
                (gpos.row, beginGpos.col),
                (gpos.row, gpos.col),
                false
            );
            Line.line(
                (beginGpos.row, gpos.col),
                (gpos.row, gpos.col),
                false
            );
        }
        else if (isFilled) {
        int upper_row;
        int lower_row;

            if (beginGpos.row <= gpos.row) {
                upper_row = beginGpos.row;
                lower_row = gpos.row;
            }
            else { //hopefully no error
                upper_row = gpos.row;
                lower_row = beginGpos.row;
            }

            for (int i = upper_row; i <= lower_row; i++) {
                Line.line(
                    (i, beginGpos.col),
                    (i, gpos.col),
                    false
                );
            }   
        }        
    }
    public override void draw() //control for drawing rectangles with any strokeWidth
    {
        gridManager.emptyPreviewBuffer();
        (int row, int col) beginGpos = startGpos; //initialize both corners
        (int row, int col) gpos = gridManager.getGridPos();

        for (int i = 0; i <= Toolbox.GetStrokeWidth() - 1; i++) { //will run once with no offset if strokeWidth = 1, twice but once normal and once with offset if width = 2, etc.
            if (i % 2 == 0){ //In even cases of strokeWidth, it goes in
            beginGpos.col += i;
            beginGpos.row += i;
            gpos.col -= i;
            gpos.row -= i;
            }
            else if (i % 2 != 0){ //In odd, it goes out
            beginGpos.col -= i;
            beginGpos.row -= i;
            gpos.col += i;
            gpos.row += i;
            }
            else {
                Debug.Log("Error: Invalid strokeWidth. How did you do that?");
            }
            DrawRectangle(beginGpos, gpos);
        }
    }

    public override void handleInput(){
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered){
            isFilled = !isFilled;
            globalOperations.renderUpdate = true;
        }
        else if(
            globalOperations.controls.Grid.RegularToggle.triggered ||
            globalOperations.controls.Grid.RegularToggle.WasReleasedThisFrame()
        ){
            globalOperations.renderUpdate = true;
        }
    }

}
