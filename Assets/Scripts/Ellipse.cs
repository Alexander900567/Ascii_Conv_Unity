using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;

public class Ellipse : Tool
{
    [SerializeField] private Line Line;
    private bool isFilled = false;
    private (int row, int col) beginGpos;
    private Action<int, int> drawQuadPixels;
    private Action<int, int> drawLinePairs;

    private void setBeginGpos((int row, int col) newBeginGpos){
        beginGpos.row = newBeginGpos.row;
        beginGpos.col = newBeginGpos.col;
    }

    private (int row, int col) getBeginGpos(){
        return beginGpos;
    }

    private List<(int, int, char)> previewQueue = new List<(int, int, char)>(); //row, col, input
    private void addToPreviewQueue(int row, int col, char input){
        previewQueue.Add((row, col, input));
    }
    private void flushPreviewQueue(List<(int, int, char)> queue){
        foreach ((int, int, char) item in queue){
            if (item.Item3 != ' ') {
                gridManager.addToPreviewBuffer(item.Item1, item.Item2, item.Item3);
            }
            previewQueue.Clear();
        }
    }
    private void drawCircle(int r){
            (int row, int col) beginGposLocal = getBeginGpos();

            int rowNum = 0;
            int colNum = r;
            int p = 1 - r;

            while (rowNum <= colNum) { // draws 8 sections "simulataneously"
                if (!isFilled) {
                    addToPreviewQueue(beginGposLocal.row + rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row + colNum, beginGposLocal.col + rowNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row - colNum, beginGposLocal.col + rowNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row - rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row - rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row - colNum, beginGposLocal.col - rowNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row + colNum, beginGposLocal.col - rowNum, globalOperations.activeLetter);
                    addToPreviewQueue(beginGposLocal.row + rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
                }
                else if (isFilled) { //much like the filled ellipse, we use lines to fill within the circle
                    Line.line(
                    (beginGposLocal.row + rowNum, beginGposLocal.col + colNum),
                    (beginGposLocal.row - rowNum, beginGposLocal.col + colNum),
                    false);
                    Line.line(
                    (beginGposLocal.row + colNum, beginGposLocal.col + rowNum),
                    (beginGposLocal.row - colNum, beginGposLocal.col + rowNum),
                    false);
                    Line.line(
                    (beginGposLocal.row + rowNum, beginGposLocal.col - colNum),
                    (beginGposLocal.row - rowNum, beginGposLocal.col - colNum),
                    false);
                    Line.line(
                    (beginGposLocal.row + colNum, beginGposLocal.col - rowNum),
                    (beginGposLocal.row - colNum, beginGposLocal.col - rowNum),
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
            flushPreviewQueue(previewQueue);
        }

    private void Awake(){ 
        if (gridManager == null || globalOperations == null || Line == null){ //this may be able to go away
            Debug.LogError("Ellipse is missing gridManager, globalOperations, or Line");
            return;
        }

        drawQuadPixels = (rowNum, colNum) => { //Called if ellipse is not filled
            (int row, int col) beginGposLocal = getBeginGpos();
            gridManager.addToPreviewBuffer(beginGposLocal.row + rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter); //Draws 4 quadrants "simultaneously"
            gridManager.addToPreviewBuffer(beginGposLocal.row - rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter); //i.e. the perimeter
            gridManager.addToPreviewBuffer(beginGposLocal.row + rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
            gridManager.addToPreviewBuffer(beginGposLocal.row - rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
        };
        drawLinePairs = (rowNum, colNum) => { //Called if ellipse is filled
            (int row, int col) beginGposLocal = getBeginGpos();
            Line.line( //Draws lines to make the ellipse filled
                (beginGposLocal.row - rowNum, 
                beginGposLocal.col + colNum),
                (beginGposLocal.row + rowNum,
                beginGposLocal.col + colNum),
                false);
            Line.line(
                (beginGposLocal.row - rowNum, 
                beginGposLocal.col - colNum),
                (beginGposLocal.row + rowNum,
                beginGposLocal.col - colNum),
                false);
        };
    }
    private void ellipseLogic((int row, int col) beginGpos, (int row, int col) gpos){ //Controls logic to draw circle, ellipse, filled or not filled

        int rowDif = gpos.row - beginGpos.row; //row and col components of
        int colDif = gpos.col - beginGpos.col; //difference between start and end

        if (globalOperations.controls.Grid.RegularToggle.IsPressed() || //if user wants circle
        (!globalOperations.controls.Grid.RegularToggle.IsPressed() && rowDif == colDif)){ //Math to make a circle
            float diagonalR = (float)Math.Sqrt((rowDif * rowDif) + (colDif * colDif));
            //pythag: c = sqrt(a^2 + b^2)
            //this is not yet usable due to the geometry of a grid in non-cardinal cases
            //Note about precision: if not good enough, make these floats into doubles
            int r;
            if (rowDif != 0 && colDif != 0) { //non-cardinal case AKA trig time
                if (globalOperations.controls.Grid.RegularToggle.IsPressed()){
                int o = Math.Abs(colDif); //converts o to be positive to work with sin()
                float angleTheta = (float)Math.Asin(o / diagonalR);
                float h = (float)(o / diagonalR) / (float)Math.Sin(angleTheta); //hypotenuse
                float r0 = diagonalR / h; //radius in terms of pixels
                r = (int)Math.Floor(r0); //floor makes our radius usable
                }
                else if (!globalOperations.controls.Grid.RegularToggle.IsPressed()){
                    r = Math.Abs(colDif); //since they are the same, it could be rowDif as well, abs() makes it positive
                }
                else{
                    r = 0;
                }
            }
            else if (rowDif == 0 || colDif == 0) { //cardinal cases
                r = (int)Math.Floor(diagonalR);
            }
            else {
                r = 0;
            }
            if (Toolbox.GetStrokeWidth() == 1){ //If no stroke
                drawCircle(r); //Regular circle
            }
            else{ //If need stroke
                bool temp = isFilled;
                isFilled = true;
                drawCircle(r); //Outer Circle
                char lastActiveLetter = globalOperations.activeLetter;
                globalOperations.activeLetter = ' '; //Now draw with spaces
                int innerR = Math.Max(1, r - Toolbox.GetStrokeWidth()); //At least 1
                drawCircle(innerR); //Inner Circle
                globalOperations.activeLetter = lastActiveLetter; //Restore activeLetter
                isFilled = temp;
            }
        }
        else if(!globalOperations.controls.Grid.RegularToggle.IsPressed()){ //Math to make an ellipse
            if (rowDif == 0 || colDif == 0){ //Line optimization i.e. draw line instead of flat ellipse
                if (rowDif == 0){
                    Line.line( //Line with length equal to flat ellipse
                    (beginGpos.row, beginGpos.col - (gpos.col - beginGpos.col)),
                    (gpos.row, gpos.col),
                    true
                    );
                    return;
                }
                else if (colDif == 0){
                    Line.line( //Line with length equal to flat ellipse
                    (beginGpos.row - (gpos.row - beginGpos.row), beginGpos.col),
                    (gpos.row, gpos.col),
                    true
                    );
                    return;                    
                }
            }
            if (Toolbox.GetStrokeWidth() != 1){ //If need stroke
                for (int i = 0; i <= Toolbox.GetStrokeWidth() - 1; i++) { //will run once with no offset if strokeWidth = 1,
                    //twice but once normal and once with offset if width = 2, etc.
                    if (i % 2 == 0){ //In even cases of strokeWidth, it goes in
                        drawEllipse(drawQuadPixels, Math.Abs(rowDif - i), Math.Abs(colDif - i));
                    }
                    else if (i % 2 != 0){ //In odd, it goes out
                        drawEllipse(drawQuadPixels, Math.Abs(rowDif + i), Math.Abs(colDif + i));
                    }
                }
                return;
            }
            if (!isFilled){
                drawEllipse(drawQuadPixels, Math.Abs(rowDif), Math.Abs(colDif)); //Non-filled ellipse
            }
            else if (isFilled){
                drawEllipse(drawLinePairs, Math.Abs(rowDif), Math.Abs(colDif)); //Filled ellipse
            }
        }
    }
    private void drawEllipse(Action <int, int> renderFunc, int rowDif, int colDif) {

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
    public override void draw(){
        gridManager.emptyPreviewBuffer();
        setBeginGpos(startGpos);
        (int row, int col) gpos = gridManager.getGridPos();

        ellipseLogic(beginGpos, gpos);
    }
    public override void handleInput(){
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered){
            isFilled = !isFilled;
        }    
        else if(
            globalOperations.controls.Grid.RegularToggle.triggered ||
            globalOperations.controls.Grid.RegularToggle.WasReleasedThisFrame()
        ){
            globalOperations.renderUpdate = true;
        }
    }
}