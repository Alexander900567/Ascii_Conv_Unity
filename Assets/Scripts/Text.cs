using UnityEngine;

public class Text : Tool
{

    [SerializeField] private UndoRedo undoRedo; 
    [SerializeField] private RectTransform textCursor;
    private (int row, int col) cursorGpos;
    private float commitTimer;
    private bool isTimerActive;
    private bool isTimerRinging;
    //[SerializeField] private 

    public override void handleInput()
    {
        if (globalOperations.controls.Grid.MainClick.triggered){
            stopTimer();
        }
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.IsPressed()){
            cursorGpos = gridManager.getGridPos();
        }
        draw();
        renderTextCursor();
        decrementTimer();
        if(isTimerRinging){
            gridManager.writePbufferToArray();
            isTimerRinging = false;
        }
    }

    public override void draw(){
        if(Input.GetKeyDown(KeyCode.Backspace)){
            if (cursorGpos.col == 0){
                gridManager.addToPreviewBuffer(cursorGpos.row, cursorGpos.col, ' ');
            }
            else{
                gridManager.addToPreviewBuffer(cursorGpos.row, cursorGpos.col - 1, ' ');
                cursorGpos.col -= 1;
            }
            globalOperations.renderUpdate = true;
            initTimer();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)){
            cursorGpos.row = Mathf.Max(cursorGpos.row - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            cursorGpos.row = Mathf.Min(cursorGpos.row + 1, gridManager.getRowCount() - 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            cursorGpos.col = Mathf.Max(cursorGpos.col - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
            cursorGpos.col = Mathf.Min(cursorGpos.col + 1, gridManager.getColCount() - 1);
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)){
            //probably implement a proper enter movement at some point
            //for now just here to avoid new lines being written to the grid
        }
        else if (Input.anyKeyDown && Input.inputString.Length > 0){
            //Debug.Log(Input.inputString);
            gridManager.addToPreviewBuffer(cursorGpos.row, cursorGpos.col, Input.inputString[0]);
            if (cursorGpos.col < gridManager.getColCount() - 1){
                cursorGpos.col += 1;
            }
            globalOperations.renderUpdate = true;
            initTimer();
        }
    }

    public override void onEnter(){
        cursorGpos = (0, 0);
        isTimerActive = false;
        isTimerRinging = false;
        textCursor.sizeDelta = new Vector2(gridManager.getColSize(), gridManager.getRowSize());
        textCursor.localScale = new Vector3(1, 1, 1);
        globalOperations.controls.Grid.Disable();
        globalOperations.controls.Grid.MainClick.Enable();
    }

    public override void onExit(){
        stopTimer();
        textCursor.localScale = new Vector3(0, 0, 0);
        globalOperations.controls.Grid.Enable();
    }

    public void renderTextCursor(){
        textCursor.anchoredPosition = new Vector2(
            gridManager.getColSize() * cursorGpos.col + gridManager.getUiBarWidth(), 
            gridManager.getRowSize() * gridManager.invertRowPos(cursorGpos.row)
        );
    }

    private void initTimer(){
        commitTimer = 2;
        isTimerActive = true;
    }
    private void decrementTimer(){
        if (isTimerActive){
            commitTimer -= Time.deltaTime;
            if (commitTimer <= 0){
                isTimerActive = false;
                isTimerRinging = true;
            }
        }
    }
    private void stopTimer(){
        if (gridManager.getPbufferLength() > 0){
            gridManager.writePbufferToArray();
        }
        isTimerActive = false;
        isTimerRinging = false;
    }
}
