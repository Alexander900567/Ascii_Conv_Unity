using UnityEngine;

public class Brush : Tool
{
    [SerializeField] private Line Line;
    [SerializeField] private GameObject MoreStrokeButton;
    [SerializeField] private GameObject LessStrokeButton;
    private (int row, int col) prevGpos;
    private int strokeWidth = 1;

    public override void draw(){
        (int row, int col) gpos = gridManager.getGridPos();

        if (gpos != prevGpos){
            brush(gpos, strokeWidth);
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
    public void increaseStrokeWidth(int increase){
        if (strokeWidth + increase <= 100){
            strokeWidth += increase;
        }
    }
    public void decreaseStrokeWidth(int decrease){
        if (strokeWidth - decrease >= 1){
            strokeWidth -= decrease;
        }
    }

    public override void onEnter()
    {
        base.onEnter();
        MoreStrokeButton.SetActive(true);       
        LessStrokeButton.SetActive(true);
    }
    public override void onExit()
    {
        base.onExit();
        MoreStrokeButton.SetActive(false);       
        LessStrokeButton.SetActive(false);
    }
}