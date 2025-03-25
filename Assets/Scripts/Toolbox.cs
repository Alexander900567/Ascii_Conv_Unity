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
    [SerializeField] private Ellipse Ellipse;
    [SerializeField] private RectangleSelector RectangleSelector;
    [SerializeField] private Eraser Eraser;
    [SerializeField] private ImageConvertor ImageConvertor;
    [SerializeField] private Brush Brush;
    [SerializeField] private SaveLoad SaveLoad;


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
        //let the active tool do its thing
        activeTool.onUpdate();

        //handle any keyboard input.
        handleInput();

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
    public void changeToEllipse(){
        changeActiveTool(Ellipse);
    }
    public void changeToRectangleSelector(){
        changeActiveTool(RectangleSelector);
    }
    public void changeToEraser(){
        changeActiveTool(Eraser);
    }
    public void changeToImageConvertor(){
        changeActiveTool(ImageConvertor);
    }
    public void changeToBrush(){
        changeActiveTool(Brush);
    }
    public void setLetterListeningTrue(){
        isLetterListening = true;
    }

    public void handleInput(){
        if(isLetterListening){
            readInActiveLetter();
        }
        else if(global.controls.Grid.PerformCopy.triggered){ //These are at the beginning because they have modifiers
            gridManager.copyGridToClipboard();
        }
        else if(global.controls.Grid.PerformSave.triggered){
            SaveLoad.saveGridArray();
        }
        else if(global.controls.Grid.PerformLoad.triggered){
            SaveLoad.loadGridArray();
        }
        else if(global.controls.Grid.PenSwitch.triggered){
            changeToPencil();
        }
        else if(global.controls.Grid.EraserSwitch.triggered){
            changeToEraser();
        }
        else if(global.controls.Grid.BrushSwitch.triggered){
            changeToBrush();
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
        else if(global.controls.Grid.RectangleSelectorSwitch.triggered){
            changeToRectangleSelector();
        }
        else if(global.controls.Grid.EllipseSwitch.triggered){
            changeToEllipse();
        }
        else if(global.controls.Grid.ConverterSwitch.triggered){
            changeToImageConvertor();
        }
        else if(global.controls.Grid.LetterSwitch.triggered){
            setLetterListeningTrue();
        }
        else if(global.controls.Grid.PerformStrokeBigIncrease.triggered){ //Same with these abt modifiers
            Brush.increaseStrokeWidth(5); 
        }
        else if(global.controls.Grid.PerformStrokeBigDecrease.triggered){
            Brush.decreaseStrokeWidth(5);
        }
        else if(global.controls.Grid.PerformStrokeIncrease.triggered){
            Brush.increaseStrokeWidth(1); //Change to a global stroke width change whenever it is implemented
        }
        else if(global.controls.Grid.PerformStrokeDecrease.triggered){
            Brush.decreaseStrokeWidth(1);
        }
        
        void readInActiveLetter(){
            if(Input.inputString.Length > 0){
                global.activeLetter = Input.inputString[0];
                isLetterListening = false;
            }
        }
    }


    

}