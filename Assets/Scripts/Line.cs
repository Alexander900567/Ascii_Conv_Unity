using UnityEngine;

public class Line : Tool
{

    public override void draw(){
        line(startGpos, gridManager.getGridPos(), true);
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
}
