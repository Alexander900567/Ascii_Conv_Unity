using UnityEngine;

public class Pencil : Tool
{

    private (int row, int col) prevGpos;

    public override void draw(){
        (int row, int col) gpos = gridManager.getGridPos();

        if (gpos != prevGpos){
            gridManager.add_to_preview_buffer(gpos.row, gpos.col, globalOperations.active_letter);
        } 

        prevGpos = gpos;
    }




}
