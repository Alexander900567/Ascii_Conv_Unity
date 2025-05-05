using UnityEngine;

public class Pencil : StrokeTool
{
    [SerializeField] private Line Line;
    private (int row, int col) prevGpos;

        public override void draw(){
            (int row, int col) gpos = gridManager.getGridPos();

            if (gpos != prevGpos){
                gridManager.addToPreviewBuffer(gpos.row, gpos.col, globalOperations.activeLetter);
                
                int upperRow = gpos.row - getStrokeWidth() + 1;
                int lowerRow = gpos.row + getStrokeWidth() - 1;
                int leftSide = gpos.col - getStrokeWidth() + 1;
                int rightSide = gpos.col + getStrokeWidth() - 1;

                for (int i = upperRow; i <= lowerRow; i++) {
                    Line.line(
                        (i, leftSide),
                        (i, rightSide),
                        false
                    );
                }
            prevGpos = gpos;
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