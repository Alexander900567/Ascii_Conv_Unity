using UnityEngine;

public class Rectangle : Tool
{
    [SerializeField] Line Line;


    public override void draw()
    {
        gridManager.emptyPreviewBuffer();

        (int row, int col) gpos = gridManager.getGridPos();

        Line.line(
            (startGpos.row, startGpos.col),
            (startGpos.row, gpos.col),
            false
        );
        Line.line(
            (startGpos.row, startGpos.col),
            (gpos.row, startGpos.col),
            false
        );
        Line.line(
            (gpos.row, startGpos.col),
            (gpos.row, gpos.col),
            false
        );
        Line.line(
            (startGpos.row, gpos.col),
            (gpos.row, gpos.col),
            false
        );
    }

}
