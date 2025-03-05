using UnityEngine;
using System;

public class Pencil : Tool
{

    [SerializeField] private Line Line;
    private (int row, int col) prevGpos = (0,0);
    private bool isRegular = false;
    public override void draw(){
        (int row, int col) gpos = gridManager.getGridPos();
        if (Line == null){
            Debug.LogError("Line is null");
        }
        if (gpos != prevGpos){
            if (!isRegular){
                gridManager.addToPreviewBuffer(gpos.row, gpos.col, globalOperations.activeLetter);
            }
            else if (isRegular){
                int row_dif = gpos.row - prevGpos.row;
                int col_dif = gpos.col - prevGpos.col;

                if (Math.Abs(row_dif) != Math.Abs(col_dif)) { //If not already a square, then make square
                    int smaller_dif;
                    if (Math.Abs(row_dif) < Math.Abs(col_dif)){ //Determine shorter component
                        smaller_dif = row_dif;
                    }
                    else{
                        smaller_dif = col_dif;
                    }
                    smaller_dif = Math.Abs(smaller_dif); //The math can be optimized I bet
                    if (col_dif > 0 && row_dif < 0){
                        gpos.row = prevGpos.row - smaller_dif;
                        gpos.col = prevGpos.col + smaller_dif;
                    }
                    else if (col_dif > 0 && row_dif > 0){ //Conforms longer side to be equal to smaller side
                        gpos.row = prevGpos.row + smaller_dif;
                        gpos.col = prevGpos.col + smaller_dif;
                    }
                    else if (col_dif < 0 && row_dif > 0){
                        gpos.row = prevGpos.row + smaller_dif;
                        gpos.col = prevGpos.col - smaller_dif;
                    }
                    else if (col_dif < 0 && row_dif < 0){
                        gpos.row = prevGpos.row - smaller_dif;
                        gpos.col = prevGpos.col - smaller_dif;
                    }
                }    
            Line.line((prevGpos.row, prevGpos.col), (gpos.row, gpos.col), false);                
            }
        } 
        prevGpos = gpos;
    }
    public override void handleInput(){
        base.handleInput();
        if (globalOperations.controls.Grid.RegularToggle.triggered){
            isRegular = !isRegular;
        }
    }
}
