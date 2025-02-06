using System;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager gridManager;

    private (int row, int col) prevGpos; 
    private Tool activeTool;
    [SerializeField] private Pencil Pencil;
    [SerializeField] private Line Line;
    [SerializeField] private Rectangle Rectangle;
    [SerializeField] private Text Text;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevGpos = (-1, -1);
        activeTool = Pencil;
    }

    // Update is called once per frame
    void Update()
    {
        activeTool.onUpdate();

        (int row, int col) gpos = gridManager.getGridPos();
        if (gpos != prevGpos){
            global.render_update = true;
        }
        prevGpos = gpos; 
    }

    public void changeActiveTool(Tool newTool){
        activeTool.onExit();
        activeTool = newTool;
        activeTool.onEnter();
    }

    //-----button functions-----
    public void changeToPencil(){
        changeActiveTool(Pencil);
    }
    public void changeToLine(){
        changeActiveTool(Line);
    }
    public void changeToRectangle(){
        changeActiveTool(Rectangle);
    }
    public void changeToText(){
        changeActiveTool(Text);
    }

/*

    private void circle(
        (int row, int col) startGpos,
        (int row, int col) grid_pos
    ){
        gridManager.empty_preview_buffer(); //Assume user will make a new circle

        int row_dif = grid_pos.row - startGpos.row; //row and col components of
        int col_dif = grid_pos.col - startGpos.col; //difference between start and end
        float diagonal_r = (float)Math.Sqrt((row_dif * row_dif) + (col_dif * col_dif));
        //pythag: c = sqrt(a^2 + b^2)
        //this is not yet usable due to the geometry of a grid in non-cardinal cases
        //Note about precision: if not good enough, make these floats into doubles
        int r;
        if (row_dif != 0 && col_dif != 0) { //non-cardinal case AKA trig time
            int o = Math.Abs(col_dif); //converts o to be positive to work with sin()
            float angle_theta = (float)Math.Asin(o / diagonal_r);
            float h = (float)(o / diagonal_r) / (float)Math.Sin(angle_theta); //hypotenuse
            float r0 = diagonal_r / h; //radius in terms of pixels
            r = (int)Math.Floor(r0); //floor makes our radius usable
        }
        else if (row_dif == 0 || col_dif == 0) {
            r = (int)Math.Floor(diagonal_r);
        }
        else {
            r = 0;
        }

        int row_num = 0;
        int col_num = r;
        int p = 1 - r;

        while (row_num <= col_num) { // draws 8 sections "simulataneously"
            gridManager.addToPreviewBuffer(startGpos.row + row_num, startGpos.col + col_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row + col_num, startGpos.col + row_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row - col_num, startGpos.col + row_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row - row_num, startGpos.col + col_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row - row_num, startGpos.col - col_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row - col_num, startGpos.col - row_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row + col_num, startGpos.col - row_num, active_letter);
            gridManager.addToPreviewBuffer(startGpos.row + row_num, startGpos.col - col_num, active_letter);
        
            row_num += 1;
            if (p < 0) {
                p += 2 * row_num + 1;
            }
            else {
                col_num -= 1;
                p += 2 * (row_num - col_num) + 1;
            }
        }
    }
    public void text(){


        global.render_update = true;
    }

*/
}