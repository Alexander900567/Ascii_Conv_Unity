using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;

public class Ellipse : StrokeTool
{
    [SerializeField] private Line Line;
    private bool isFilled = false;
    private bool offset;
    private (int row, int col) beginGpos;
    private Action<int, int> drawQuadPixels;
    private Action<int, int> drawLinePairs;
    private List<(int, int, char)> previewQueue = new List<(int, int, char)>(); //(row, col, input)
    private void setBeginGpos((int row, int col) newBeginGpos){
        beginGpos.row = newBeginGpos.row;
        beginGpos.col = newBeginGpos.col;
    }
    private (int row, int col) getBeginGpos(){
        return beginGpos;
    }
    private void previewQueueLine((int row, int col) beginGposLocal, (int row, int col) gridPos){
        int colIter = beginGposLocal.col <= gridPos.col ? 1 : -1;
        int rowIter = beginGposLocal.row <= gridPos.row ? 1 : -1;

        for (int i = beginGposLocal.col; i != gridPos.col + colIter; i += colIter){
            for (int j = beginGposLocal.row; j != gridPos.row + rowIter; j += rowIter){
                previewQueue.Add((j, i, globalOperations.activeLetter));
            }
        }
    }
    private void flushPreviewQueue(List<(int, int, char)> queue) {
        HashSet<(int, int)> conflictingPositions = new HashSet<(int, int)>(); //Finds difference of two circles
        foreach ((int row, int col, char input) in queue) {
            if (input == ' ') {
                conflictingPositions.Add((row, col)); //Stores the void here
            }
        }

        foreach ((int row, int col, char input) in queue) {
            if (input != ' ' && !conflictingPositions.Contains((row, col))) {
                gridManager.addToPreviewBuffer(row, col, input);
            }
        }
        conflictingPositions.Clear();
        previewQueue.Clear();
    }

