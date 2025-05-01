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
                double theta = Math.Atan2(col_dif, row_dif) * (180d / Math.PI);
                //Clockwise: Down TO Up: 0 to 180,
                //Clockwise: Next to Up TO Next to Down: -179.9999 to -0.0001
                int big_dif;
                if (Math.Abs(row_dif) <= Math.Abs(col_dif)){ //Determine bigger component
                    big_dif = col_dif;
                }
                else{
                    big_dif = row_dif;
                }

                //8 cases because of 8 octants. Range is 45 degrees (size of octant) with offset of 22.5 degrees
                if (theta >= -22.5 && theta < 22.5){ //Down
                    endGridPos.col = startGpos.col; //Row stays same, Col gets straightened out
                }
                else if (theta >= 22.5 && theta < 67.5){ //Bottom Right
                    endGridPos.row = startGpos.row + big_dif;
                    endGridPos.col = startGpos.col + big_dif;
                }
                else if (theta >= 67.5 && theta < 112.5){ //Right
                    endGridPos.row = startGpos.row; //Col stays same, Row gets straightened out
                }
                else if (theta >= 112.5 && theta < 157.5){ //Top Right
                    endGridPos.row = startGpos.row - big_dif;
                    endGridPos.col = startGpos.col + big_dif;
                }
                else if ((theta >= 157.5 && theta <= 180) || (theta > -180 && theta < -157.5)){ //Up
                    endGridPos.col = startGpos.col; //Row stays same, Col gets straightened out
                }
                else if (theta >= -157.5 && theta < -112.5){ //Top Left
                    endGridPos.row = startGpos.row - big_dif;
                    endGridPos.col = startGpos.col - big_dif;
                }
                else if (theta >= -112.5 && theta < -67.5){ //Left
                    endGridPos.row = startGpos.row; //Col stays same, Row gets straightened out
                }
                else if (theta >= -67.5 && theta < -22.5){ //Bottom Left
                    endGridPos.row = startGpos.row + big_dif;
                    endGridPos.col = startGpos.col - big_dif;
                }
            }
            if(Toolbox.getStrokeWidth() != 1){
                Debug.Log($"Stroke Width: {Toolbox.getStrokeWidth()}");
                for (int i = -Toolbox.getStrokeWidth(); i <= Toolbox.getStrokeWidth(); i++){
                    line((startGpos.row + i, startGpos.col), (endGridPos.row + i, endGridPos.col), true);
                }
            }
            else{
                line((startGpos.row, startGpos.col), (endGridPos.row, endGridPos.col), true);
            }
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
        float extraInterval = (float)shortSlope / (float)(extra);
        float extraCounter = 1;

        for (int x = 0; x < shortSlope; x++){
            int thisChunk = perChunk;
            if (extraNeeded()){
                thisChunk += 1;
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
            extraCounter += 1;
        }

        bool extraNeeded(){
            if(extra <= 0){
                return false;
            } 
            if(extraCounter >= extraInterval){
                extra -= 1;
                extraCounter = extraCounter - extraInterval;
                return true;
            }
            return false;
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
