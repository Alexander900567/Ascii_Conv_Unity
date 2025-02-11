using UnityEngine;

public class Text : Tool
{

    (int row, int col) cursorGpos;
    [SerializeField] private RectTransform textCursor;

    public override void handleInput()
    {
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.triggered){
            cursorGpos = gridManager.getGridPos();
        }
        draw();
        renderTextCursor();
    }

    public override void draw(){
        if(Input.GetKeyDown(KeyCode.Backspace)){
            if (cursorGpos.col == 0){
                gridManager.addToGridArray(cursorGpos.row, cursorGpos.col, ' ');
            }
            else{
                gridManager.addToGridArray(cursorGpos.row, cursorGpos.col - 1, ' ');
                cursorGpos.col -= 1;
            }
            globalOperations.renderUpdate = true;
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
        else if (Input.anyKeyDown && Input.inputString.Length > 0){
            //Debug.Log(Input.inputString);
            gridManager.addToGridArray(cursorGpos.row, cursorGpos.col, Input.inputString[0]);
            if (cursorGpos.col < gridManager.getColCount() - 1){
                cursorGpos.col += 1;
            }
            globalOperations.renderUpdate = true;
        }
    }

    public override void onEnter(){
        cursorGpos = (0, 0);
        textCursor.sizeDelta = new Vector2(gridManager.getColSize(), gridManager.getRowSize());
        textCursor.localScale = new Vector3(1, 1, 1);
        globalOperations.controls.Grid.Disable();
    }

    public override void onExit(){
        textCursor.localScale = new Vector3(0, 0, 0);
        globalOperations.controls.Grid.Enable();
    }

    public void renderTextCursor(){
        textCursor.anchoredPosition = new Vector2(
            gridManager.getColSize() * cursorGpos.col + gridManager.uiPanelTransform.rect.width, 
            gridManager.getRowSize() * gridManager.invertRowPos(cursorGpos.row)
        );
    }
}
