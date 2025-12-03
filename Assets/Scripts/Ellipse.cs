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

    //Called when "pen", "tool", or mouse is picked up
    private void setBeginGpos((int row, int col) newBeginGpos)
    {
        beginGpos.row = newBeginGpos.row;
        beginGpos.col = newBeginGpos.col;
    }

    //Returns the beginGpos tuple indicating where the "pen", "tool", or mouse
    //first made "contact"
    private (int row, int col) getBeginGpos() { return beginGpos; }
    
    //Adds a line of points to the preview Queue
    private void previewQueueLine((int row, int col) beginGposLocal, (int row, int col) gridPos)
    {
        //Adjusts direction to ensure complete iteration
        int rowIter = beginGposLocal.row <= gridPos.row ? 1 : -1;
        int colIter = beginGposLocal.col <= gridPos.col ? 1 : -1;

        for (int j = beginGposLocal.row; j != gridPos.row + rowIter; j += rowIter)
        {
            for (int i = beginGposLocal.col; i != gridPos.col + colIter; i += colIter)
            {
                previewQueue.Add((j, i, globalOperations.activeLetter));
            }
        }
    }
    private void flushPreviewQueue(List<(int, int, char)> queue)
    {
        //We need to handle queue before deleting its contents
        //Stores difference of two circles
        HashSet<(int, int)> conflictingPositions = new HashSet<(int, int)>();
        //Finds difference of two circles
        foreach ((int row, int col, char input) in queue)
        {
            //Identifies the difference
            if (input == ' ')
            {
                //Stores the intentional void as the difference
                conflictingPositions.Add((row, col)); 
            }
        }
        //Finds drawable points
        foreach ((int row, int col, char input) in queue)
        {
            //Non-voids which are not recorded as a difference
            if (input != ' ' && !conflictingPositions.Contains((row, col)))
            {
                //So they get drawn
                gridManager.addToPreviewBuffer(row, col, input);
            }
        }
        //Garbage collector should kill this
        conflictingPositions.Clear();
        //Now the queue can truly be emptied and flushed
        previewQueue.Clear();
    }

    public void drawCircle((int row, int col) beginGposLocal, int r, bool forceFill = false, bool strokeCircle = false)
    {
        int rowNum = 0;
        int colNum = r;
        int p = 1 - r;


        while (rowNum <= colNum)
        {
            //Draws 8 sections "simulataneously"
            //These are handled in each case
            //These ifs are probably wasteful

            //Stroked Circle Case
            if (strokeCircle)
            {
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
            //"Normal" Circle Case (No Stroke or Fill)
            else if (!isFilled && !forceFill)
            {
                gridManager.addToPreviewBuffer(beginGposLocal.row + rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row + colNum, beginGposLocal.col + rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - colNum, beginGposLocal.col + rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - rowNum, beginGposLocal.col + colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row - colNum, beginGposLocal.col - rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row + colNum, beginGposLocal.col - rowNum, globalOperations.activeLetter);
                gridManager.addToPreviewBuffer(beginGposLocal.row + rowNum, beginGposLocal.col - colNum, globalOperations.activeLetter);
            }
            //Filled Circle Case
            else if (isFilled || forceFill)
            {
                //Much like the filled ellipse, we use lines to fill within the circle
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
            //No matter how the sections were drawn
            //We must update variables to draw the next point correctly
            rowNum += 1;
            if (p < 0)
            {
                p += 2 * rowNum + 1;
            }
            else
            {
                colNum -= 1;
                p += 2 * (rowNum - colNum) + 1;
            }
        }
    }
    //VSCode says this is never called. None of this may not be necessary
    //If necessary, then it's supposed to initialize key drawing functions
    private void Awake(){ 
        if (gridManager == null || globalOperations == null || Line == null) {
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
    private void drawEllipse(Action<int, int> renderFunc, int rowDif, int colDif)
    {

        int rowNum = 0; //Used to keep track of drawing math 
        int colNum = colDif; //See midpoint ellipse algorithm for more detail

        float rowDifSquared = rowDif * rowDif; //Used for math later :)
        float colDifSquared = colDif * colDif;

        float pRow = 0; //More drawing math
        float pCol = 2 * rowDifSquared * colNum;

        renderFunc(rowNum, colNum);

        float p = colDifSquared - (rowDifSquared * colDif) + (0.25f * rowDifSquared);

        //Top and bottom
        while (pRow <= pCol)
        {
            rowNum += 1;
            pRow += 2.0f * colDifSquared;
            if (p < 0.0f)
            {
                p += colDifSquared + pRow;
            }
            else
            {
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

        while (colNum >= 0)
        {
            colNum -= 1;
            pCol += -2.0f * rowDifSquared;
            if (p > 0.0f)
            {
                p += rowDifSquared - pCol;
            }
            else
            {
                rowNum += 1;
                pRow += 2.0f * colDifSquared;
                p += rowDifSquared - pCol + pRow;
            }
            renderFunc(rowNum, colNum);
        }
    }
    //TODO: Fix filled circle and ellipse with stroke > 1 (drawn with stroke despite fill being enabled)
    //Controls logic to draw circle, ellipse, filled or not filled
    private void ellipseLogic((int row, int col) beginGpos, (int row, int col) gpos)
    {
        int rowDif = gpos.row - beginGpos.row; //row and col components of
        int colDif = gpos.col - beginGpos.col; //difference between start and end

        //This is quite the beast. Rewrite would do wonders
        //Circle Case
        //This is chosen if we can save time by drawing a circle instead of an implicit regular ellipse
        //Or alternately if the user wants to draw a regular ellipse explicitly
        if (globalOperations.controls.Grid.RegularToggle.IsPressed() ||
        (!globalOperations.controls.Grid.RegularToggle.IsPressed() && rowDif == colDif))
        {
            //Calculates size of circle user wants
            //Its assignment depends on settings and the user moving their mouse

            int r;
            //Non-cardinal Mouse Drag Cases (diagonals)
            if (rowDif != 0 && colDif != 0)
            {
                //When user explicitly asks for a circle
                if (globalOperations.controls.Grid.RegularToggle.IsPressed())
                {
                    //Radius is easy to calculate
                    r = (int)Math.Floor(gridManager.pythagLength(rowDif, colDif));
                }
                //Implicit Circle Case (Optimization if ellipses ever have same dimensions as a circle)
                else if (!globalOperations.controls.Grid.RegularToggle.IsPressed())
                {
                    r = Math.Abs(colDif); //Since that ellipse is essentially a circle, it could be rowDif as well. abs() makes it positive
                }
                else
                //No Mouse Movement or Weirdness Case
                {
                    r = 1;
                }
            }
            //Cardinal Mouse Drag Cases (straight lines)
            else if (rowDif == 0 || colDif == 0)
            {
                r = (int)Math.Floor(gridManager.pythagLength(rowDif, colDif)); //Could just be whichever is not 0
            }
            //No Mouse Drag Case
            else
            {
                r = 1;
            }

            //Now that radius is known
            //We can handle drawing the preview

            //No Stroke Circle Case
            if (getStrokeWidth() == 1)
            {
                drawCircle(getBeginGpos(), r); //Regular circle
            }
            //Stroke Circle Case
            else
            {
                drawCircle(getBeginGpos(), r, false, true); //Outer Circle

                char lastActiveLetter = globalOperations.activeLetter;
                globalOperations.activeLetter = ' '; //Now draw with spaces

                int innerR = Math.Max(1, r - getStrokeWidth()); //At least 1

                drawCircle(getBeginGpos(), innerR, false, true); //Inner Circle

                globalOperations.activeLetter = lastActiveLetter; //Restore activeLetter

                flushPreviewQueue(previewQueue); //Draw the circles
            }
        }
        //Ellipse Case
        else if (!globalOperations.controls.Grid.RegularToggle.IsPressed())
        {
            //Implicit Line Case (Optimization if ellipses ever have same dimensions as a line)
            if (rowDif == 0 || colDif == 0)
            { //TODO: replace with soon to exist line with stroke width!
                //Implicit Vertical Line Case
                if (rowDif == 0)
                {
                    Line.line( //Line with length equal to flat ellipse
                    (beginGpos.row, beginGpos.col - (gpos.col - beginGpos.col)),
                    (gpos.row, gpos.col),
                    true);
                    return;
                }
                //Implicit Horizontal Line Case
                else if (colDif == 0)
                {
                    Line.line( //Line with length equal to flat ellipse
                    (beginGpos.row - (gpos.row - beginGpos.row), beginGpos.col),
                    (gpos.row, gpos.col),
                    true);
                    return;
                }
            }
            //Stroke Ellipse Case
            else if (getStrokeWidth() != 1)
            {
                int resolutionFactor = 10; //resolutionFactor bigger is smoother
                int adjustedRowDif = Math.Abs(rowDif) * resolutionFactor;
                int adjustedColDif = Math.Abs(colDif) * resolutionFactor;

                float centerRow = getBeginGpos().row + 0.5f;
                float centerCol = getBeginGpos().col + 0.5f;

                float aOuter = Math.Abs(colDif) + 0.5f;
                float bOuter = Math.Abs(rowDif) + 0.5f;
                float aInner = Math.Max(1, aOuter - getStrokeWidth());
                float bInner = Math.Max(1, bOuter - getStrokeWidth());

                //Check a rectangle of size adjustedRowDif * adjustedColDif
                for (int i = -adjustedRowDif; i <= adjustedRowDif; i++)
                {
                    for (int j = -adjustedColDif; j <= adjustedColDif; j++)
                    {
                        float testRow = i / (float)resolutionFactor;
                        float testCol = j / (float)resolutionFactor;

                        float adjustedRow = centerRow + testRow;
                        float adjustedCol = centerCol + testCol;

                        float outerNorm = (testCol * testCol) / (aOuter * aOuter) + (testRow * testRow) / (bOuter * bOuter);
                        float innerNorm = (testCol * testCol) / (aInner * aInner) + (testRow * testRow) / (bInner * bInner);

                        //If the point falls within the ellipse, draw
                        if (outerNorm <= 1.0f && innerNorm >= 1.0f)
                        {
                            previewQueue.Add(((int)Math.Floor(adjustedRow), (int)Math.Floor(adjustedCol), globalOperations.activeLetter));
                        }
                    }
                }
                //Once preview has entered the queue, make sure the queue is flushed and the stroke is handled
                flushPreviewQueue(previewQueue);
            }
            //"Normal" Ellipse Case (no stroke or fill)
            else if (!isFilled)
            {
                drawEllipse(drawQuadPixels, Math.Abs(rowDif), Math.Abs(colDif)); //Non-filled ellipse
            }
            //Filled Ellipse Case
            else if (isFilled)
            {
                drawEllipse(drawLinePairs, Math.Abs(rowDif), Math.Abs(colDif)); //Filled ellipse
            }
        }
    }
    //Handles offsets, updates beginGpos, and calls ellipseLogic
    public override void draw() {
        gridManager.emptyPreviewBuffer();
        (int row, int col) gpos;
        
        if (globalOperations.controls.Grid.OffsetToggle.IsPressed()) {
            setBeginGpos(gridManager.getGridPos());
            gpos = startGpos;
        }
        else {
            setBeginGpos(startGpos);
            gpos = gridManager.getGridPos();
        }
        
        ellipseLogic(beginGpos, gpos);
    }
    public override void handleInput() {
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered)
        {
            isFilled = !isFilled;
        }
        else if (globalOperations.controls.Grid.OffsetToggle.triggered)
        {
            offset = !offset;
        }
        else if (globalOperations.controls.Grid.RegularToggle.triggered ||
            globalOperations.controls.Grid.RegularToggle.WasReleasedThisFrame())
        {
            globalOperations.renderUpdate = true;
        }
    }
}