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
                gridManager.add_to_grid_array(cursorGpos.row, cursorGpos.col, ' ');
            }
            else{
                gridManager.add_to_grid_array(cursorGpos.row, cursorGpos.col - 1, ' ');
                cursorGpos.col -= 1;
            }
            globalOperations.render_update = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow)){
            cursorGpos.row = Mathf.Max(cursorGpos.row - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            cursorGpos.row = Mathf.Min(cursorGpos.row + 1, gridManager.get_row_count() - 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            cursorGpos.col = Mathf.Max(cursorGpos.col - 1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)){
            cursorGpos.col = Mathf.Min(cursorGpos.col + 1, gridManager.get_col_count() - 1);
        }
        else if (Input.anyKeyDown && Input.inputString.Length > 0){
            //Debug.Log(Input.inputString);
            gridManager.add_to_grid_array(cursorGpos.row, cursorGpos.col, Input.inputString[0]);
            if (cursorGpos.col < gridManager.get_col_count() - 1){
                cursorGpos.col += 1;
            }
            globalOperations.render_update = true;
        }
    }

    public override void onEnter(){
        cursorGpos = (0, 0);
        textCursor.sizeDelta = new Vector2(gridManager.get_col_size(), gridManager.get_row_size());
        textCursor.localScale = new Vector3(1, 1, 1);
    }

    public override void onExit(){
        textCursor.localScale = new Vector3(0, 0, 0);
    }

    public void renderTextCursor(){
        textCursor.anchoredPosition = new Vector2(
            gridManager.get_col_size() * cursorGpos.col + gridManager.ui_manager.ui_panel_transform.rect.width, 
            gridManager.get_row_size() * gridManager.invert_row_pos(cursorGpos.row)
        );
    }
}
