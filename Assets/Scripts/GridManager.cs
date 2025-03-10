using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public GlobalOperations global;
    public UndoRedo undoRedo;
    public RectTransform canvasTransform;
    public RectTransform gridSpaceOutline;
    public RectTransform uiPanelTransform;
    public RawImage gridImage;
    public GameObject FontSourceObj;

    private Texture2D workspaceTexture;
    private List<List<char>> gridArray = new List<List<char>>();
    private List<(int, int, char)> previewBuffer = new List<(int, int, char)>(); //row, col, input
    private float colSize;
    private float rowSize;
    [SerializeField] private int colCount;
    [SerializeField] private int rowCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colSize = (float) (Screen.width - getUiBarWidth()) / (float) colCount;
        rowSize =  (float) Screen.height / (float) rowCount;

        for (int row = 0; row < rowCount; row++){
            gridArray.Add(new List<char>());
            for (int col = 0; col < colCount; col++){
                gridArray[row].Add(' ');
            }
        }

        int textureWidth = FontSourceObj.GetComponent<FontSource>().getCharWidth() * colCount;
        int textureHeight = FontSourceObj.GetComponent<FontSource>().getCharHeight() * rowCount;
        createWorkspaceTexture(textureWidth, textureHeight);

        gridSpaceOutline.sizeDelta = new Vector2(colSize, rowSize);
    }

    // Update is called once per frame
    void Update()
    {
        if (global.renderUpdate){
            renderGrid();
            global.renderUpdate = false;
        }
        renderGridOutline();
    }

    private void createWorkspaceTexture(int width, int height){
        TextureFormat sourceFontFormat = FontSourceObj.GetComponent<FontSource>().fontTexture.format;
        workspaceTexture = new Texture2D(width, height, sourceFontFormat, true);
        workspaceTexture.Apply(false, true);
    }
    
    private void renderGrid(){
        //Debug.Log("new render");

        //add preview buffer to a temp version of grid
        List<List<char>> renderArray = getGridArray();
        foreach ((int, int, char) item in previewBuffer){
            renderArray[item.Item1][item.Item2] = item.Item3;
        }


        FontSource fontSourceScript = FontSourceObj.GetComponent<FontSource>();
        Texture2D fontSourceTexture = fontSourceScript.fontTexture;
        int charTextureWidth = fontSourceScript.getCharWidth();
        int charTextureHeight = fontSourceScript.getCharHeight();
        for (int row = 0; row < rowCount; row++){
            for (int col = 0; col < colCount; col++){
                (int, int) location = fontSourceScript.getLocationOfChar(renderArray[row][col]);
                Graphics.CopyTexture(
                    fontSourceTexture, 0, 0, 
                    location.Item1, location.Item2, charTextureWidth, charTextureHeight, 
                    workspaceTexture, 0, 0,
                    (int)(col * charTextureWidth), (int)((rowCount - 1 - row) * charTextureHeight)
                );
            }
        } 

        gridImage.texture = workspaceTexture;
    }
    
    private void renderGridOutline(){
        (int row, int col) gridPos = getGridPos();
        gridSpaceOutline.GetComponent<HollowBoxAttach>().renderBox(gridPos, gridPos);
    }

    public void writePbufferToArray(bool addToUndo=true){
        if (addToUndo){
            undoRedo.addUndoFromPbuffer();
        }
        foreach ((int, int, char) item in previewBuffer){
            gridArray[item.Item1][item.Item2] = item.Item3;
        }
        emptyPreviewBuffer();
    }

    public void copyGridToClipboard(){
        string gridString = "";
        foreach(List<char> row in gridArray){
            foreach(char element in row){
                gridString += element;
            }
            gridString += "\n";
        }
        EditorGUIUtility.systemCopyBuffer = gridString;
    }

    public (int row, int col) getGridPos(bool invertRow=true){
        Vector3 mousePos = Input.mousePosition;
        int col = (int) ((mousePos[0] - getUiBarWidth()) / colSize);
        int row = (int) (mousePos[1] / rowSize);

        if (col < 0) { col = 0; }
        else if (col >= colCount) { col = colCount - 1; }
        if (row < 0) { row = 0; }
        else if (row >= rowCount) { row = rowCount - 1; }

        if (invertRow){
            row = rowCount - 1 - row;
        }
        return (row: row, col: col);
    }

    public int invertRowPos(int row){
        return rowCount - 1 - row;
    }

    //---setters---

    public void addToPreviewBuffer(int row, int col, char input){

        if (row >= rowCount || row < 0){ return; }
        else if (col >= colCount || col < 0){ return; }

        previewBuffer.Add((row, col, input));
    }

    public void editPbufferItemPos(int ind, int rowDelta, int colDelta){
        previewBuffer[ind] = (
            previewBuffer[ind].Item1 + rowDelta,
            previewBuffer[ind].Item2 + colDelta,
            previewBuffer[ind].Item3
        );
    }

    public void emptyPreviewBuffer(){
        previewBuffer.Clear();        
    }

    public void addToGridArray(int row, int col, char input){

        if (row >= rowCount || row < 0){ return; }
        else if (col >= colCount || col < 0){ return; }

        gridArray[row][col] = input;
    }

    //---getters---

    public int getColCount(){
        return colCount;
    }
    
    public int getRowCount(){
        return rowCount;
    }

    public float getColSize(){
        return colSize;
    }

    public float getRowSize(){
        return rowSize;
    }

    public char getGarrSpace(int row, int col){
        return gridArray[row][col];
    }

    public int getPbufferLength(){
        return previewBuffer.Count;
    }

    public float getUiBarWidth(){
        return uiPanelTransform.rect.width;
    }

    public List<(int, int, char)> getPbuffer(){
        return new List<(int, int, char)>(previewBuffer);
    }

    public List<List<char>> getGridArray(){
        List<List<char>> arrayCopy = new List<List<char>>();
        for (int row = 0; row < rowCount; row++){
            arrayCopy.Add(new List<char>(gridArray[row]));
        }
        return arrayCopy;
    }

}

/*
Component[] components = gridTextCols[col].GetComponents(typeof(Component));
foreach(Component component in components) {
    Debug.Log(component.ToString());
}
*/