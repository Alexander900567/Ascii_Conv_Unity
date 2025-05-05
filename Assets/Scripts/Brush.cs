using UnityEngine;

public class Brush : StrokeTool
{
    [SerializeField] private Ellipse Ellipse;
    private (int row, int col) prevGpos;

    public override void draw(){ //Render brush draw if the brush was moved
        (int row, int col) gpos = gridManager.getGridPos();

        if (gpos != prevGpos){
            Ellipse.drawCircle(gpos, getStrokeWidth(), true);
        }
        prevGpos = gpos;
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