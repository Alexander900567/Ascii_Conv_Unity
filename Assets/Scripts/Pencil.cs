using UnityEngine;

public class Pencil : Tool
{
    [SerializeField] private Line Line;
    private (int row, int col) prevGpos;

        public override void draw(){
            (int row, int col) gpos = gridManager.getGridPos();

            if (gpos != prevGpos){
                gridManager.addToPreviewBuffer(gpos.row, gpos.col, globalOperations.activeLetter);

                (int row, int col) topLeft = gpos;
                (int row, int col) bottomRight = gpos;

                for (int i = 0; i <= Toolbox.GetStrokeWidth(); i++) {
                    Line.line(
                        (i, topLeft.col),
                        (i, bottomRight.col),
                        false
                    );
                }   
            prevGpos = gpos;
        }
    }
}