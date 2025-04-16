using System.Collections.Generic;
using UnityEngine;

public class RectangleSelector : Tool
{
    [SerializeField] private UndoRedo undoRedo;
    [SerializeField] private GameObject selectorBox;
    private GameObject selectorBoxInstance;
    [SerializeField] private GameObject selectorOptionsContainer;
    [SerializeField] private GameObject commitButton;

    private bool active;
    private (int row, int col) topLeft;
    private (int row, int col) botRight;
    private List<(int, int, char)> originalBuffer;

    private enum SelectorTypes{
        Move,
        Copy,
    }
    private SelectorTypes activeType = SelectorTypes.Move; 

    public override void onUpdate(){
        handleInput();
        if (active){
            renderRectangleSelector();
        }
    }

    public override void handleInput(){
        (int row, int col) gpos = gridManager.getGridPos();
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.triggered){
            clickedGrid = true;
            onMouseDown(gpos);
        }
        else if (clickedGrid && globalOperations.controls.Grid.MainClick.IsPressed()){
           onMouseMove(gpos);
        }

        if (clickedGrid && globalOperations.controls.Grid.MainClick.WasReleasedThisFrame()){
            clickedGrid = false;
            onMouseUp();
        }
        
    }

    public override void onEnter(){
        active = false;
        startGpos = (-1, -1);
        topLeft = (-1, -1);
        botRight = (-1, -1);
        originalBuffer = new List<(int, int, char)>();
        selectorOptionsContainer.SetActive(true);
    }

    public override void onExit(){
        handleInterupt();
        selectorOptionsContainer.SetActive(false);
    }

    public void renderRectangleSelector(){
       selectorBoxInstance.GetComponent<HollowBoxAttach>().renderBox(topLeft, botRight); 
    }

    public void initializeSelectorBox(){
        selectorBoxInstance = Instantiate(
            selectorBox,
            new Vector3(0, 0, 0),
            transform.rotation
        );
        selectorBoxInstance.transform.SetParent(gridManager.canvasTransform);
    }

    
    public void onMouseDown((int row, int col) gpos){
        if (!active){
            startGpos = (gpos.row, gpos.col);
            changeCorners(gpos);
            initializeSelectorBox();
            renderRectangleSelector();
            undoRedo.disableUndoRedo();
        }
        else{
            startGpos = (gpos.row, gpos.col);
        }

    }

    public void onMouseMove((int row, int col) gpos){
        if (!active){
            changeCorners(gpos);
            renderRectangleSelector();
        }
        else if (gpos != startGpos){
            int rowDelta = gpos.row - startGpos.row;
            int colDelta = gpos.col - startGpos.col;

            boundDeltas(topLeft);
            boundDeltas(botRight);

            topLeft = (topLeft.row + rowDelta, topLeft.col + colDelta);
            botRight = (botRight.row + rowDelta, botRight.col + colDelta); 
            startGpos = gpos;
            if (rowDelta != 0 || colDelta != 0){
                for(int x = 0; x < gridManager.getPbufferLength(); x++){
                    gridManager.editPbufferItemPos(x, rowDelta, colDelta);
                }
            }

            void boundDeltas((int row, int col) corner){
                if (rowDelta + corner.row < 0) {rowDelta = corner.row * -1;}
                else if (rowDelta + corner.row >= gridManager.getRowCount()) {
                    rowDelta = gridManager.getRowCount() - 1 - corner.row;
                }
                if (colDelta + corner.col < 0) {colDelta = corner.col * -1;}
                else if (colDelta + corner.col >= gridManager.getColCount()) {
                    colDelta = gridManager.getColCount() - 1 - corner.col;
                }
            };
        }
    }

    public void onMouseUp(){
        if (!active){
            active = true;
            commitButton.SetActive(true);
            for(int row = topLeft.row; row <= botRight.row; row++){
                for(int col = topLeft.col; col <= botRight.col; col++){
                    gridManager.addToPreviewBuffer(row, col, gridManager.getGarrSpace(row, col));
                    originalBuffer.Add((row, col, gridManager.getGarrSpace(row, col)));
                    if (activeType == SelectorTypes.Move){
                        gridManager.addToGridArray(row, col, ' ');
                    }
                }
            } 
        }
    }

    private void resetStates(){
        active = false;
        topLeft = (-1, -1);
        botRight = (-1, -1);
        startGpos = (-1, -1);
        Destroy(selectorBoxInstance);
        commitButton.SetActive(false);
        undoRedo.enableUndoRedo();
    }

    public void resetBox(){
        resetStates();

        //Set up undo for this rectangle commit
        List<(int, int, char)> undoElement = new List<(int, int, char)>();
        //record what happened to the new position 
        foreach((int, int, char) item in gridManager.getPbuffer()){
            undoElement.Add((item.Item1, item.Item2, gridManager.getGarrSpace(item.Item1, item.Item2)));
        }
        //record what happened to the original position if nessacary
        if (activeType == SelectorTypes.Move){
            foreach((int, int, char) item in originalBuffer){
                undoElement.Add((item.Item1, item.Item2, item.Item3));
            }
        }
        undoRedo.addUndoElement(undoElement);
        
        //write the edit to the grid
        gridManager.writePbufferToArray(addToUndo:false);
    }

    public void changeCorners((int row, int col) newGpos){
        //derive the top left and bottom right corner benwteen start and new gpos
        topLeft = (
            Mathf.Min(startGpos.row, newGpos.row),
            Mathf.Min(startGpos.col, newGpos.col)
        );
        botRight = (
            Mathf.Max(startGpos.row, newGpos.row),
            Mathf.Max(startGpos.col, newGpos.col)
        );
    }

    private void handleInterupt(){
        //handle if Tool or SelectorType is changed while a box is active
        if (!active){
            return;
        }

        if (activeType == SelectorTypes.Move){
            resetBox();
        }
        else if (activeType == SelectorTypes.Copy){
            resetStates();
        }
    }

    private void changeSelectorState(SelectorTypes type){
        handleInterupt();
        activeType = type;
    }

    public void handleMoveToggle(bool toggleState){
        if(toggleState) { changeSelectorState(SelectorTypes.Move); }
    }    
    public void handleCopyToggle(bool toggleState){
        if(toggleState) { changeSelectorState(SelectorTypes.Copy); }
    }    



}
