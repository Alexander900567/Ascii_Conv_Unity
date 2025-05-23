using UnityEngine;
using System;

public class Rectangle : StrokeTool
{
    [SerializeField] private Line Line;
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
        int upperRow;
        int lowerRow;

            if (beginGpos.row <= gpos.row) {
                upperRow = beginGpos.row;
                lowerRow = gpos.row;
            }
            else { //hopefully no error
                upperRow = gpos.row;
                lowerRow = beginGpos.row;
            }

            for (int i = upperRow; i <= lowerRow; i++) {
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
            if (!isFilled) { //Usually goes here
                int rowGrowth;
                if (gpos.row >= beginGpos.row){
                    rowGrowth = 1;
                } 
                else { rowGrowth = -1; }

                int colGrowth;
                if (gpos.col >= beginGpos.col){
                    colGrowth = 1;
                } 
                else { colGrowth = -1; }

                DrawRectangle(beginGpos, gpos);

                for (int i = 1; i <= getStrokeWidth() - 1; i++) { //will run once with no offset if strokeWidth = 1
                //twice but once normal and once with offset if width = 2, etc.
                    beginGpos.row -= rowGrowth;
                    beginGpos.col -= colGrowth;
                    gpos.row += rowGrowth;
                    gpos.col += colGrowth;

                    DrawRectangle(beginGpos, gpos);
                }
            }
            else { DrawRectangle(beginGpos, gpos); } //Does this to avoid stroke for filled rects.
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
