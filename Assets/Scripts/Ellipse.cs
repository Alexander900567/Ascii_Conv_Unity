using UnityEngine;
using System;
using Unity.VisualScripting;

public class Ellipse : Tool
{
    [SerializeField] private Line Line;
    private bool isFilled = false;
    private bool isRegular = false;
    private Action<int, int> drawQuadPixels;
    private Action<int, int> drawLinePairs;

    private void Awake(){
        if (gridManager == null || globalOperations == null || Line == null){
            Debug.LogError("Ellipse is missing gridManager, globalOperations, or Line");
            return;
        }

        drawQuadPixels = (rowNum, colNum) => {
            (int row, int col) gpos = gridManager.getGridPos();
            gridManager.addToPreviewBuffer(startGpos.row + rowNum, startGpos.col + colNum, globalOperations.activeLetter);
            gridManager.addToPreviewBuffer(startGpos.row - rowNum, startGpos.col + colNum, globalOperations.activeLetter);
            gridManager.addToPreviewBuffer(startGpos.row + rowNum, startGpos.col - colNum, globalOperations.activeLetter);
            gridManager.addToPreviewBuffer(startGpos.row - rowNum, startGpos.col - colNum, globalOperations.activeLetter);
        };
        drawLinePairs = (rowNum, colNum) => {
            (int row, int col) gpos = gridManager.getGridPos();
            Line.line(
                (startGpos.row - rowNum, 
                startGpos.col + colNum),
                (startGpos.row + rowNum,
                startGpos.col + colNum),
                false);
            Line.line(
                (startGpos.row - rowNum, 
                startGpos.col - colNum),
                (startGpos.row + rowNum,
                startGpos.col - colNum),
                false);
        };
    }
    public override void draw(){
        (int row, int col) gpos = gridManager.getGridPos();

        gridManager.emptyPreviewBuffer();

        int rowDif = gpos.row - startGpos.row; //row and col components of
        int colDif = gpos.col - startGpos.col; //difference between start and end

        if (isRegular || (!isRegular && rowDif == colDif)){ //Math to make a circle
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
        else if(!isRegular){ //Math to make an ellipse
            if (rowDif == 0 || colDif == 0){ //Line optimization
                if (rowDif == 0){
                    Line.line( //Line with length equal to flat ellipse
                    (startGpos.row, startGpos.col - (gpos.col - startGpos.col)),
                    (gpos.row, gpos.col),
                    true
                    );
                    return;
                }
                else if (colDif == 0){
                    Line.line( //Line with length equal to flat ellipse
                    (startGpos.row - (gpos.row - startGpos.row), startGpos.col),
                    (gpos.row, gpos.col),
                    true
                    );
                    return;                    
                }
            }
            if (!isFilled){
                drawEllipse(drawQuadPixels, Mathf.Abs(rowDif), Mathf.Abs(colDif)); //Non fill ellipse
            }
            else if (isFilled){
                drawEllipse(drawLinePairs, Mathf.Abs(rowDif), Mathf.Abs(colDif)); //Filled ellipse
            }
        }
    }
    private void drawEllipse(Action <int, int> renderFunc, int rowDif, int colDif) {
        (int row, int col) gpos = gridManager.getGridPos();

        int rowNum = 0; //Used to keep track of drawing math
        int colNum = colDif;

        float rowDifSquared = rowDif * rowDif; //Used for math later :)
        float colDifSquared = colDif * colDif;

        float pRow = 0; //More drawing math
        float pCol = 2 * rowDifSquared * colNum;

        renderFunc(rowNum, colNum);

        float p = colDifSquared - (rowDifSquared * colDif) + (0.25f * rowDifSquared);

        while (pRow <= pCol){ //Top and bottom
            rowNum += 1;
            pRow += 2.0f * colDifSquared;
            if (p < 0.0f){
                p += colDifSquared + pRow;
            }
            else{
                colNum -= 1;
                pCol += -2.0f * rowDifSquared;
                p += colDifSquared + pRow - pCol;
            }
            renderFunc(rowNum, colNum);
        }
        //Left and right
        p = (colDifSquared * ((float)(rowNum + 0.5) * (float)(rowNum + 0.5))) +
        (rowDifSquared * (colNum - 1) * (colNum - 1)) -
        (rowDifSquared * colDifSquared);

        while (colNum >= 0){
            colNum -= 1;
            pCol += -2.0f * rowDifSquared;
            if (p > 0.0f){
                p += rowDifSquared - pCol;
            }
            else{
                rowNum += 1;
                pRow += 2.0f * colDifSquared;
                p += rowDifSquared - pCol + pRow;
            }
            renderFunc(rowNum, colNum);
        }
    }    
    public override void handleInput()
    {
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered){
            isFilled = !isFilled;
        }
        if (globalOperations.controls.Grid.RegularToggle.triggered){
            isRegular = !isRegular;
        }
    }
}
