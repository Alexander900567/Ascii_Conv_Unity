using UnityEngine;

public class Brush : Tool
{
    [SerializeField] private Line Line;
    [SerializeField] private GameObject MoreStrokeButton;
    [SerializeField] private GameObject LessStrokeButton;
    private (int row, int col) prevGpos;
    private int strokeWidth = 1;
    private int strokeMin = 1;
    private int strokeMax = 100;

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
    public void increaseStrokeWidth(int increase){ //These are called by keybinds and buttons and soon by input fields
        if (strokeWidth + increase <= strokeMax){ //If less than or equal to upper bound for strokeWidth
            strokeWidth += increase; //Increase as usual
        }
        else if (strokeWidth + increase > strokeMax){ //If goes over
            strokeWidth = strokeMax; //Set to max
        }

    }
    public void decreaseStrokeWidth(int decrease){
        if (strokeWidth - decrease >= strokeMin){ //If less than or equal to lower bound
            strokeWidth -= decrease; //Decrease as usual
        }
        else if (strokeWidth - decrease < strokeMin){ //If goes under
            strokeWidth = strokeMin; //Set to min
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