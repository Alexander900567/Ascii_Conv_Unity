using UnityEngine;

public class Line : Tool
{

    public override void draw(){
        line(startGpos, gridManager.getGridPos(), true);
    }

    public void line(
        (int row, int col) startGpos,
        (int row, int col) grid_pos,
        bool clearBuffer
    ){
        
        if (clearBuffer){
            gridManager.emptyPreviewBuffer();
        }

        int horizontal_slope = grid_pos.row - startGpos.row;
        int vertical_slope = grid_pos.col - startGpos.col;
        int row_iter = 0;
        int col_iter = 0;

        if (horizontal_slope != 0){
            row_iter = horizontal_slope / System.Math.Abs(horizontal_slope);
        }
        if (vertical_slope != 0){
            col_iter = vertical_slope / System.Math.Abs(vertical_slope);
        }

        horizontal_slope = System.Math.Abs(horizontal_slope);
        vertical_slope = System.Math.Abs(vertical_slope);

        int long_slope;
        int short_slope;
        bool row_length_is_long;

        if (horizontal_slope > vertical_slope){
            long_slope = horizontal_slope;
            short_slope = vertical_slope + 1;
            row_length_is_long = true;
        }        
        else{
            long_slope = vertical_slope;
            short_slope = horizontal_slope + 1;
            row_length_is_long = false;
        }

        int per_chunk = long_slope / short_slope;
        int extra = (long_slope % short_slope) + 1;

        for (int x = 0; x < short_slope; x++){
            int this_chunk = per_chunk;
            if (extra > 0){
                this_chunk += 1;
                extra -= 1;
            }
            for (int y = 0; y < this_chunk; y++){
                gridManager.addToPreviewBuffer(startGpos.row, startGpos.col, globalOperations.active_letter);
                if (row_length_is_long){
                    startGpos.row += row_iter;
                }
                else {
                    startGpos.col += col_iter;
                }
            }
            if (!row_length_is_long){
                startGpos.row += row_iter;
            }
            else {
                startGpos.col += col_iter;
            }
        }
    }
}
