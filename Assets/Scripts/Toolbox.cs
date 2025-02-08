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
    [SerializeField] private Circle Circle;
    [SerializeField] private RectangleSelector RectangleSelector;


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
            global.renderUpdate = true;
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
    public void changeToCircle(){
        changeActiveTool(Circle);
    }
    public void changeToRectangleSelector(){
        changeActiveTool(RectangleSelector);
    }

}