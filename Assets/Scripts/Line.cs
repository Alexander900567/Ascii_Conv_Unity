using UnityEngine;
using System;

public class Line : Tool
{
    public override void draw(){
        if(!globalOperations.controls.Grid.RegularToggle.IsPressed()){ //Noncardinal line
            line(startGpos, gridManager.getGridPos(), true);
        }
        else { //Regular (straight) line
            (int row, int col) endGridPos = gridManager.getGridPos();

            int row_dif = endGridPos.row - startGpos.row;
            int col_dif = endGridPos.col - startGpos.col;

            if (Math.Abs(row_dif) != Math.Abs(col_dif)) { //If not already a regular, then make regular
                int smaller_dif;
                if (Math.Abs(row_dif) < Math.Abs(col_dif)){ //Determine shorter component
                    smaller_dif = row_dif;
                }
                else{
                    smaller_dif = col_dif;
                }
                smaller_dif = Math.Abs(smaller_dif); //The math can be optimized I bet
                if (col_dif > 0 && row_dif < 0){
                    endGridPos.row = startGpos.row - smaller_dif;
                    endGridPos.col = startGpos.col + smaller_dif;
                }
                else if (col_dif > 0 && row_dif > 0){ //Conforms longer side to be equal to smaller side
                    endGridPos.row = startGpos.row + smaller_dif;
                    endGridPos.col = startGpos.col + smaller_dif;
                }
                else if (col_dif < 0 && row_dif > 0){
                    endGridPos.row = startGpos.row + smaller_dif;
                    endGridPos.col = startGpos.col - smaller_dif;
                }
                else if (col_dif < 0 && row_dif < 0){
                    endGridPos.row = startGpos.row - smaller_dif;
                    endGridPos.col = startGpos.col - smaller_dif;
                }
            }
            line((startGpos.row, startGpos.col), (endGridPos.row, endGridPos.col), true);
        }
    }

    public void line(
        (int row, int col) startGpos,
        (int row, int col) gridPos,
        bool clearBuffer
    ){
        
        if (clearBuffer){
            gridManager.emptyPreviewBuffer();
        }

        int horizontalSlope = gridPos.row - startGpos.row;
        int verticalSlope = gridPos.col - startGpos.col;
        int rowIter = 0;
        int colIter = 0;

        if (horizontalSlope != 0){
            rowIter = horizontalSlope / System.Math.Abs(horizontalSlope);
        }
        if (verticalSlope != 0){
            colIter = verticalSlope / System.Math.Abs(verticalSlope);
        }

        horizontalSlope = System.Math.Abs(horizontalSlope);
        verticalSlope = System.Math.Abs(verticalSlope);

        int longSlope;
        int shortSlope;
        bool rowLengthIsLong;

        if (horizontalSlope > verticalSlope){
            longSlope = horizontalSlope;
            shortSlope = verticalSlope + 1;
            rowLengthIsLong = true;
        }        
        else{
            longSlope = verticalSlope;
            shortSlope = horizontalSlope + 1;
            rowLengthIsLong = false;
        }

        int perChunk = longSlope / shortSlope;
        int extra = (longSlope % shortSlope) + 1;

        for (int x = 0; x < shortSlope; x++){
            int thisChunk = perChunk;
            if (extra > 0){
                thisChunk += 1;
                extra -= 1;
            }
            for (int y = 0; y < thisChunk; y++){
                gridManager.addToPreviewBuffer(startGpos.row, startGpos.col, globalOperations.activeLetter);
                if (rowLengthIsLong){
                    startGpos.row += rowIter;
                }
                else {
                    startGpos.col += colIter;
                }
            }
            if (!rowLengthIsLong){
                startGpos.row += rowIter;
            }
            else {
                startGpos.col += colIter;
            }
        }
    }

    public override void handleInput(){
        base.handleInput();
        if (
            globalOperations.controls.Grid.RegularToggle.triggered ||
            globalOperations.controls.Grid.RegularToggle.WasReleasedThisFrame()
        ){
            globalOperations.renderUpdate = true;
        }
    }
}
