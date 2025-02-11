using UnityEngine;
using System;

public class Circle : Tool
{

    [SerializeField] Line Line;
    private bool isFilled = false;

    public override void draw(){
        (int row, int col) gpos = gridManager.getGridPos();

        gridManager.emptyPreviewBuffer();
        
        int rowDif = gpos.row - startGpos.row; //row and col components of
        int colDif = gpos.col - startGpos.col; //difference between start and end
        float diagonalR = (float)Math.Sqrt((rowDif * rowDif) + (colDif * colDif));
        //pythag: c = sqrt(a^2 + b^2)
        //this is not yet usable due to the geometry of a grid in non-cardinal cases
        //Note about precision: if not good enough, make these floats into doubles
        int r;
        if (rowDif != 0 && colDif != 0) { //non-cardinal case AKA trig time
            int o = Math.Abs(colDif); //converts o to be positive to work with sin()
            float angleTheta = (float)Math.Asin(o / diagonalR);
            float h = (float)(o / diagonalR) / (float)Math.Sin(angleTheta); //hypotenuse
            float r0 = diagonalR / h; //radius in terms of pixels
            r = (int)Math.Floor(r0); //floor makes our radius usable
        }
        else if (rowDif == 0 || colDif == 0) {
            r = (int)Math.Floor(diagonalR);
        }
        else {
            r = 0;
        }
        int rowNum = 0;
        int colNum = r;
        int p = 1 - r;

        while (rowNum <= colNum) { // draws 8 sections "simulataneously"
            if (!isFilled) {
                gridManager.addToPreviewBuffer(startGpos.row + rowNum, startGpos.col + colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row + colNum, startGpos.col + rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row - colNum, startGpos.col + rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row - rowNum, startGpos.col + colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row - rowNum, startGpos.col - colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row - colNum, startGpos.col - rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row + colNum, startGpos.col - rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(startGpos.row + rowNum, startGpos.col - colNum, globalOperations.activeLetter);
            }
            else if (isFilled) {
                Line.line(
                (startGpos.row + rowNum, startGpos.col + colNum),
                (startGpos.row - rowNum, startGpos.col + colNum),
                false);
                Line.line(
                (startGpos.row + colNum, startGpos.col + rowNum),
                (startGpos.row - colNum, startGpos.col + rowNum),
                false);
                Line.line(
                (startGpos.row + rowNum, startGpos.col - colNum),
                (startGpos.row - rowNum, startGpos.col - colNum),
                false);
                Line.line(
                (startGpos.row + colNum, startGpos.col - rowNum),
                (startGpos.row - colNum, startGpos.col - rowNum),
                false);
            }
            rowNum += 1;
            if (p < 0) {
                p += 2 * rowNum + 1;
            }
            else {
                colNum -= 1;
                p += 2 * (rowNum - colNum) + 1;
            }
        }
    }

    public override void handleInput()
    {
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered){
            isFilled = !isFilled;
        }
    }

}
