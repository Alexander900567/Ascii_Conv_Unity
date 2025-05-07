using UnityEngine;
using System;

public class Line : StrokeTool
{
    private (int row, int col) regularify(int rowDif, int colDif, (int row, int col) endGridPos){
        double theta = Math.Atan2(colDif, rowDif) * (180d / Math.PI);
        /*
                 180
            -135     135
        -90              90  
            -45      45
                  0
        */
        int newRow = endGridPos.row;
        int newCol = endGridPos.col;

        if (theta <= 45 && theta >= -45){ //Down
            newCol = startGpos.col;
        } 
        else if (theta <= -45 && theta >= -135){ //Left
            newRow = startGpos.row;
        }
        else if (theta <= 135 && theta >= 45){ // Right
            newRow = startGpos.row;
        }
        else if (theta <= -135 || theta >= 135){ //Up
            newCol = startGpos.col;
        }

        return (newRow, newCol);
    }
    public override void draw(){

        (int row, int col) endGridPos = gridManager.getGridPos();
        int rowDif = endGridPos.row - startGpos.row;
        int colDif = endGridPos.col - startGpos.col;

        if(globalOperations.controls.Grid.RegularToggle.IsPressed()) { //Regular (straight) line
            endGridPos = regularify(rowDif, colDif, endGridPos); //Updates endpoints to make a regular line
        }

        gridManager.emptyPreviewBuffer(); //Clears the previously drawn lines
        double theta = Math.Atan2(colDif, rowDif) * (180d / Math.PI);
        if ((theta >= -45 && theta < 45) || (theta >= 135 && theta <= 180) || (theta > -180 && theta < -135)){ //Down or Up

            for (int i = -getStrokeWidth() + 1; i <= getStrokeWidth() - 1; i++){

                line((startGpos.row, startGpos.col + i), (endGridPos.row, endGridPos.col + i), false); //Draws regular line or stroke Width line
            }
        }
        else if((theta >= -135 && theta < -45) || (theta >= 45 && theta < 135)){ //Left or Right
            for (int i = -getStrokeWidth() + 1; i <= getStrokeWidth() - 1; i++){

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
    public override void onEnter()
    {
        showStrokeWidthSlider();
    }
    public override void onExit()
    {
        hideStrokeWidthSlider();
    }
}
