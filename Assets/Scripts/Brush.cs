using UnityEngine;

public class Brush : Tool
{
    [SerializeField] private Ellipse Ellipse;
    private (int row, int col) prevGpos;

    public override void draw(){ //Render brush draw if the brush was moved
        (int row, int col) gpos = gridManager.getGridPos();

        if (gpos != prevGpos){
            Ellipse.drawCircle(gpos, Toolbox.getStrokeWidth(), true);
        }
        prevGpos = gpos;
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