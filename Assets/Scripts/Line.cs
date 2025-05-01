using UnityEngine;
using System;

public class Line : Tool
{
    private (int row, int col) regularify(int rowDif, int colDif){
        double theta = Math.Atan2(colDif, rowDif) * (180d / Math.PI);
        //Clockwise: Down TO Up: 0 to 180,
        //Clockwise: Next to Up TO Next to Down: -179.9999 to -0.0001
        int newRow = 0;
        int newCol = 0;
        int bigDif;
        if (Math.Abs(rowDif) <= Math.Abs(colDif)){ //Determine bigger component
            bigDif = colDif;
        }
        else{
            bigDif = rowDif;
        }

        //8 cases because of 8 octants. Range is 45 degrees (size of octant) with offset of 22.5 degrees
        if (theta >= -22.5 && theta < 22.5){ //Down
            newCol = startGpos.col; //Row stays same, Col gets straightened out
        }
        else if (theta >= 22.5 && theta < 67.5){ //Bottom Right
            newRow = startGpos.row + bigDif;
            newCol = startGpos.col + bigDif;
        }
        else if (theta >= 67.5 && theta < 112.5){ //Right
            newRow = startGpos.row; //Col stays same, Row gets straightened out
        }
        else if (theta >= 112.5 && theta < 157.5){ //Top Right
            newRow = startGpos.row - bigDif;
            newCol = startGpos.col + bigDif;
        }
        else if ((theta >= 157.5 && theta <= 180) || (theta > -180 && theta < -157.5)){ //Up
            newCol = startGpos.col; //Row stays same, Col gets straightened out
        }
        else if (theta >= -157.5 && theta < -112.5){ //Top Left
            newRow = startGpos.row - bigDif;
            newCol = startGpos.col - bigDif;
        }
        else if (theta >= -112.5 && theta < -67.5){ //Left
            newRow = startGpos.row; //Col stays same, Row gets straightened out
        }
        else if (theta >= -67.5 && theta < -22.5){ //Bottom Left
            newRow = startGpos.row + bigDif;
            newCol = startGpos.col - bigDif;
        }

        return (newRow, newCol);
    }
    public override void draw(){

        (int row, int col) endGridPos = gridManager.getGridPos();
        int rowDif = endGridPos.row - startGpos.row;
        int colDif = endGridPos.col - startGpos.col;

        if(globalOperations.controls.Grid.RegularToggle.IsPressed()) { //Regular (straight) line

            if (Math.Abs(rowDif) != Math.Abs(colDif)) { //If not already a regular, then make regular
                endGridPos = regularify(rowDif, colDif); //Updates endpoints to make a regular line
            }
        }

        gridManager.emptyPreviewBuffer(); //Clears the previously drawn lines
        double theta = Math.Atan2(colDif, rowDif) * (180d / Math.PI);
        if ((theta >= -45 && theta < 45) || (theta >= 135 && theta <= 180) || (theta > -180 && theta < -135)){ //Down or Up

            for (int i = -Toolbox.getStrokeWidth() + 1; i <= Toolbox.getStrokeWidth() - 1; i++){

                line((startGpos.row, startGpos.col + i), (endGridPos.row, endGridPos.col + i), false); //Draws regular line or stroke Width line
            }
        }
        else if((theta >= -135 && theta < -45) || (theta >= 45 && theta < 135)){ //Left or Right
            for (int i = -Toolbox.getStrokeWidth() + 1; i <= Toolbox.getStrokeWidth() - 1; i++){

                line((startGpos.row + i, startGpos.col), (endGridPos.row + i, endGridPos.col), false); //Draws regular line or stroke Width line
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