    public void drawCircle((int row, int col) beginGposLocal, int r, bool forceFill = false, bool strokeCircle = false){

        int rowNum = 0;
        int colNum = r;
        int p = 1 - r;

        while (rowNum <= colNum) { // draws 8 sections "simulataneously"
            if (strokeCircle){
                previewQueueLine(
                (beginGposLocal.row + rowNum, beginGposLocal.col + colNum),
                (beginGposLocal.row - rowNum, beginGposLocal.col + colNum));
                previewQueueLine(
                (beginGposLocal.row + colNum, beginGposLocal.col + rowNum),
                (beginGposLocal.row - colNum, beginGposLocal.col + rowNum));
                previewQueueLine(
                (beginGposLocal.row + rowNum, beginGposLocal.col - colNum),
                (beginGposLocal.row - rowNum, beginGposLocal.col - colNum));
                previewQueueLine(
                (beginGposLocal.row + colNum, beginGposLocal.col - rowNum),
                (beginGposLocal.row - colNum, beginGposLocal.col - rowNum));
            }
            else if (!isFilled && !forceFill){
                gridManager.addToPreviewBuffer(beginGposLocal.row + rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row + colNum, beginGposLocal.col + rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - colNum, beginGposLocal.col + rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - colNum, beginGposLocal.col - rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row + colNum, beginGposLocal.col - rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row + rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
            }
            else if (isFilled || forceFill){ //much like the filled ellipse, we use lines to fill within the circle
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
    private void drawEllipse(Action <int, int> renderFunc, int rowDif, int colDif){

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
    private void ellipseLogic((int row, int col) beginGpos, (int row, int col) gpos){ //Controls logic to draw circle, ellipse, filled or not filled

        int rowDif = gpos.row - beginGpos.row; //row and col components of
        int colDif = gpos.col - beginGpos.col; //difference between start and end

        if (globalOperations.controls.Grid.RegularToggle.IsPressed() || //When drawing a circle
        (!globalOperations.controls.Grid.RegularToggle.IsPressed() && rowDif == colDif)){ //Math to make a circle
            int r; //Calculates size of circle user wants
            if (rowDif != 0 && colDif != 0) { //Non-cardinal cases (diagonals)
                if (globalOperations.controls.Grid.RegularToggle.IsPressed()){ //When user explicitly asks for a circle
                    r = (int)Math.Floor(gridManager.pythagLength(rowDif, colDif));
                }
                else if (!globalOperations.controls.Grid.RegularToggle.IsPressed()){ //When optimizing for a circle (when using trying to draw an ellipse)
                    r = Math.Abs(colDif); //Since that ellipse is essentially a circle, it could be rowDif as well. abs() makes it positive
                }
                else{ //If user didn't move
                    r = 1;
                }
            }
            else if (rowDif == 0 || colDif == 0) { //Cardinal cases (straight lines)
                r = (int)Math.Floor(gridManager.pythagLength(rowDif, colDif)); //Could just be whichever is not 0
            }
            else { //If user didn't move.
                r = 1;
            }

            if (getStrokeWidth() == 1){ //If no stroke
                drawCircle(getBeginGpos(), r); //Regular circle
            }
            else{ //If need stroke
                drawCircle(getBeginGpos(), r, false, true); //Outer Circle

                char lastActiveLetter = globalOperations.activeLetter;
                globalOperations.activeLetter = ' '; //Now draw with spaces

                int innerR = Math.Max(1, r - getStrokeWidth()); //At least 1

                drawCircle(getBeginGpos(), innerR, false, true); //Inner Circle

                globalOperations.activeLetter = lastActiveLetter; //Restore activeLetter

                flushPreviewQueue(previewQueue); //Draw the circles
            }
        }
        else if(!globalOperations.controls.Grid.RegularToggle.IsPressed()){ //Math to make an ellipse
            if (rowDif == 0 || colDif == 0){ //Line optimization i.e. draw line instead of flat ellipse, TODO: replace with soon to exist line with stroke width!
                if (rowDif == 0){
                    Line.line( //Line with length equal to flat ellipse
                    (beginGpos.row, beginGpos.col - (gpos.col - beginGpos.col)),
                    (gpos.row, gpos.col),
                    true);
                    return;
                }
                else if (colDif == 0){
                    Line.line( //Line with length equal to flat ellipse
                    (beginGpos.row - (gpos.row - beginGpos.row), beginGpos.col),
                    (gpos.row, gpos.col),
                    true);
                    return;                    
                }
            }
            else if (getStrokeWidth() != 1) { //Stroke Width Ellipse
                int resolutionFactor = 10; //resolutionFactor bigger is smoother
                int adjustedRowDif = Math.Abs(rowDif) * resolutionFactor;
                int adjustedColDif = Math.Abs(colDif) * resolutionFactor;

                float centerRow = getBeginGpos().row + 0.5f;
                float centerCol = getBeginGpos().col + 0.5f;

                float aOuter = Math.Abs(colDif) + 0.5f;
                float bOuter = Math.Abs(rowDif) + 0.5f;
                float aInner = Math.Max(1, aOuter - getStrokeWidth());
                float bInner = Math.Max(1, bOuter - getStrokeWidth());

                for (int i = -adjustedRowDif; i <= adjustedRowDif; i++) {
                    for (int j = -adjustedColDif; j <= adjustedColDif; j++) {
                        float testRow = i / (float)resolutionFactor;
                        float testCol = j / (float)resolutionFactor;

                        float adjustedRow = centerRow + testRow;
                        float adjustedCol = centerCol + testCol;

                        float outerNorm = (testCol * testCol) / (aOuter * aOuter) + (testRow * testRow) / (bOuter * bOuter);
                        float innerNorm = (testCol * testCol) / (aInner * aInner) + (testRow * testRow) / (bInner * bInner);

                        if (outerNorm <= 1.0f && innerNorm >= 1.0f) {
                            previewQueue.Add(((int)Math.Floor(adjustedRow), (int)Math.Floor(adjustedCol), globalOperations.activeLetter));
                        }
                    }
                }
                flushPreviewQueue(previewQueue);
            }
            else if (!isFilled){
                drawEllipse(drawQuadPixels, Math.Abs(rowDif), Math.Abs(colDif)); //Non-filled ellipse
            }
            else if (isFilled){
                drawEllipse(drawLinePairs, Math.Abs(rowDif), Math.Abs(colDif)); //Filled ellipse
            }
        }
    }
    public override void draw(){
        gridManager.emptyPreviewBuffer();
        (int row, int col) gpos;
        if (globalOperations.controls.Grid.OffsetToggle.IsPressed()){
            setBeginGpos(gridManager.getGridPos());
            gpos = startGpos;
        }
        else{
            setBeginGpos(startGpos);
            gpos = gridManager.getGridPos();
        }
        
        ellipseLogic(beginGpos, gpos);
    }
    public override void handleInput(){
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered){
            isFilled = !isFilled;
        }
        else if (globalOperations.controls.Grid.OffsetToggle.triggered){
            offset = !offset;
        }
        else if(
            globalOperations.controls.Grid.RegularToggle.triggered ||
            globalOperations.controls.Grid.RegularToggle.WasReleasedThisFrame()
        ){
            globalOperations.renderUpdate = true;
        }
    }
}