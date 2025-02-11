using System.Collections.Generic;
using UnityEngine;

public class RectangleSelector : Tool
{
    [SerializeField] private GameObject selectorBox;
    private GameObject selectorBoxInstance;
    [SerializeField] private GameObject commitButton;

    private bool active;
    private (int row, int col) topLeft;
    private (int row, int col) botRight;
    private (int row, int col) size;
    private List<(int, int, char)> originalBuffer;

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
        size = (-1, -1);
        originalBuffer = new List<(int, int, char)>();
    }

    public override void onExit(){
        if (active){
            resetBox();
        }
    }

    public void renderRectangleSelector(){
        selectorBoxInstance.GetComponent<RectTransform>().sizeDelta = new Vector2(
            gridManager.getColSize() * size.col,
            gridManager.getRowSize() * size.row 
        );
        selectorBoxInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            gridManager.uiPanelTransform.rect.width + topLeft.col * gridManager.getColSize(),
            gridManager.invertRowPos(botRight.row) * gridManager.getRowSize()
        );
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
                    gridManager.addToGridArray(row, col, ' ');
                }
            } 
        }
    }

    public void resetBox(){
        active = false;
        topLeft = (-1, -1);
        botRight = (-1, -1);
        size = (-1, -1);
        startGpos = (-1, -1);

        gridManager.writePbufferToArray();
        Destroy(selectorBoxInstance);
        commitButton.SetActive(false);
    }

    public void changeCorners((int row, int col) newGpos){
        topLeft = (
            Mathf.Min(startGpos.row, newGpos.row),
            Mathf.Min(startGpos.col, newGpos.col)
        );
        botRight = (
            Mathf.Max(startGpos.row, newGpos.row),
            Mathf.Max(startGpos.col, newGpos.col)
        );
        size = (botRight.row - topLeft.row + 1, botRight.col - topLeft.col + 1); 
    }



}
