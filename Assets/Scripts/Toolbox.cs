using System;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Toolbox : MonoBehaviour
{

    public GlobalOperations global;
    public GridManager gridManager;
    public LetterSelector letterSelector;

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
    [SerializeField] private Bucket Bucket;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prevGpos = (-1, -1); //Load defaults
        activeTool = Pencil;
        activeTool.onEnter();
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
        highlightCorrectToggle(newTool);
    }

    private void highlightCorrectToggle(Tool newTool){
        string toolName = newTool.GetType().Name;
        GameObject toolToggle = GameObject.Find($"{toolName}Toggle");
        if (toolToggle == null){
            toolToggle = GameObject.Find("HiddenToggle");
        }
        Toggle toggle = toolToggle.GetComponent<Toggle>(); 
        toggle.isOn = true;
    }

    //-----button functions-----
    public void changeToPencil(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Pencil);
        }
    }
    public void changeToLine(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Line);
        }
    }
    public void changeToRectangle(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Rectangle);
        }
    }
    public void changeToText(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Text);
        }
    }
    public void changeToEllipse(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Ellipse);
        }
    }
    public void changeToRectangleSelector(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(RectangleSelector);
        }
    }
    public void changeToEraser(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Eraser);
        }
    }
    public void changeToImageConvertor(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(ImageConvertor);
        }
    }
    public void changeToBrush(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Brush);
        }
    }
    public void changeToBucket(bool toggleStatus){
        if (toggleStatus){
            changeActiveTool(Bucket);
        }
    }
    public void setLetterListeningTrue(){
        isLetterListening = true;
    }

    public void handleInput(){
        if(isLetterListening){
            readInActiveLetter();
        }
        else if(global.controls.PopUp.ClosePopUp.triggered){
            global.closePopUp();
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
            changeToPencil(true);
        }
        else if(global.controls.Grid.EraserSwitch.triggered){
            changeToEraser(true);
        }
        else if(global.controls.Grid.BrushSwitch.triggered){
            changeToBrush(true);
        }
        else if(global.controls.Grid.LineSwitch.triggered){
            changeToLine(true);
        }
        else if(global.controls.Grid.RectangleSwitch.triggered){
            changeToRectangle(true);
        }
        else if(global.controls.Grid.TextSwitch.triggered){
            changeToText(true);
        }
        else if(global.controls.Grid.RectangleSelectorSwitch.triggered){
            changeToRectangleSelector(true);
        }
        else if(global.controls.Grid.EllipseSwitch.triggered){
            changeToEllipse(true);
        }
        else if(global.controls.Grid.ConverterSwitch.triggered){
            changeToImageConvertor(true);
        }
        else if(global.controls.Grid.LetterSwitch.triggered){
            setLetterListeningTrue();
        }
        else if(global.controls.Grid.PerformStrokeBigIncrease.triggered){
            if (activeTool is StrokeTool){
                StrokeTool tempTool = activeTool as StrokeTool;
                tempTool.increaseStrokeWidth(5);
            }
        }
        else if(global.controls.Grid.PerformStrokeBigDecrease.triggered){
            if (activeTool is StrokeTool){
                StrokeTool tempTool = activeTool as StrokeTool;
                tempTool.decreaseStrokeWidth(5);
            }
        }
        else if(global.controls.Grid.PerformStrokeIncrease.triggered){
            if (activeTool is StrokeTool){
                StrokeTool tempTool = activeTool as StrokeTool;
                tempTool.increaseStrokeWidth(1);
            }
        }
        else if(global.controls.Grid.PerformStrokeDecrease.triggered){
            if (activeTool is StrokeTool){
                StrokeTool tempTool = activeTool as StrokeTool;
                tempTool.decreaseStrokeWidth(1);
            }
        }
        
        void readInActiveLetter(){
            if(Input.inputString.Length > 0){
                global.activeLetter = Input.inputString[0];
                letterSelector.moveBoxToActiveLetter();
                isLetterListening = false;
            }
        }
    }

}