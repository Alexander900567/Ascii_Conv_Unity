using UnityEngine;

public class Pencil : Tool
{

    private (int row, int col) prevGpos;

    public override void draw(){
        (int row, int col) gpos = gridManager.getGridPos();

        if (gpos != prevGpos){
            gridManager.addToPreviewBuffer(gpos.row, gpos.col, globalOperations.activeLetter);
        } 

        prevGpos = gpos;
    }

}