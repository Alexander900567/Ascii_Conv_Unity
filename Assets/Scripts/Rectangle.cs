using UnityEngine;
using System;

public class Rectangle : Tool
{
    [SerializeField] private Line Line;
    private bool isFilled = false;

    public override void draw()
    {
        gridManager.emptyPreviewBuffer();
        (int row, int col) gpos = gridManager.getGridPos();

        if (globalOperations.controls.Grid.RegularToggle.IsPressed()){ //Math to make rectangle a square
            int row_dif = gpos.row - startGpos.row;
            int col_dif = gpos.col - startGpos.col;

            if (Math.Abs(row_dif) != Math.Abs(col_dif)) { //If not already a square, then make square
                int smaller_dif;
                if (Math.Abs(row_dif) < Math.Abs(col_dif)){ //Determine shorter side
                    smaller_dif = row_dif;
                }
                else{
                    smaller_dif = col_dif;
                }
                smaller_dif = Math.Abs(smaller_dif); //The math can be optimized I bet
                if (col_dif > 0 && row_dif < 0){
                    gpos.row = startGpos.row - smaller_dif;
                    gpos.col = startGpos.col + smaller_dif;
                }
                else if (col_dif > 0 && row_dif > 0){ //Conforms longer side to be equal to smaller side
                    gpos.row = startGpos.row + smaller_dif;
                    gpos.col = startGpos.col + smaller_dif;
                }
                else if (col_dif < 0 && row_dif > 0){
                    gpos.row = startGpos.row + smaller_dif;
                    gpos.col = startGpos.col - smaller_dif;
                }
                else if (col_dif < 0 && row_dif < 0){
                    gpos.row = startGpos.row - smaller_dif;
                    gpos.col = startGpos.col - smaller_dif;
                }
            }
        }

        if (!isFilled) {
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
        else if (isFilled) {
        int upper_row;
        int lower_row;

            if (startGpos.row <= gpos.row) {
                upper_row = startGpos.row;
                lower_row = gpos.row;
            }
            else { //hopefully no error
                upper_row = gpos.row;
                lower_row = startGpos.row;
            }

            for (int i = upper_row; i <= lower_row; i++) {
                Line.line(
                    (i, startGpos.col),
                    (i, gpos.col),
                    false
                );
            }   
        }
    }

    public override void handleInput(){
        base.handleInput();
        if (globalOperations.controls.Grid.FilledToggle.triggered){
            isFilled = !isFilled;
            globalOperations.renderUpdate = true;
        }
        else if(
            globalOperations.controls.Grid.RegularToggle.triggered ||
            globalOperations.controls.Grid.RegularToggle.WasReleasedThisFrame()
        ){
            globalOperations.renderUpdate = true;
        }
    }

}
