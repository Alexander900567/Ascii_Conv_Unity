using UnityEngine;
using System.Collections.Generic;

public class Bucket : Tool
{
    [SerializeField] private UndoRedo undoRedo;
    private char initialCharacter;
    private List<(int row, int col)> changedList;

    public override void handleInput()
    {
        if (isMouseOnGrid() && globalOperations.controls.Grid.MainClick.triggered){
            clickedGrid = true;
            draw();
        }
    }

    public override void draw(){
        changedList = new List<(int row, int col)>();
        (int row, int col) gpos = gridManager.getGridPos(); 
        initialCharacter = gridManager.getGarrSpace(gpos.row, gpos.col);
        if(initialCharacter == globalOperations.activeLetter){
            return;
        }
        fillSpace(gpos);
        List<(int, int, char)> undoItem = new List<(int, int, char)>();
        foreach ((int row, int col) space in changedList){
            undoItem.Add((space.row, space.col, initialCharacter));
        }
        undoRedo.addUndoElement(undoItem);
    }

    private void fillSpace((int row, int col) gridSpace){
        if (gridManager.getGarrSpace(gridSpace.row, gridSpace.col) != initialCharacter){
            return;
        }
        gridManager.addToGridArray(gridSpace.row, gridSpace.col, globalOperations.activeLetter);
        changedList.Add((gridSpace.row, gridSpace.col));
        fillSpace((row: gridSpace.row + 1, col: gridSpace.col));
        fillSpace((row: gridSpace.row - 1, col: gridSpace.col));
        fillSpace((row: gridSpace.row, col: gridSpace.col + 1));
        fillSpace((row: gridSpace.row, col: gridSpace.col - 1));
    }
}
