using UnityEngine;

public class Brush : Tool
{
    [SerializeField] private Line Line;
    private (int row, int col) prevGpos;

    public override void draw(){ //Render brush draw if the brush was moved
        (int row, int col) gpos = gridManager.getGridPos();

        if (gpos != prevGpos){
            brush(gpos, Toolbox.getStrokeWidth());
        }
        prevGpos = gpos;
    }

    public void brush( //Simplified Ellipse draw()
    (int row, int col) gpos,
    int strokeWidth //Radius of brush
    ){ //Does filled Circle of radius strokeWidth
        int rowNum = 0; 
        int colNum = strokeWidth;
        int p = 1 - strokeWidth;

        while (rowNum <= colNum){
            Line.line(
            (gpos.row + rowNum, gpos.col + colNum),
            (gpos.row - rowNum, gpos.col + colNum),
            false);
            Line.line(
            (gpos.row + colNum, gpos.col + rowNum),
            (gpos.row - colNum, gpos.col + rowNum),
            false);
            Line.line(
            (gpos.row + rowNum, gpos.col - colNum),
            (gpos.row - rowNum, gpos.col - colNum),
            false);
            Line.line(
            (gpos.row + colNum, gpos.col - rowNum),
            (gpos.row - colNum, gpos.col - rowNum),
            false);            
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

    public override void onEnter()
    {
        base.onEnter();
    }
    public override void onExit()
    {
        base.onExit();
    }
}