using System;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager gridManager;

    private bool isLetterListening;
    private (int row, int col) prevGpos; 
    private Tool activeTool;
    [SerializeField] private Pencil Pencil;
    [SerializeField] private Line Line;
    [SerializeField] private Rectangle Rectangle;
    [SerializeField] private Text Text;
    [SerializeField] private Circle Circle;
    [SerializeField] private RectangleSelector RectangleSelector;
    [SerializeField] private Eraser Eraser;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevGpos = (-1, -1);
        activeTool = Pencil;
        isLetterListening = false;
    }

    // Update is called once per frame
    void Update()
    {
        //handle any keyboard input
        //If this if statement gets more complicated than just excluding during text mode, think up a better solution
        if (activeTool != Text){
            handleInput();
        }

        //let the active tool do its thing
        activeTool.onUpdate();

        //handle if a rerender of the grid should happen
        if(global.controls.Grid.MainClick.IsPressed()){
            (int row, int col) gpos = gridManager.getGridPos();
            if (gpos != prevGpos){
                global.renderUpdate = true;
            }
            prevGpos = gpos;
        } 
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
    public void changeToEraser(){
        changeActiveTool(Eraser);
    }
    public void setLetterListeningTrue(){
        isLetterListening = true;
    }

    public void handleInput(){
        if(isLetterListening){
            readInActiveLetter();
        }
        else if(global.controls.Grid.PenSwitch.triggered){
            changeToPencil();
        }
        else if(global.controls.Grid.LineSwitch.triggered){
            changeToLine();
        }
        else if(global.controls.Grid.RectangleSwitch.triggered){
            changeToRectangle();
        }
        else if(global.controls.Grid.TextSwitch.triggered){
            changeToText();
        }
        else if(global.controls.Grid.CircleSwitch.triggered){
            changeToCircle();
        }
        else if(global.controls.Grid.EraserSwitch.triggered){
            changeToEraser();
        }
        /* Uncomment when keybind is added
        else if(global.controls.Grid.RectangleSelectorSwitch.triggered){
            changeToRectangleSelector();
        }
        */
        /* Uncomment when the keybind is added
        else if(global.controls.Grid.LetterSwitch.triggered){
            setLetterListeningTrue();
        }
        */
        
        void readInActiveLetter(){
            if(Input.inputString.Length > 0){
                global.activeLetter = Input.inputString[0];
                isLetterListening = false;
            }
        }
    }


    

}